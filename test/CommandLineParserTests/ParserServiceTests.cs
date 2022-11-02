using Meshmakers.Common.CommandLineParser;
using Meshmakers.Common.Shared.Services;
using Moq;
using Xunit;

namespace Meshmakers.Common.CommandLineParserTests;

public class ParserServiceTests
{
    private readonly Mock<IConsoleService> _stubIConsoleService = new();
    private readonly Mock<IEnvironmentService> _stubIEnvironmentService = new();

    [Fact]
    public void ParseAndValidate_ArgumentNotFound_Fail()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe", "-a" });
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        Assert.Throws<InvalidParameterException>(() =>
            commandLineParserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_ArgumentFound_OK()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe", "-a" });
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var argumentDefinition = commandLineParserService.AddArgument("a", "longTerm", new[] { "test" });
        commandLineParserService.ParseAndValidate();

        Assert.True(commandLineParserService.IsArgumentUsed(argumentDefinition));
    }

    [Fact]
    public void ParseAndValidate_ClearValue_OK()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe", "-a" });
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe", "-b" });
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var argumentADefinition = commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" });
        var argumentBDefinition = commandLineParserService.AddArgument("b", "bLongTerm", new[] { "test" });
        commandLineParserService.ParseAndValidate();
        commandLineParserService.ParseAndValidate();

