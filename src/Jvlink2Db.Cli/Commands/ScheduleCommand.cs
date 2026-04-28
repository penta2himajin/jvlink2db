using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using Jvlink2Db.Pipeline.Schedule;
using Microsoft.Win32.TaskScheduler;

namespace Jvlink2Db.Cli.Commands;

/// <summary>
/// <c>schedule install</c> / <c>schedule uninstall</c> /
/// <c>schedule status</c> — drives Windows Task Scheduler. All
/// jvlink2db tasks live under the <c>\jvlink2db\</c> folder so
/// status / uninstall stay scoped.
///
/// Tasks default to "run only when the user is logged on" with
/// <see cref="TaskRunLevel.Highest"/> so JV-Link COM is reachable
/// in the user's session. Pass <c>--always</c> to register the task
/// with stored credentials so it runs regardless of logon state.
/// </summary>
public static class ScheduleCommand
{
    public static Command Create()
    {
        var cmd = new Command("schedule", "Manage Windows Task Scheduler entries for jvlink2db.")
        {
            Install(),
            Uninstall(),
            Status(),
        };

        return cmd;
    }

    private static Command Install()
    {
        var name = new Option<string>("--name", "Task name within the jvlink2db folder.") { IsRequired = true };
        var mode = new Option<string>("--mode", "normal | weekly.") { IsRequired = true };
        var connection = ModeRunner.Connection();
        var schema = ModeRunner.Schema();
        var operationalSchema = ModeRunner.OperationalSchema();
        var dataspec = ModeRunner.Dataspec();
        var sid = ModeRunner.Sid();
        var since = new Option<string?>("--since", "fromtime; required for weekly, optional for normal.");
        var daily = new Option<string?>("--daily", "Run once a day at HH:MM[:SS]. Mutually exclusive with --every.");
        var every = new Option<string?>("--every", "Run every <duration>. Accepts HH:MM:SS or shorthand Ns/Nm/Nh/Nd. Mutually exclusive with --daily.");
        var always = new Option<bool>("--always", "Run whether the user is logged on or not (requires storing the user's password — pass --password).");
        var password = new Option<string?>("--password", "Password for the current user. Required with --always.");

        var cmd = new Command("install", "Register or replace a scheduled task.")
        {
            name, mode, connection, schema, operationalSchema, dataspec, sid, since, daily, every, always, password,
        };

        cmd.SetHandler(ctx =>
        {
            var nameValue = ctx.ParseResult.GetValueForOption(name)!;
            var modeValue = ctx.ParseResult.GetValueForOption(mode)!;

            ScheduleTrigger trigger;
            try
            {
                trigger = ScheduleTriggerParser.Parse(
                    ctx.ParseResult.GetValueForOption(daily),
                    ctx.ParseResult.GetValueForOption(every));
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                ctx.ExitCode = 1;
                return;
            }

            ScheduleArgsBuilder.InstallSpec spec;
            string args;
            try
            {
                spec = new ScheduleArgsBuilder.InstallSpec(
                    Mode: modeValue,
                    Connection: ctx.ParseResult.GetValueForOption(connection)!,
                    Schema: ctx.ParseResult.GetValueForOption(schema)!,
                    OperationalSchema: ctx.ParseResult.GetValueForOption(operationalSchema)!,
                    Dataspec: ctx.ParseResult.GetValueForOption(dataspec)!,
                    Sid: ctx.ParseResult.GetValueForOption(sid)!,
                    Since: ctx.ParseResult.GetValueForOption(since));
                args = ScheduleArgsBuilder.Build(spec);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                ctx.ExitCode = 1;
                return;
            }

            var alwaysValue = ctx.ParseResult.GetValueForOption(always);
            var passwordValue = ctx.ParseResult.GetValueForOption(password);
            if (alwaysValue && string.IsNullOrEmpty(passwordValue))
            {
                Console.Error.WriteLine("schedule install --always requires --password (Task Scheduler stores the credential to launch without an interactive session).");
                ctx.ExitCode = 1;
                return;
            }

            var exePath = Environment.ProcessPath
                ?? throw new InvalidOperationException("Cannot resolve current process path.");
            var workingDir = Path.GetDirectoryName(exePath)
                ?? throw new InvalidOperationException("Cannot resolve current process directory.");

            using var ts = new TaskService();
            EnsureFolder(ts);

            var def = ts.NewTask();
            def.RegistrationInfo.Description =
                $"jvlink2db {modeValue} acquisition for dataspec {spec.Dataspec} (schema {spec.Schema}).";
            def.RegistrationInfo.Author = "jvlink2db";
            def.Principal.RunLevel = TaskRunLevel.Highest;
            def.Settings.AllowDemandStart = true;
            def.Settings.StartWhenAvailable = true;
            def.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;

            switch (trigger)
            {
                case DailyAtTrigger d:
                    def.Triggers.Add(new DailyTrigger
                    {
                        StartBoundary = DateTime.Today.Add(d.TimeOfDay),
                    });
                    break;
                case EveryIntervalTrigger e:
                    def.Triggers.Add(new TimeTrigger
                    {
                        StartBoundary = DateTime.Now.AddSeconds(30),
                        Repetition = new RepetitionPattern(e.Interval, TimeSpan.Zero),
                    });
                    break;
            }

            def.Actions.Add(new ExecAction(QuoteIfNeeded(exePath), args, workingDir));

            var folder = ts.GetFolder(TaskNamingPolicy.FolderPath);
            if (alwaysValue)
            {
                folder.RegisterTaskDefinition(
                    nameValue, def, TaskCreation.CreateOrUpdate,
                    Environment.UserName, passwordValue, TaskLogonType.Password);
            }
            else
            {
                folder.RegisterTaskDefinition(
                    nameValue, def, TaskCreation.CreateOrUpdate,
                    Environment.UserName, null, TaskLogonType.InteractiveToken);
            }

            Console.WriteLine($"Installed: {TaskNamingPolicy.FullPath(nameValue)}");
            Console.WriteLine($"  Action : \"{exePath}\" {args}");
            Console.WriteLine($"  Trigger: {DescribeTrigger(trigger)}");
            Console.WriteLine($"  Run as : {Environment.UserName} ({(alwaysValue ? "always" : "only when logged on")}, highest privileges)");
            ctx.ExitCode = 0;
        });

        return cmd;
    }

