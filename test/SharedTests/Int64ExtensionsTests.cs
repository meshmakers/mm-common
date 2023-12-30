using Meshmakers.Common.Shared;
using Xunit;

namespace Meshmakers.Common.SharedTests;

public class Int64ExtensionsTests
{
    [Fact]
    public void ToIntArray_Min_OK()
    {
        long test = Int64.MinValue;
        var result = test.ToIntArray();

        Assert.Equal(
            new[] { -9, -2, -2, -3, -3, -7, -2, 0, -3, -6, -8, -5, -4, -7, -7, -5, -8, 0, -8 }, result);
    }

    [Fact]
    public void ToIntArray_0_OK()
    {
        long test = 0;
        var result = test.ToIntArray();

        Assert.Single(result);
        Assert.Equal(0, result[0]);
    }

    [Fact]
    public void ToIntArray_Max_OK()
    {
        long test = Int64.MaxValue;
        var result = test.ToIntArray();

        Assert.Equal(
            new[] { 9, 2, 2, 3, 3, 7, 2, 0, 3, 6, 8, 5, 4, 7, 7, 5, 8, 0, 7 }, result);
    }
}
