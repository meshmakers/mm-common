using Meshmakers.Common.CommandLineParser;
using Meshmakers.Common.Shared.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Meshmakers.Common.CommandLineParserTests;

[TestClass]
public class ParserServiceTests
{
    private readonly Mock<IConsoleService> _stubIConsoleService = new();
    private readonly Mock<IEnvironmentService> _stubIEnvironmentService = new();

    [TestMethod]
    [ExpectedException(typeof(InvalidParameterException))]
    public void ParseAndValidate_ArgumentNotFound_Fail()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        commandLineParserService.ParseAndValidate(new[] { "my.exe", "-a" });
    }

    [TestMethod]
    public void ParseAndValidate_ArgumentFound_OK()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var argumentDefinition = commandLineParserService.AddArgument("a", "longTerm", new[] { "test" });
        commandLineParserService.ParseAndValidate(new[] { "my.exe", "-a" });

        Assert.IsTrue(commandLineParserService.IsArgumentUsed(argumentDefinition));
    }

    [TestMethod]
    public void ParseAndValidate_ClearValue_OK()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var argumentADefinition = commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" });
        var argumentBDefinition = commandLineParserService.AddArgument("b", "bLongTerm", new[] { "test" });
        commandLineParserService.ParseAndValidate(new[] { "my.exe", "-a" });
        commandLineParserService.ParseAndValidate(new[] { "my.exe", "-b" });

        Assert.IsFalse(commandLineParserService.IsArgumentUsed(argumentADefinition));
        Assert.IsTrue(commandLineParserService.IsArgumentUsed(argumentBDefinition));
    }

    [TestMethod]
    public void ParseAndValidate_EmptyString_OK()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var argumentADefinition = commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" });
        commandLineParserService.ParseAndValidate(new[] { "my.exe", "", "-a" });

        Assert.IsTrue(commandLineParserService.IsArgumentUsed(argumentADefinition));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentValueMissingException))]
    public void ParseAndValidate_ArgumentValues_Mandatory_Missing_Fail()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" }, true, 2, false);
        commandLineParserService.ParseAndValidate(new[] { "my.exe", "-a", "value1" });
    }

    [TestMethod]
    public void ParseAndValidate_ArgumentValues_Mandatory_Match_OK()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var argDef = commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" }, true, 2, false);
        commandLineParserService.ParseAndValidate(new[] { "my.exe", "-a", "value1", "value2" });

        Assert.AreEqual("value1", commandLineParserService.GetArgumentValue(argDef).GetValue<string>());
        Assert.AreEqual("value2", commandLineParserService.GetArgumentValue(argDef).GetValue<string>(1));
        Assert.AreEqual(2, commandLineParserService.GetArgumentValue(argDef).Values.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidParameterException))]
    public void ParseAndValidate_ArgumentValues_Mandatory_MoreThanDefined_Fail()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" }, true, 2, false);
        commandLineParserService.ParseAndValidate(new[] { "my.exe", "-a", "value1", "value2", "value3" });
    }

    [TestMethod]
    public void ParseAndValidate_ArgumentValues_Optional_OK()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var argDef = commandLineParserService.AddArgument("a", "aLongTerm", new[] { "test" }, true, 1, true);
        commandLineParserService.ParseAndValidate(new[] { "my.exe", "-a", "value1", "value2", "value3" });

        Assert.AreEqual("value1", commandLineParserService.GetArgumentValue(argDef).GetValue<string>());
        Assert.AreEqual("value2", commandLineParserService.GetArgumentValue(argDef).GetValue<string>(1));
        Assert.AreEqual("value3", commandLineParserService.GetArgumentValue(argDef).GetValue<string>(2));
        Assert.AreEqual(3, commandLineParserService.GetArgumentValue(argDef).Values.Count);
    }

    [TestMethod]
    public void ParseAndValidate_CommandArgument_FilePath_OK()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, false);
        var firstCommandArgumentValue = commandArgument.AddCommandValue("ImportCk", "first command");

        var argumentA = firstCommandArgumentValue.AddArgument("f", "file", new[] { "test a" }, true, 1, false);
        commandLineParserService.ParseAndValidate(new[]
        {
            "my.exe", "-c", "ImportCk", "-f",
            "/Users/gerald/RiderProjects/PaketService/Backend/Persistence/PaketServiceConstructionKit.json"
        });

        Assert.AreEqual("ImportCk", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.AreEqual("/Users/gerald/RiderProjects/PaketService/Backend/Persistence/PaketServiceConstructionKit.json",
            firstCommandArgumentValue.GetArgumentValue(argumentA).GetValue<string>());
    }


    [TestMethod]
    [ExpectedException(typeof(MandatoryArgumentsMissingException))]
    public void ParseAndValidate_CommandArgument_MandatoryArgMissing_Fail()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, true);
        var firstCommandArgumentValue = commandArgument.AddCommandValue("first", "first command");
        var secondCommandArgumentValue = commandArgument.AddCommandValue("second", "second command");

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", new[] { "test a" }, true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", new[] { "test b" }, true, 1, true);
        commandLineParserService.ParseAndValidate(new[] { "my.exe" });

        Assert.AreEqual("first", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidParameterException))]
    public void ParseAndValidate_CommandArgument_MandatoryArgMissing_OtherArguments_Fail()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, true);
        var firstCommandArgumentValue = commandArgument.AddCommandValue("first", "first command");
        var secondCommandArgumentValue = commandArgument.AddCommandValue("second", "second command");

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", new[] { "test a" }, true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", new[] { "test b" }, true, 1, true);
        commandLineParserService.ParseAndValidate(new[] { "my.exe", "-a", "value1", "value2", "value3" });

        Assert.AreEqual("first", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
    }


    [TestMethod]
    public void ParseAndValidate_CommandArgument_OtherArguments_OK()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var otherParameterArgument =
            commandLineParserService.AddArgument("o", "Other", new[] { "Other parameter" }, false);

        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, false);
        var firstCommandArgumentValue = commandArgument.AddCommandValue("first", "first command");
        var secondCommandArgumentValue = commandArgument.AddCommandValue("second", "second command");

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", new[] { "test a" }, true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", new[] { "test b" }, true, 1, true);
        commandLineParserService.ParseAndValidate(new[]
            { "my.exe", "-c", "first", "-a", "value1", "value2", "value3", "-o" });

        Assert.AreEqual("first", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.IsTrue(commandLineParserService.IsArgumentUsed(otherParameterArgument));
    }

    [TestMethod]
    public void ParseAndValidate_CommandArgument_OK()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var otherParameterArgument =
            commandLineParserService.AddArgument("o", "Other", new[] { "Other parameter" }, false);

        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, false);
        var firstCommandArgumentValue = commandArgument.AddCommandValue("first", "first command");
        var secondCommandArgumentValue = commandArgument.AddCommandValue("second", "second command");

        firstCommandArgumentValue.AddArgument("a", "aLongTerm", new[] { "test a" }, true, 1, true);
        secondCommandArgumentValue.AddArgument("b", "bLongTerm", new[] { "test b" }, true, 1, true);
        commandLineParserService.ParseAndValidate(new[]
            { "my.exe", "-c", "first", "-a", "value1", "value2", "value3", "-o" });

        Assert.AreEqual("first", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.IsTrue(commandLineParserService.IsArgumentUsed(otherParameterArgument));
    }

    [TestMethod]
    public void ParseAndValidate_Arguments_StartsWithSameCharacters_OK()
    {
        var commandLineParserService = new ParserService(_stubIEnvironmentService.Object, _stubIConsoleService.Object);
        var otherParameterArgument =
            commandLineParserService.AddArgument("s", "simulation", new[] { "Other parameter" }, false);

        var commandArgument =
            commandLineParserService.AddCommandArgument("c", "command", new[] { "test command" }, false);
        var firstCommandArgumentValue = commandArgument.AddCommandValue("first", "first command");

        var argumentA = firstCommandArgumentValue.AddArgument("st", "status", new[] { "test a" }, 0);
        var argumentB = firstCommandArgumentValue.AddArgument("b", "bLongTerm", new[] { "test b" }, 0);
        commandLineParserService.ParseAndValidate(new[] { "my.exe", "-c", "first", "-b", "-s" });

        Assert.AreEqual("first", commandLineParserService.GetArgumentValue(commandArgument).GetValue<string>());
        Assert.IsFalse(firstCommandArgumentValue.IsArgumentUsed(argumentA));
        Assert.IsTrue(firstCommandArgumentValue.IsArgumentUsed(argumentB));
        Assert.IsTrue(commandLineParserService.IsArgumentUsed(otherParameterArgument));
    }
}