using System;
using System.Collections.Generic;
using System.Linq;
using Meshmakers.Common.CommandLineParser;
using Moq;
using Xunit;

namespace Meshmakers.Common.CommandLineParserTests;

public class ArgumentValueTests
{
    [Fact]
    public void Creation_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();

        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        Assert.Equal(stubArgDefinition.Object, commandLineValue.Argument);
    }

    [Fact]
    public void AddValue_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue");

        Assert.Single(commandLineValue.Values);
        Assert.Equal("testUnitTestValue", commandLineValue.Values.First());
    }

    [Fact]
    public void GetValue_NoValue_Fail()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        Assert.Throws<KeyNotFoundException>(() => commandLineValue.GetValue<string>());
    }

    [Fact]
    public void GetValue_ValueExisting_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue");

        var result = commandLineValue.GetValue<string>();

        Assert.Equal("testUnitTestValue", result);
    }

    [Fact]
    public void GetValue_WrongCast_Fail()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue");

        Assert.Throws<FormatException>(() => commandLineValue.GetValue<bool>());
    }

    [Fact]
    public void GetValue_DefaultValue_NoValue_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        var result = commandLineValue.GetValue("myDefaultValue");
        Assert.Equal("myDefaultValue", result);
    }

    [Fact]
    public void GetValue_DefaultValue_ValueExisting_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue");

        var result = commandLineValue.GetValue("myDefaultValue");

        Assert.Equal("testUnitTestValue", result);
    }

    [Fact]
    public void GetValue_DefaultValue_WrongCast_Fail()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue");

        Assert.Throws<FormatException>(() => commandLineValue.GetValue(true));
    }

    [Fact]
    public void GetValue_DefaultValue_Index_OutOfRange_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        var result = commandLineValue.GetValue(3, "myDefaultValue");
        Assert.Equal("myDefaultValue", result);
    }

    [Fact]
    public void GetValue_DefaultValue_Index_ValueExisting_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue1");
        commandLineValue.AddValue("testUnitTestValue2");
        commandLineValue.AddValue("testUnitTestValue3");
        commandLineValue.AddValue("testUnitTestValue4");

        var result = commandLineValue.GetValue(2, "myDefaultValue");

        Assert.Equal("testUnitTestValue3", result);
    }

    [Fact]
    public void GetValue_DefaultValue_Index_WrongCast_Fail()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue1");
        commandLineValue.AddValue("testUnitTestValue2");
        commandLineValue.AddValue("testUnitTestValue3");
        commandLineValue.AddValue("testUnitTestValue4");

        Assert.Throws<FormatException>(() => commandLineValue.GetValue(1, true));
    }
}