    private static Command Uninstall()
    {
        var name = new Option<string>("--name", "Task name within the jvlink2db folder.") { IsRequired = true };

        var cmd = new Command("uninstall", "Remove a scheduled task previously installed by jvlink2db.")
        {
            name,
        };

        cmd.SetHandler(ctx =>
        {
            var nameValue = ctx.ParseResult.GetValueForOption(name)!;

            using var ts = new TaskService();
            var folder = ts.RootFolder.SubFolders.FirstOrDefault(f =>
                string.Equals(f.Name, "jvlink2db", StringComparison.OrdinalIgnoreCase));
            if (folder is null)
            {
                Console.Error.WriteLine($"No jvlink2db task folder exists; nothing to remove.");
                ctx.ExitCode = 1;
                return;
            }

            var task = folder.AllTasks.FirstOrDefault(t =>
                string.Equals(t.Name, nameValue, StringComparison.Ordinal));
            if (task is null)
            {
                Console.Error.WriteLine($"Task '{nameValue}' not found under {TaskNamingPolicy.FolderPath}.");
                ctx.ExitCode = 1;
                return;
            }

            folder.DeleteTask(nameValue, exceptionOnNotExists: false);
            Console.WriteLine($"Removed: {TaskNamingPolicy.FullPath(nameValue)}");
            ctx.ExitCode = 0;
        });

        return cmd;
    }

    private static Command Status()
    {
        var cmd = new Command("status", "List jvlink2db scheduled tasks with their next run time and last result.");
        cmd.SetHandler(ctx =>
        {
            using var ts = new TaskService();
            var folder = ts.RootFolder.SubFolders.FirstOrDefault(f =>
                string.Equals(f.Name, "jvlink2db", StringComparison.OrdinalIgnoreCase));
            var tasks = folder is null
                ? Array.Empty<Microsoft.Win32.TaskScheduler.Task>()
                : folder.AllTasks.ToArray();
            if (tasks.Length == 0)
            {
                Console.WriteLine("(no jvlink2db scheduled tasks installed)");
                ctx.ExitCode = 0;
                return;
            }

            Console.WriteLine($"{TaskNamingPolicy.FolderPath}");
            foreach (var task in tasks.OrderBy(t => t.Name, StringComparer.Ordinal))
            {
                Console.WriteLine($"  {task.Name}");
                Console.WriteLine($"    Next run    : {FormatDate(task.NextRunTime)}");
                Console.WriteLine($"    Last run    : {FormatDate(task.LastRunTime)}");
                Console.WriteLine($"    Last result : {task.LastTaskResult}");
                Console.WriteLine($"    State       : {task.State}");
            }

            ctx.ExitCode = 0;
        });

        return cmd;
    }

    private static void EnsureFolder(TaskService ts)
    {
        var existing = ts.RootFolder.SubFolders.FirstOrDefault(f =>
            string.Equals(f.Name, "jvlink2db", StringComparison.OrdinalIgnoreCase));
        if (existing is null)
        {
            ts.RootFolder.CreateFolder("jvlink2db");
        }
    }

    private static string DescribeTrigger(ScheduleTrigger trigger) => trigger switch
    {
        DailyAtTrigger d => $"daily at {d.TimeOfDay:hh\\:mm\\:ss}",
        EveryIntervalTrigger e => $"every {e.Interval}",
        _ => trigger.ToString() ?? "(unknown)",
    };

    private static string FormatDate(DateTime dt) =>
        dt == DateTime.MinValue ? "(never)" : dt.ToString("yyyy-MM-dd HH:mm:ss");

    private static string QuoteIfNeeded(string path) =>
        path.Contains(' ', StringComparison.Ordinal) ? $"\"{path}\"" : path;
}