        Assert.False(commandLineParserService.IsArgumentUsed(argumentADefinition));
        Assert.True(commandLineParserService.IsArgumentUsed(argumentBDefinition));
    }

    [Fact]
    public void ParseAndValidate_EmptyString_OK()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe", "", "-a" });
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var argumentADefinition = commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" });
        commandLineParserService.ParseAndValidate();

        Assert.True(commandLineParserService.IsArgumentUsed(argumentADefinition));
    }

    [Fact]
    public void ParseAndValidate_ArgumentValues_Mandatory_Missing_Fail()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe", "-a", "value1"});
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" }, true, 2, false);
        Assert.Throws<ArgumentValueMissingException>(() =>
            commandLineParserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_ArgumentValues_Mandatory_Match_OK()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe", "-a", "value1", "value2" });
        
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var argDef = commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" }, true, 2, false);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("value1", commandLineParserService.GetArgumentValue(argDef).GetValue<string>());
        Assert.Equal("value2", commandLineParserService.GetArgumentValue(argDef).GetValue<string>(1));
        Assert.Equal(2, commandLineParserService.GetArgumentValue(argDef).Values.Count);
    }

    [Fact]
    public void ParseAndValidate_ArgumentValues_Mandatory_MoreThanDefined_Fail()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe", "-a", "value1", "value2", "value3" });
        
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" }, true, 2, false);
        Assert.Throws<InvalidParameterException>(() =>
            commandLineParserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_ArgumentValues_Optional_OK()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe", "-a", "value1", "value2", "value3" });
        
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var argDef = commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" }, true, 1, true);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("value1", commandLineParserService.GetArgumentValue(argDef).GetValue<string>());
        Assert.Equal("value2", commandLineParserService.GetArgumentValue(argDef).GetValue<string>(1));
        Assert.Equal("value3", commandLineParserService.GetArgumentValue(argDef).GetValue<string>(2));
        Assert.Equal(3, commandLineParserService.GetArgumentValue(argDef).Values.Count);
    }


    [Fact]
    public void ParseAndValidate_CommandArgument_MandatoryArgMissing_Fail()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe" });
        
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, true);
        var firstCommandArgumentValue = new CommandArgumentValue("first", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);
        var secondCommandArgumentValue = new CommandArgumentValue("second", "second command");
        commandArgument.AddCommandValue(secondCommandArgumentValue);

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", new[] { "test a" }, true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", new[] { "test b" }, true, 1, true);
        Assert.Throws<MandatoryArgumentsMissingException>(() =>
            commandLineParserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_CommandArgument_MandatoryArgMissing_OtherArguments_Fail()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe", "-a", "value1", "value2", "value3" });
        
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, true);
        var firstCommandArgumentValue = new CommandArgumentValue("first", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);
        var secondCommandArgumentValue = new CommandArgumentValue("second", "second command");
        commandArgument.AddCommandValue(secondCommandArgumentValue);

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", new[] { "test a" }, true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", new[] { "test b" }, true, 1, true);

        Assert.Throws<InvalidParameterException>(() =>
            commandLineParserService.ParseAndValidate());
    }

    [Fact]
    public void ParseAndValidate_CommandArgument_FilePath_OK()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[]
            {
                "my.exe", "-c", "ImportCk", "-f",
                "/Users/gerald/RiderProjects/PaketService/Backend/Persistence/PaketServiceConstructionKit.json"
            });
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, false);
        var firstCommandArgumentValue = new CommandArgumentValue("ImportCk", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);

        var argumentA = firstCommandArgumentValue.AddArgument("f", "file", new[] { "test a" }, true, 1, false);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("ImportCk", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.Equal("/Users/gerald/RiderProjects/PaketService/Backend/Persistence/PaketServiceConstructionKit.json",
            firstCommandArgumentValue.GetArgumentValue(argumentA).GetValue<string>());
    }

    [Fact]
    public void ParseAndValidate_CommandArgument_OtherArguments_OK()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[]
                { "my.exe", "-c", "first", "-a", "value1", "value2", "value3", "-o" });
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var otherParameterArgument =
            commandLineParserService.AddArgument("o", "Other", new[] { "Other parameter" }, false);

        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, false);
        var firstCommandArgumentValue = new CommandArgumentValue("first", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);

        var secondCommandArgumentValue = new CommandArgumentValue("second", "second command");
        commandArgument.AddCommandValue(secondCommandArgumentValue);

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", new[] { "test a" }, true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", new[] { "test b" }, true, 1, true);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("first", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.True(commandLineParserService.IsArgumentUsed(otherParameterArgument));
    }

    [Fact]
    public void ParseAndValidate_CommandArgument_OK()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[]
                { "my.exe", "-c", "first", "-a", "value1", "value2", "value3", "-o" });
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var otherParameterArgument =
            commandLineParserService.AddArgument("o", "Other", new[] { "Other parameter" }, false);

        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, false);
        var firstCommandArgumentValue = new CommandArgumentValue("first", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);

        var secondCommandArgumentValue = new CommandArgumentValue("second", "second command");
        commandArgument.AddCommandValue(secondCommandArgumentValue);

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", new[] { "test a" }, true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", new[] { "test b" }, true, 1, true);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("first", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.True(commandLineParserService.IsArgumentUsed(otherParameterArgument));
    }

    [Fact]
    public void ParseAndValidate_Arguments_StartsWithSameCharacters_OK()
    {
        _stubIEnvironmentService.Setup(x => x.GetCommandLineArgs()).Returns(
            () => new[] { "my.exe", "-c", "first", "-b", "-s" });
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var otherParameterArgument =
            commandLineParserService.AddArgument("s", "simulation", new[] { "Other parameter" }, false);

        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, false);
        var firstCommandArgumentValue = new CommandArgumentValue("first", "first command");
        commandArgument.AddCommandValue(firstCommandArgumentValue);

        var argumentA = firstCommandArgumentValue.AddArgument("st", "status", new[] { "test a" }, 0);
        var argumentB = firstCommandArgumentValue.AddArgument("b", "bLongTerm", new[] { "test b" }, 0);
        commandLineParserService.ParseAndValidate();

        Assert.Equal("first", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.False(firstCommandArgumentValue.IsArgumentUsed(argumentA));
        Assert.True(firstCommandArgumentValue.IsArgumentUsed(argumentB));
        Assert.True(commandLineParserService.IsArgumentUsed(otherParameterArgument));
    }
}