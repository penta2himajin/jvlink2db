using System;
using Jvlink2Db.Pipeline.Schedule;
using Xunit;

namespace Jvlink2Db.Pipeline.Tests.Schedule;

public class ScheduleArgsBuilderTests
{
    [Fact]
    public void Build_emits_normal_subcommand_with_required_options()
    {
        var spec = NewSpec(mode: "normal", since: null);

        var args = ScheduleArgsBuilder.Build(spec);

        Assert.StartsWith("normal ", args, StringComparison.Ordinal);
        Assert.Contains("--dataspec RACE", args, StringComparison.Ordinal);
        Assert.Contains("--schema jv", args, StringComparison.Ordinal);
        Assert.Contains("--operational-schema jvlink2db", args, StringComparison.Ordinal);
        Assert.Contains("--sid jvlink2db/0.1", args, StringComparison.Ordinal);
        Assert.DoesNotContain("--since", args, StringComparison.Ordinal);
    }

    [Fact]
    public void Build_quotes_connection_string_with_spaces_and_semicolons()
    {
        var spec = NewSpec("normal", since: null) with
        {
            Connection = "Host=localhost;Port=5433;Database=postgres",
        };

        var args = ScheduleArgsBuilder.Build(spec);

        Assert.Contains("--connection \"Host=localhost;Port=5433;Database=postgres\"", args, StringComparison.Ordinal);
    }

    [Fact]
    public void Build_includes_since_for_weekly_mode()
    {
        var spec = NewSpec("weekly", since: "20260415000000");

        var args = ScheduleArgsBuilder.Build(spec);

        Assert.StartsWith("weekly ", args, StringComparison.Ordinal);
        Assert.Contains("--since 20260415000000", args, StringComparison.Ordinal);
    }

    [Fact]
    public void Build_throws_when_weekly_has_no_since()
    {
        var spec = NewSpec("weekly", since: null);
        Assert.Throws<ArgumentException>(() => ScheduleArgsBuilder.Build(spec));
    }

    [Fact]
    public void Build_rejects_modes_other_than_normal_or_weekly()
    {
        var spec = NewSpec("setup", since: null);
        Assert.Throws<ArgumentException>(() => ScheduleArgsBuilder.Build(spec));
    }

    [Fact]
    public void Build_escapes_inner_quotes_in_connection_string()
    {
        var spec = NewSpec("normal", since: null) with
        {
            Connection = "Password=ab\"cd;Host=localhost",
        };

        var args = ScheduleArgsBuilder.Build(spec);

        Assert.Contains("--connection \"Password=ab\\\"cd;Host=localhost\"", args, StringComparison.Ordinal);
    }

    private static ScheduleArgsBuilder.InstallSpec NewSpec(string mode, string? since) => new(
        Mode: mode,
        Connection: "Host=localhost;Username=postgres",
        Schema: "jv",
        OperationalSchema: "jvlink2db",
        Dataspec: "RACE",
        Sid: "jvlink2db/0.1",
        Since: since);
}
