using System;
using System.CommandLine;
using System.Threading.Tasks;
using Jvlink2Db.Cli.Commands;
using Microsoft.Extensions.Configuration;

namespace Jvlink2Db.Cli;

public static class Program
{
    public static Task<int> Main(string[] args)
    {
        IConfiguration configuration = BuildConfiguration(args);
        _ = configuration;

        var root = new RootCommand("jvlink2db — JRA-VAN Data Lab. importer for PostgreSQL.")
        {
            ProbeCommand.Create(),
            SetupCommand.Create(),
            RangeCommand.Create(),
            NormalCommand.Create(),
            WeeklyCommand.Create(),
            ScheduleCommand.Create(),
        };

        return root.InvokeAsync(args);
    }

    internal static IConfiguration BuildConfiguration(string[] args)
    {
        var baseDirectory = AppContext.BaseDirectory;

        return new ConfigurationBuilder()
            .SetBasePath(baseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: "JVLINK2DB_")
            .AddCommandLine(args)
            .Build();
    }
}
