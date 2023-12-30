using Meshmakers.Common.Shared;
using Xunit;

namespace Meshmakers.Common.SharedTests;

public class ArgumentValidationTests
{
    private const string TestArgument = "test";

    [Fact]
    public void ValidateTyped_OK()
    {
        ArgumentValidation.Validate<int>(TestArgument, 5);
    }

    [Fact]
    public void ValidateTyped_Fail()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ArgumentValidation.Validate<int>(TestArgument, "string"));
    }
}
