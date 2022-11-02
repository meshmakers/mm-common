using Meshmakers.Common.MetricsTests.Helper;

namespace Meshmakers.Common.MetricsTests.Meters.Data;

public class RuntimeDataArgsTests
{
    [Fact]
    public void Create()
    {
        const int numCheckpoints = 5;
        var args = RuntimeHelper.CreateRuntimeDataArgs(RuntimeHelper.DefaultRuntimeName, numCheckpoints);
        Assert.Equal(RuntimeHelper.DefaultRuntimeName, args.Name);
        Assert.Equal(numCheckpoints, args.RuntimeData.GetCheckpoints().Count);
    }
}
