using Meshmakers.Common.CommandLineParser;
using Xunit;

namespace Meshmakers.Common.CommandLineParserTests;

public class ArgumentTests
{
    [Fact]
    public void Creation_OK()
    {
        var argDefinition = new Argument("a", "longterm", ["description"], true, 67, true);
        Assert.Equal("a", argDefinition.ShortTerm);
        Assert.Equal("longterm", argDefinition.LongTerm);
        Assert.Equal(67, argDefinition.MandatoryValuesCount);
        Assert.True(argDefinition.AreOptionalValuesAllowed);
        Assert.Single(argDefinition.Description);
        Assert.Equal("description", argDefinition.Description[0]);
    }

    [Fact]
    public void Compare_UnknownSign_OK()
    {
        var argDefinition = new Argument("a", "longterm", ["description"], true, 0, true);
        Assert.False(argDefinition.Compare("%a"));
    }

    [Fact]
    public void Compare_ShortTerm_Match_OK()
    {
        var argDefinition = new Argument("a", "longterm", ["description"], true, 0, true);
        Assert.True(argDefinition.Compare("-a"));
    }

    [Fact]
    public void Compare_ShortTerm_DoesNotMatch_OK()
    {
        var argDefinition = new Argument("a", "longterm", ["description"], true, 0, true);
        Assert.False(argDefinition.Compare("-b"));
    }

    [Fact]
    public void Compare_LongTerm_Match_OK()
    {
        var argDefinition = new Argument("a", "longterm", ["description"], true, 0, true);
        Assert.True(argDefinition.Compare("--longterm"));
    }

    [Fact]
    public void Compare_LongTerm_DoesNotMatch_OK()
    {
        var argDefinition = new Argument("a", "longterm", ["description"], true, 0, true);
        Assert.False(argDefinition.Compare("--unknown"));
    }


    [Fact]
    public void Compare_LongTerm_Slash_Match_OK()
    {
        var argDefinition = new Argument("a", "longterm", ["description"], true, 0, true);
        Assert.True(argDefinition.Compare("/longterm"));
    }

    [Fact]
    public void Compare_LongTerm_Slash_DoesNotMatch_OK()
    {
        var argDefinition = new Argument("a", "longterm", ["description"], true, 0, true);
        Assert.False(argDefinition.Compare("/unknown"));
    }
}
