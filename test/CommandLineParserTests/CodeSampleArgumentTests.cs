using System;
using Meshmakers.Common.CommandLineParser;
using Xunit;

namespace Meshmakers.Common.CommandLineParserTests;

public class CodeSampleArgumentTests
{
    private static Argument ValueArgument() =>
        new("t", "target", ["target id"], isMandatoryArgument: true, mandatoryValuesCount: 1, areOptionalValuesAllowed: false);

    private static Argument FlagArgument() =>
        new("w", "wait", ["wait flag"], isMandatoryArgument: false, mandatoryValuesCount: 0, areOptionalValuesAllowed: false);

    [Fact]
    public void Value_Ctor_StoresArgumentAndValue()
    {
        var arg = ValueArgument();

        var binding = new CodeSampleArgument(arg, "bar");

        Assert.Same(arg, binding.Argument);
        Assert.Equal("bar", binding.Value);
    }

    [Fact]
    public void Flag_Ctor_StoresArgumentAndNullValue()
    {
        var arg = FlagArgument();

        var binding = new CodeSampleArgument(arg);

        Assert.Same(arg, binding.Argument);
        Assert.Null(binding.Value);
    }

    [Fact]
    public void Value_Ctor_Rejects_FlagOnlyArgument()
    {
        var flag = FlagArgument();

        var ex = Assert.Throws<ArgumentException>(() => new CodeSampleArgument(flag, "ignored"));
        Assert.Contains("--wait", ex.Message);
        Assert.Contains("flag", ex.Message);
    }

    [Fact]
    public void Flag_Ctor_Rejects_ArgumentRequiringValue()
    {
        var arg = ValueArgument();

        var ex = Assert.Throws<ArgumentException>(() => new CodeSampleArgument(arg));
        Assert.Contains("--target", ex.Message);
        Assert.Contains("requires a value", ex.Message);
    }

    [Fact]
    public void Value_Ctor_Rejects_EmptyValue()
    {
        var arg = ValueArgument();

        // ArgumentValidation.ValidateString throws on empty string.
        Assert.ThrowsAny<ArgumentException>(() => new CodeSampleArgument(arg, ""));
    }
}
