using System.CommandLine;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Jvlink2Db.Cli;

public static class Program
{
    public static int Main(string[] args)
    {
        IConfiguration configuration = BuildConfiguration(args);
        _ = configuration;

        RootCommand root = new("jvlink2db — JRA-VAN Data Lab. importer for PostgreSQL.");

        return root.Invoke(args);
    }

    internal static IConfiguration BuildConfiguration(string[] args)
    {
        string baseDirectory = AppContext.BaseDirectory;

        return new ConfigurationBuilder()
            .SetBasePath(baseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: "JVLINK2DB_")
            .AddCommandLine(args)
            .Build();
    }
}
