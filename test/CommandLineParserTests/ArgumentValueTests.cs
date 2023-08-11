using System;
using System.Collections.Generic;
using System.Linq;
using Meshmakers.Common.CommandLineParser;
using NSubstitute;
using Xunit;

namespace Meshmakers.Common.CommandLineParserTests;

public class ArgumentValueTests
{
    [Fact]
    public void Creation_OK()
    {

        var stubArgDefinition = Substitute.For<IArgument>();

        var commandLineValue = new ArgumentValue(stubArgDefinition);

        Assert.Equal(stubArgDefinition, commandLineValue.Argument);
    }

    [Fact]
    public void AddValue_OK()
    {
        var stubArgDefinition = Substitute.For<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition);

        commandLineValue.AddValue("testUnitTestValue");

        Assert.Single(commandLineValue.Values);
        Assert.Equal("testUnitTestValue", commandLineValue.Values.First());
    }

    [Fact]
    public void GetValue_NoValue_Fail()
    {
        var stubArgDefinition = Substitute.For<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition);

        Assert.Throws<KeyNotFoundException>(() => commandLineValue.GetValue<string>());
    }

    [Fact]
    public void GetValue_ValueExisting_OK()
    {
        var stubArgDefinition = Substitute.For<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition);

        commandLineValue.AddValue("testUnitTestValue");

        var result = commandLineValue.GetValue<string>();

        Assert.Equal("testUnitTestValue", result);
    }

    [Fact]
    public void GetValue_WrongCast_Fail()
    {
        var stubArgDefinition = Substitute.For<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition);

        commandLineValue.AddValue("testUnitTestValue");

        Assert.Throws<FormatException>(() => commandLineValue.GetValue<bool>());
    }

    [Fact]
    public void GetValue_DefaultValue_NoValue_OK()
    {
        var stubArgDefinition = Substitute.For<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition);

        var result = commandLineValue.GetValue("myDefaultValue");
        Assert.Equal("myDefaultValue", result);
    }

    [Fact]
    public void GetValue_DefaultValue_ValueExisting_OK()
    {
        var stubArgDefinition = Substitute.For<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition);

        commandLineValue.AddValue("testUnitTestValue");

        var result = commandLineValue.GetValue("myDefaultValue");

        Assert.Equal("testUnitTestValue", result);
    }

    [Fact]
    public void GetValue_DefaultValue_WrongCast_Fail()
    {
        var stubArgDefinition = Substitute.For<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition);

        commandLineValue.AddValue("testUnitTestValue");

        Assert.Throws<FormatException>(() => commandLineValue.GetValue(true));
    }

    [Fact]
    public void GetValue_DefaultValue_Index_OutOfRange_OK()
    {
        var stubArgDefinition = Substitute.For<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition);

        var result = commandLineValue.GetValue(3, "myDefaultValue");
        Assert.Equal("myDefaultValue", result);
    }

    [Fact]
    public void GetValue_DefaultValue_Index_ValueExisting_OK()
    {
        var stubArgDefinition = Substitute.For<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition);

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
        var stubArgDefinition = Substitute.For<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition);

        commandLineValue.AddValue("testUnitTestValue1");
        commandLineValue.AddValue("testUnitTestValue2");
        commandLineValue.AddValue("testUnitTestValue3");
        commandLineValue.AddValue("testUnitTestValue4");

        Assert.Throws<FormatException>(() => commandLineValue.GetValue(1, true));
    }
}
