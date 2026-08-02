using Meshmakers.Common.CommandLineParser;
using Meshmakers.Common.Shared.Services;
using NSubstitute;
using Xunit;

namespace Meshmakers.Common.CommandLineParserTests;

/// <summary>
///     Covers the implicit help flag: how it is matched, that it suppresses validation of an intentionally
///     incomplete command line, and that it never shadows an argument a command declares itself.
/// </summary>
public class HelpArgumentTests
{
    private readonly IConsoleService _stubIConsoleService = Substitute.For<IConsoleService>();
    private readonly IEnvironmentService _stubIEnvironmentService = Substitute.For<IEnvironmentService>();

    /// <summary>
    ///     Builds a parser with a mandatory command selector and one command "first" that has a mandatory
    ///     argument of its own, which is what makes the validation-suppression observable.
    /// </summary>
    private ParserService CreateParserService(out CommandArgumentValue firstCommand,
        out ICommandArgument commandArgument)
    {
        var parserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        parserService.AddHelpArgument();

        commandArgument = parserService.AddCommandArgument("c", "command", ["test command"], true);

        firstCommand = new CommandArgumentValue("g", "first", "first command");
        firstCommand.AddArgument("a", "aLongTerm", ["test a"], true, 1);
        commandArgument.AddCommandValue(firstCommand);

        return parserService;
    }

    [Theory]
    [InlineData("--help")]
    [InlineData("-help")]
    [InlineData("/help")]
    [InlineData("--HELP")]
    [InlineData("-h")]
    [InlineData("-?")]
    [InlineData("/?")]
    public void ParseAndValidate_Help_WithoutCommand_OK(string helpTerm)
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", helpTerm]);
        var parserService = CreateParserService(out _, out _);

        // The command selector is mandatory, so without help suppressing validation this would throw.
        parserService.ParseAndValidate();

        Assert.True(parserService.IsHelpRequested);
    }

    [Fact]
    public void ParseAndValidate_Help_AfterCommand_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-c", "first", "--help"]);
        var parserService = CreateParserService(out var firstCommand, out var commandArgument);

        parserService.ParseAndValidate();

        Assert.True(firstCommand.IsHelpRequested);
        Assert.True(parserService.IsHelpRequested);
        Assert.Equal("first", parserService.GetArgumentValue(commandArgument).GetValue<string>());
    }

    [Fact]
    public void ParseAndValidate_Help_BeforeCommand_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "--help", "-c", "first"]);
        var parserService = CreateParserService(out var firstCommand, out _);

        parserService.ParseAndValidate();

        // The command layer inherits the pending request, otherwise its mandatory argument would be demanded.
        Assert.True(firstCommand.IsHelpRequested);
        Assert.True(parserService.IsHelpRequested);
    }

    [Fact]
    public void ParseAndValidate_Help_WithIncompleteArgumentValues_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-c", "first", "-a", "--help"]);
        var parserService = CreateParserService(out var firstCommand, out _);

        parserService.ParseAndValidate();

        Assert.True(firstCommand.IsHelpRequested);
    }

    [Fact]
    public void ParseAndValidate_NoHelp_MandatoryArgumentsStillValidated_Fail()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-c", "first"]);
        var parserService = CreateParserService(out _, out _);

        Assert.Throws<MandatoryArgumentsMissingException>(() => parserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_ShortHelp_DeclaredArgumentWins_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-c", "second", "-h", "myhost"]);

        var parserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        parserService.AddHelpArgument();
        var commandArgument = parserService.AddCommandArgument("c", "command", ["test command"], true);

        var secondCommand = new CommandArgumentValue("g", "second", "second command");
        var hostArgument = secondCommand.AddArgument("h", "host", ["host"], true, 1);
        commandArgument.AddCommandValue(secondCommand);

        parserService.ParseAndValidate();

        // A command owning -h keeps it; its help remains reachable via --help or -?.
        Assert.False(secondCommand.IsHelpRequested);
        Assert.Equal("myhost", secondCommand.GetArgumentValue(hostArgument).GetValue<string>());
    }

    [Fact]
    public void ParseAndValidate_LongHelp_CommandDeclaringShortHelp_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-c", "second", "--help"]);

        var parserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        parserService.AddHelpArgument();
        var commandArgument = parserService.AddCommandArgument("c", "command", ["test command"], true);

        var secondCommand = new CommandArgumentValue("g", "second", "second command");
        secondCommand.AddArgument("h", "host", ["host"], true, 1);
        commandArgument.AddCommandValue(secondCommand);

        parserService.ParseAndValidate();

        Assert.True(secondCommand.IsHelpRequested);
    }

    [Fact]
    public void ParseAndValidate_HelpNotDeclared_Fail()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "--help"]);

        var parserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        parserService.AddCommandArgument("c", "command", ["test command"], true);

        // Layers that never called AddHelpArgument keep the previous behaviour.
        Assert.Throws<InvalidParameterException>(() => parserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_HelpFlagIsNotListedAsArgument_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "--help"]);
        var parserService = CreateParserService(out _, out _);
        var helpArgument = parserService.AddHelpArgument();

        parserService.ParseAndValidate();

        // Help is tracked by IsHelpRequested, not as an argument value of the layer.
        Assert.False(parserService.IsArgumentUsed(helpArgument));
    }

    [Fact]
    public void AddHelpArgument_CalledTwice_ReturnsSameArgument_OK()
    {
        var parserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);

        Assert.Same(parserService.AddHelpArgument(), parserService.AddHelpArgument());
    }
}
