using Meshmakers.Common.CommandLineParser;
using Meshmakers.Common.CommandLineParser.Commands;
using Meshmakers.Common.Shared.Services;
using NSubstitute;
using Xunit;

namespace Meshmakers.Common.CommandLineParserTests;

/// <summary>
///     Covers help narrowed down to a command group: how a topic is resolved to a group, and what the group
///     overview and the group listing render.
/// </summary>
public class CommandGroupHelpTests
{
    private readonly IConsoleService _stubIConsoleService = Substitute.For<IConsoleService>();
    private readonly IEnvironmentService _stubIEnvironmentService = Substitute.For<IEnvironmentService>();
    private readonly IParserService _stubParserService = Substitute.For<IParserService>();
    private readonly ICommandArgument _stubCommandArgument = Substitute.For<ICommandArgument>();

    public CommandGroupHelpTests()
    {
        _stubParserService.AddCommandArgument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string[]>(),
            Arg.Any<bool>()).Returns(_ => _stubCommandArgument);
    }

    /// <summary>
    ///     Commands spread over three groups, one of which ("Reporting") also exists as a command name so the
    ///     collision handling is covered by the same fixture.
    /// </summary>
    private static ICommand[] CreateCommands()
    {
        return
        [
            CreateCommand("Identity Services", "CreateUser", "Creates a user."),
            CreateCommand("Identity Services", "DeleteUser", "Deletes a user."),
            CreateCommand("Context Management", "UseContext", "Switches the context."),
            CreateCommand("Reporting", "Reporting", "Runs the report.")
        ];
    }

    private static ICommand CreateCommand(string group, string value, string description)
    {
        var command = Substitute.For<ICommand>();
        var commandArgumentValue = Substitute.For<ICommandArgumentValue>();
        commandArgumentValue.Group.Returns(group);
        commandArgumentValue.Value.Returns(value);
        commandArgumentValue.Description.Returns(description);
        command.CommandArgumentValue.Returns(commandArgumentValue);

        return command;
    }

    private CommandParser CreateCommandParser(string? helpTopic)
    {
        _stubParserService.IsHelpRequested.Returns(true);
        _stubParserService.HelpTopic.Returns(helpTopic);

        return new CommandParser(_stubParserService, CreateCommands());
    }

    [Theory]
    [InlineData("Identity Services")]
    [InlineData("identity services")]
    [InlineData("identity")]
    [InlineData("Ident")]
    [InlineData("Services")]
    public async Task ParseAndValidateAsync_TopicResolvesToGroup_OK(string topic)
    {
        var commandParser = CreateCommandParser(topic);

        await commandParser.ParseAndValidateAsync("Demo.exe");

        // Whatever the caller typed, the canonical group name is what gets rendered.
        _stubParserService.Received(1).ShowGroupUsageInformation("Demo.exe", _stubCommandArgument,
            "Identity Services", null);
    }

    [Fact]
    public async Task ParseAndValidateAsync_WithoutTopic_ShowsGroupOverview_OK()
    {
        var commandParser = CreateCommandParser(null);

        await commandParser.ParseAndValidateAsync("Demo.exe");

        _stubParserService.Received(1).ShowGroupOverviewInformation("Demo.exe", _stubCommandArgument);
        _stubParserService.DidNotReceive().ShowUsageInformation(Arg.Any<string>());
    }

    [Fact]
    public async Task ParseAndValidateAsync_TopicAll_ShowsFullUsage_OK()
    {
        var commandParser = CreateCommandParser("all");

        await commandParser.ParseAndValidateAsync("Demo.exe");

        _stubParserService.Received(1).ShowUsageInformation("Demo.exe");
    }

    [Fact]
    public async Task ParseAndValidateAsync_TopicIsCommand_ShowsCommandUsage_OK()
    {
        var commandParser = CreateCommandParser("createuser");

        await commandParser.ParseAndValidateAsync("Demo.exe");

        _stubParserService.Received(1).ShowCommandUsageInformation("Demo.exe", _stubCommandArgument,
            Arg.Is<ICommandArgumentValue>(x => x.Value == "CreateUser"), Arg.Any<CommandDocumentation?>());
    }

    [Fact]
    public async Task ParseAndValidateAsync_TopicIsGroupAndCommand_GroupWinsAndCommandIsPointedOut_OK()
    {
        var commandParser = CreateCommandParser("Reporting");

        await commandParser.ParseAndValidateAsync("Demo.exe");

        // The group has no second way in, so it takes the name — but the command must not vanish silently.
        _stubParserService.Received(1).ShowGroupUsageInformation("Demo.exe", _stubCommandArgument, "Reporting",
            "Reporting");
        _stubParserService.DidNotReceive().ShowCommandUsageInformation(Arg.Any<string>(), Arg.Any<IArgument>(),
            Arg.Any<ICommandArgumentValue>(), Arg.Any<CommandDocumentation?>());
    }

    [Theory]
    [InlineData("Unknown")]
    [InlineData("e")] // matches "Identity Services", "Context Management" and "Reporting"
    [InlineData("Creat")] // fuzzy matching is for groups only, commands resolve exactly
    public async Task ParseAndValidateAsync_TopicWithoutUniqueMatch_Fail(string topic)
    {
        var commandParser = CreateCommandParser(topic);

        var exception = await Assert.ThrowsAsync<InvalidParameterException>(
            () => commandParser.ParseAndValidateAsync("Demo.exe"));

        Assert.Contains("Identity Services", exception.Message);
        Assert.Contains("all", exception.Message);
    }

    [Fact]
    public void ShowGroupOverviewInformation_ListsEveryGroupWithItsCount_OK()
    {
        var parserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var commandArgument = parserService.AddCommandArgument("c", "command", ["test command"], true);
        AddCommandValues(commandArgument);

        parserService.ShowGroupOverviewInformation("my.exe", commandArgument);

        _stubIConsoleService.Received().WriteColumnLine("  Identity Services", Arg.Any<int>(), "2");
        _stubIConsoleService.Received().WriteColumnLine("  Context Management", Arg.Any<int>(), "1");
        _stubIConsoleService.Received().WriteColumnLine("  Reporting", Arg.Any<int>(), "1");
    }

    [Fact]
    public void ShowGroupUsageInformation_RendersOwnCommandsWithoutArguments_OK()
    {
        var parserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var commandArgument = parserService.AddCommandArgument("c", "command", ["test command"], true);
        AddCommandValues(commandArgument);

        parserService.ShowGroupUsageInformation("my.exe", commandArgument, "Identity Services", null);

        _stubIConsoleService.Received().WriteLine("IDENTITY SERVICES");
        _stubIConsoleService.Received().WriteColumnLine("  CreateUser:", Arg.Any<int>(), "Creates a user.");
        _stubIConsoleService.Received().WriteColumnLine("  DeleteUser:", Arg.Any<int>(), "Deletes a user.");
        // Other groups stay out, and the group view is compact — no argument lines.
        _stubIConsoleService.DidNotReceive().WriteLine("CONTEXT MANAGEMENT");
        _stubIConsoleService.DidNotReceive().WriteColumnLine(
            Arg.Is<string>(x => x.Contains("--name")), Arg.Any<int>(), Arg.Any<string>());
    }

    [Fact]
    public void ShowGroupUsageInformation_WithShadowedCommand_PointsAtTheCommand_OK()
    {
        var parserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var commandArgument = parserService.AddCommandArgument("c", "command", ["test command"], true);
        AddCommandValues(commandArgument);

        parserService.ShowGroupUsageInformation("my.exe", commandArgument, "Reporting", "Reporting");

        _stubIConsoleService.Received().WriteLineRegardSpace(
            "Note: 'Reporting' is also a command — run 'my.exe -c Reporting --help' for its help.");
    }

    /// <summary>
    ///     Mirrors <see cref="CreateCommands" /> on the parser side, with an argument on one command so the
    ///     compact rendering can be told apart from the full one.
    /// </summary>
    private static void AddCommandValues(ICommandArgument commandArgument)
    {
        var createUser = new CommandArgumentValue("Identity Services", "CreateUser", "Creates a user.");
        createUser.AddArgument("n", "name", ["Name of the user"], true, 1);
        commandArgument.AddCommandValue(createUser);
        commandArgument.AddCommandValue(
            new CommandArgumentValue("Identity Services", "DeleteUser", "Deletes a user."));
        commandArgument.AddCommandValue(
            new CommandArgumentValue("Context Management", "UseContext", "Switches the context."));
        commandArgument.AddCommandValue(new CommandArgumentValue("Reporting", "Reporting", "Runs the report."));
    }
}
