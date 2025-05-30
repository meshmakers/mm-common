using Meshmakers.Common.CommandLineParser;
using Meshmakers.Common.Shared.Services;
using NSubstitute;
using Xunit;

namespace Meshmakers.Common.CommandLineParserTests;

public class ParserServiceTests
{
    private readonly IConsoleService _stubIConsoleService = Substitute.For<IConsoleService>();
    private readonly IEnvironmentService _stubIEnvironmentService = Substitute.For<IEnvironmentService>();

    [Fact]
    public void ParseAndValidate_ArgumentNotFound_Fail()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(
            ["my.exe", "-a"]);
        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        Assert.Throws<InvalidParameterException>(() =>
            commandLineParserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_ArgumentFound_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-a"]);
        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var argumentDefinition = commandLineParserService.AddArgument("a", "longTerm", ["test"]);
        commandLineParserService.ParseAndValidate();

        Assert.True(commandLineParserService.IsArgumentUsed(argumentDefinition));
    }

    [Fact]
    public void ParseAndValidate_ClearValue_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-a"]);
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-b"]);
        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var argumentADefinition = commandLineParserService.AddArgument("a", "aLongTerm", ["test"]);
        var argumentBDefinition = commandLineParserService.AddArgument("b", "bLongTerm", ["test"]);
        commandLineParserService.ParseAndValidate();
        commandLineParserService.ParseAndValidate();

