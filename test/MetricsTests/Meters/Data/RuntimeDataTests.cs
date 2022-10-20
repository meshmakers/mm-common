using Meshmakers.Common.Metrics.Meters.Data;
using Meshmakers.Common.MetricsTests.Helper;

namespace Meshmakers.Common.MetricsTests.Meters.Data;

public class RuntimeDataTests
{
    [Fact]
    public void Create_Add_GetCheckpoints()
    {
        const int numCheckpoints = 3;
        var runtimeData = new RuntimeData(RuntimeHelper.CreateCheckpoints(numCheckpoints));
        runtimeData.Add(new RuntimeData(RuntimeHelper.CreateCheckpoints(numCheckpoints)));
        Assert.Equal(numCheckpoints * 2, runtimeData.GetCheckpoints().Count);
    }
}