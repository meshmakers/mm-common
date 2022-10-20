using System;
using System.Collections.Generic;
using System.Linq;
using Meshmakers.Common.CommandLineParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Meshmakers.Common.CommandLineParserTests;

[TestClass]
public class ArgumentValueTests
{
    [TestMethod]
    public void Creation_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();

        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        Assert.AreEqual(stubArgDefinition.Object, commandLineValue.Argument);
    }

    [TestMethod]
    public void AddValue_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue");

        Assert.AreEqual(1, commandLineValue.Values.Count);
        Assert.AreEqual("testUnitTestValue", commandLineValue.Values.First());
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void GetValue_NoValue_Fail()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.GetValue<string>();
    }

    [TestMethod]
    public void GetValue_ValueExisting_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue");

        var result = commandLineValue.GetValue<string>();

        Assert.AreEqual("testUnitTestValue", result);
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void GetValue_WrongCast_Fail()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue");

        commandLineValue.GetValue<bool>();
    }

    [TestMethod]
    public void GetValue_DefaultValue_NoValue_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        var result = commandLineValue.GetValue("myDefaultValue");
        Assert.AreEqual("myDefaultValue", result);
    }

    [TestMethod]
    public void GetValue_DefaultValue_ValueExisting_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue");

        var result = commandLineValue.GetValue("myDefaultValue");

        Assert.AreEqual("testUnitTestValue", result);
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void GetValue_DefaultValue_WrongCast_Fail()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue");

        commandLineValue.GetValue(true);
    }

    [TestMethod]
    public void GetValue_DefaultValue_Index_OutOfRange_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        var result = commandLineValue.GetValue(3, "myDefaultValue");
        Assert.AreEqual("myDefaultValue", result);
    }

    [TestMethod]
    public void GetValue_DefaultValue_Index_ValueExisting_OK()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue1");
        commandLineValue.AddValue("testUnitTestValue2");
        commandLineValue.AddValue("testUnitTestValue3");
        commandLineValue.AddValue("testUnitTestValue4");

        var result = commandLineValue.GetValue(2, "myDefaultValue");

        Assert.AreEqual("testUnitTestValue3", result);
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void GetValue_DefaultValue_Index_WrongCast_Fail()
    {
        var stubArgDefinition = new Mock<IArgument>();
        var commandLineValue = new ArgumentValue(stubArgDefinition.Object);

        commandLineValue.AddValue("testUnitTestValue1");
        commandLineValue.AddValue("testUnitTestValue2");
        commandLineValue.AddValue("testUnitTestValue3");
        commandLineValue.AddValue("testUnitTestValue4");

        commandLineValue.GetValue(1, true);
    }
}