        Assert.False(commandLineParserService.IsArgumentUsed(argumentADefinition));
        Assert.True(commandLineParserService.IsArgumentUsed(argumentBDefinition));
    }

    [Fact]
    public void ParseAndValidate_EmptyString_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-a"]);
        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var argumentADefinition = commandLineParserService.AddArgument("a", "aLongTerm", ["test"]);
        commandLineParserService.ParseAndValidate();

        Assert.True(commandLineParserService.IsArgumentUsed(argumentADefinition));
    }

    [Fact]
    public void ParseAndValidate_ArgumentValues_Mandatory_Missing_Fail()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-a", "value1"]);
        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        commandLineParserService.AddArgument("a", "aLongTerm", ["test"], true, 2, false);
        Assert.Throws<ArgumentValueMissingException>(() =>
            commandLineParserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_ArgumentValues_Mandatory_Match_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-a", "value1", "value2"]);

        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var argDef = commandLineParserService.AddArgument("a", "aLongTerm", ["test"], true, 2, false);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("value1", commandLineParserService.GetArgumentValue(argDef).GetValue<string>());
        Assert.Equal("value2", commandLineParserService.GetArgumentValue(argDef).GetValue<string>(1));
        Assert.Equal(2, commandLineParserService.GetArgumentValue(argDef).Values.Count);
    }

    [Fact]
    public void ParseAndValidate_ArgumentValues_Mandatory_MoreThanDefined_Fail()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-a", "value1", "value2", "value3"]);

        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        commandLineParserService.AddArgument("a", "aLongTerm", ["test"], true, 2, false);
        Assert.Throws<InvalidParameterException>(() =>
            commandLineParserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_ArgumentValues_Optional_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-a", "value1", "value2", "value3"]);

        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var argDef = commandLineParserService.AddArgument("a", "aLongTerm", ["test"], true, 1, true);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("value1", commandLineParserService.GetArgumentValue(argDef).GetValue<string>());
        Assert.Equal("value2", commandLineParserService.GetArgumentValue(argDef).GetValue<string>(1));
        Assert.Equal("value3", commandLineParserService.GetArgumentValue(argDef).GetValue<string>(2));
        Assert.Equal(3, commandLineParserService.GetArgumentValue(argDef).Values.Count);
    }


    [Fact]
    public void ParseAndValidate_CommandArgument_MandatoryArgMissing_Fail()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe"]);

        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", ["test command"], true);
        var firstCommandArgumentValue = new CommandArgumentValue("g", "first", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);
        var secondCommandArgumentValue = new CommandArgumentValue("g", "second", "second command");
        commandArgument.AddCommandValue(secondCommandArgumentValue);

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", ["test a"], true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", ["test b"], true, 1, true);
        Assert.Throws<MandatoryArgumentsMissingException>(() =>
            commandLineParserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_CommandArgument_MandatoryArgMissing_OtherArguments_Fail()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-a", "value1", "value2", "value3"]);

        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", ["test command"], true);
        var firstCommandArgumentValue = new CommandArgumentValue("g", "first", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);
        var secondCommandArgumentValue = new CommandArgumentValue("g", "second", "second command");
        commandArgument.AddCommandValue(secondCommandArgumentValue);

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", ["test a"], true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", ["test b"], true, 1, true);

        Assert.Throws<InvalidParameterException>(() =>
            commandLineParserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_CommandArgument_FilePath_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns([
            "my.exe", "-c", "ImportCk", "-f",
            "/Users/gerald/RiderProjects/PaketService/Backend/Persistence/PaketServiceConstructionKit.json"
        ]);

        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", ["test command"], false);
        var firstCommandArgumentValue = new CommandArgumentValue("g", "ImportCk", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);

        var argumentA = firstCommandArgumentValue.AddArgument("f", "file", ["test a"], true, 1, false);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("ImportCk", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.Equal("/Users/gerald/RiderProjects/PaketService/Backend/Persistence/PaketServiceConstructionKit.json",
            firstCommandArgumentValue.GetArgumentValue(argumentA).GetValue<string>());
    }

    [Fact]
    public void ParseAndValidate_CommandArgument_OtherArguments_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-c", "first", "-a", "value1", "value2", "value3", "-o"]);

        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var otherParameterArgument =
            commandLineParserService.AddArgument("o", "Other", ["Other parameter"], false);

        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", ["test command"], false);
        var firstCommandArgumentValue = new CommandArgumentValue("g", "first", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);

        var secondCommandArgumentValue = new CommandArgumentValue("g", "second", "second command");
        commandArgument.AddCommandValue(secondCommandArgumentValue);

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", ["test a"], true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", ["test b"], true, 1, true);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("first", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.True(commandLineParserService.IsArgumentUsed(otherParameterArgument));
    }

    [Fact]
    public void ParseAndValidate_CommandArgument_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-c", "first", "-a", "value1", "value2", "value3", "-o"]);

        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var otherParameterArgument =
            commandLineParserService.AddArgument("o", "Other", ["Other parameter"], false);

        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", ["test command"], false);
        var firstCommandArgumentValue = new CommandArgumentValue("g", "first", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);

        var secondCommandArgumentValue = new CommandArgumentValue("g", "second", "second command");
        commandArgument.AddCommandValue(secondCommandArgumentValue);

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", ["test a"], true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", ["test b"], true, 1, true);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("first", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.True(commandLineParserService.IsArgumentUsed(otherParameterArgument));
    }

    [Fact]
    public void ParseAndValidate_Arguments_StartsWithSameCharacters_OK()
    {
        _stubIEnvironmentService.GetCommandLineArgs().Returns(["my.exe", "-c", "first", "-b", "-s"]);

        var commandLineParserService = new ParserService(_stubIEnvironmentService, _stubIConsoleService);
        var otherParameterArgument =
            commandLineParserService.AddArgument("s", "simulation", ["Other parameter"], false);

        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", ["test command"], false);
        var firstCommandArgumentValue = new CommandArgumentValue("g", "first", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);

        var argumentA = firstCommandArgumentValue.AddArgument("st", "status", ["test a"], 0);
        var argumentB = firstCommandArgumentValue.AddArgument("b", "bLongTerm", ["test b"], 0);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("first", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.False(firstCommandArgumentValue.IsArgumentUsed(argumentA));
        Assert.True(firstCommandArgumentValue.IsArgumentUsed(argumentB));
        Assert.True(commandLineParserService.IsArgumentUsed(otherParameterArgument));
    }
}
