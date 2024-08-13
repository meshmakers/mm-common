using Meshmakers.Common.Shared;
using Xunit;

namespace Meshmakers.Common.SharedTests;

public class StringExtensionTests
{
    [Fact]
    public void NormalizeString_Ok()
    {
        string test = "aBc";
        var result = test.NormalizeString();

        Assert.Equal("abc", result);
    }
}
