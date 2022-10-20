using Meshmakers.Common.CommandLineParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meshmakers.Common.CommandLineParserTests;

[TestClass]
public class ArgumentTests
{
    [TestMethod]
    public void Creation_OK()
    {
        var argDefinition = new Argument("a", "longterm", new[] { "description" }, true, 67, true);
        Assert.AreEqual("a", argDefinition.ShortTerm);
        Assert.AreEqual("longterm", argDefinition.LongTerm);
        Assert.AreEqual(67, argDefinition.MandatoryValuesCount);
        Assert.AreEqual(true, argDefinition.AreOptionalValuesAllowed);
        Assert.AreEqual(1, argDefinition.Description.Length);
        Assert.AreEqual("description", argDefinition.Description[0]);
    }

    [TestMethod]
    public void Compare_UnknownSign_OK()
    {
        var argDefinition = new Argument("a", "longterm", new[] { "description" }, true, 0, true);
        Assert.IsFalse(argDefinition.Compare("%a"));
    }

    [TestMethod]
    public void Compare_ShortTerm_Match_OK()
    {
        var argDefinition = new Argument("a", "longterm", new[] { "description" }, true, 0, true);
        Assert.IsTrue(argDefinition.Compare("-a"));
    }

    [TestMethod]
    public void Compare_ShortTerm_DoesNotMatch_OK()
    {
        var argDefinition = new Argument("a", "longterm", new[] { "description" }, true, 0, true);
        Assert.IsFalse(argDefinition.Compare("-b"));
    }

    [TestMethod]
    public void Compare_LongTerm_Match_OK()
    {
        var argDefinition = new Argument("a", "longterm", new[] { "description" }, true, 0, true);
        Assert.IsTrue(argDefinition.Compare("--longterm"));
    }

    [TestMethod]
    public void Compare_LongTerm_DoesNotMatch_OK()
    {
        var argDefinition = new Argument("a", "longterm", new[] { "description" }, true, 0, true);
        Assert.IsFalse(argDefinition.Compare("--unknown"));
    }


    [TestMethod]
    public void Compare_LongTerm_Slash_Match_OK()
    {
        var argDefinition = new Argument("a", "longterm", new[] { "description" }, true, 0, true);
        Assert.IsTrue(argDefinition.Compare("/longterm"));
    }

    [TestMethod]
    public void Compare_LongTerm_Slash_DoesNotMatch_OK()
    {
        var argDefinition = new Argument("a", "longterm", new[] { "description" }, true, 0, true);
        Assert.IsFalse(argDefinition.Compare("/unknown"));
    }
}