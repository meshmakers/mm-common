using Meshmakers.Common.CommandLineParser;
using Meshmakers.Common.Shared.Services;
using NSubstitute;
using Xunit;

namespace Meshmakers.Common.CommandLineParserTests;

/// <summary>
///     Verifies that <see cref="ParserService.ShowUsageInformation" /> composes invocation strings
///     from each registered <see cref="CodeSample" /> by combining the application name, the
///     command verb passed to <see cref="ParserService.AddSample" />, and the live
///     <see cref="IArgument.ShortTerm" /> of every bound argument. Renaming the short term on the
///     argument propagates here automatically — that's the whole point of the typed sample model.
/// </summary>
public class SampleCompositionTests
{
    private readonly IConsoleService _consoleService = Substitute.For<IConsoleService>();
    private readonly IEnvironmentService _environmentService = Substitute.For<IEnvironmentService>();

    private ParserService NewService() => new(_environmentService, _consoleService);

    private static Argument ValueArgument(string shortTerm, string longTerm) =>
        new(shortTerm, longTerm, ["help"], isMandatoryArgument: true, mandatoryValuesCount: 1, areOptionalValuesAllowed: false);

    private static Argument FlagArgument(string shortTerm, string longTerm) =>
        new(shortTerm, longTerm, ["help"], isMandatoryArgument: false, mandatoryValuesCount: 0, areOptionalValuesAllowed: false);

    [Fact]
    public void ShowUsage_RendersInvocation_WithBoundValue()
    {
        var service = NewService();
        var tenantArg = ValueArgument("tid", "tenantId");
        var sample = new CodeSample([new CodeSampleArgument(tenantArg, "newtenant")], "Basic usage");
        service.AddSample("Create", sample);

        service.ShowUsageInformation("octo-cli");

        _consoleService.Received().WriteLine("octo-cli -c Create -tid \"newtenant\"");
        _consoleService.Received().WriteLine("  Basic usage");
    }

    [Fact]
    public void ShowUsage_RendersFlag_WithoutQuotedValue()
    {
        var service = NewService();
        var waitArg = FlagArgument("w", "wait");
        var sample = new CodeSample([new CodeSampleArgument(waitArg)], "Interactive with wait");
        service.AddSample("FixAll", sample);

        service.ShowUsageInformation("octo-cli");

        _consoleService.Received().WriteLine("octo-cli -c FixAll -w");
    }

    [Fact]
    public void ShowUsage_RendersMultipleArguments_InOrder()
    {
        var service = NewService();
        var tenantArg = ValueArgument("tid", "tenantId");
        var dbArg = ValueArgument("db", "database");
        var sample = new CodeSample(
            [
                new CodeSampleArgument(tenantArg, "newtenant"),
                new CodeSampleArgument(dbArg, "newtenant_db"),
            ],
            "Create with explicit database");
        service.AddSample("Create", sample);

        service.ShowUsageInformation("octo-cli");

        _consoleService.Received().WriteLine("octo-cli -c Create -tid \"newtenant\" -db \"newtenant_db\"");
    }

    [Fact]
    public void ShowUsage_RendersMultipleSamples()
    {
        var service = NewService();
        var verboseArg = FlagArgument("v", "verbose");
        service.AddSample("Status", new CodeSample([], "Default"));
        service.AddSample("Status", new CodeSample([new CodeSampleArgument(verboseArg)], "Verbose"));

        service.ShowUsageInformation("octo-cli");

        _consoleService.Received().WriteLine("octo-cli -c Status");
        _consoleService.Received().WriteLine("  Default");
        _consoleService.Received().WriteLine("octo-cli -c Status -v");
        _consoleService.Received().WriteLine("  Verbose");
    }

    [Fact]
    public void ShowUsage_DoesNotRender_ExpectedOutput()
    {
        // ExpectedOutput is documentation-only — CLI help (this method) must ignore it.
        var service = NewService();
        var sample = new CodeSample(
            [],
            "Show",
            expectedOutput: "NAME  STATE\nfoo   OK");
        service.AddSample("Status", sample);

        service.ShowUsageInformation("octo-cli");

        _consoleService.DidNotReceive().WriteLine(Arg.Is<string>(s => s.Contains("foo   OK")));
        _consoleService.DidNotReceive().WriteLine(Arg.Is<string>(s => s.Contains("NAME  STATE")));
    }
}
