using Meshmakers.Common.Shared;
using Xunit;

namespace Meshmakers.Common.SharedTests;

public class RandomGeneratorTests
{
    [Fact]
    public void GenerateUniqueString_OK()
    {
        var test1 = RandomGenerator.GenerateUniqueString();
        var test2 = RandomGenerator.GenerateUniqueString();
        var test3 = RandomGenerator.GenerateUniqueString();
        
        Assert.NotEmpty(test1);
        Assert.NotEmpty(test2);
        Assert.NotEmpty(test3);
        Assert.NotEqual(test1, test2);
        Assert.NotEqual(test2, test3);
        Assert.NotEqual(test3, test1);
    }
}
