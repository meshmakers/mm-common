using Meshmakers.Common.Metrics.Meters.Data;
using Meshmakers.Common.MetricsTests.Helper;

namespace Meshmakers.Common.MetricsTests.Meters.Data;

public class CheckpointTests
{
    [Fact]
    public void Create()
    {
        const long delta = RuntimeHelper.DefaultDelta;
        const long total = RuntimeHelper.DefaultDelta * 2;
        var checkpoint = new Checkpoint(RuntimeHelper.DefaultCheckpointName, delta, total);

        Assert.Equal(RuntimeHelper.DefaultCheckpointName, checkpoint.Name);
        Assert.Equal(delta, checkpoint.DeltaMilliseconds);
        Assert.Equal(total, checkpoint.TotalMilliseconds);
        var now = DateTime.Now;
        Assert.InRange(checkpoint.CreationDateTime, now.AddSeconds(-1), now);
    }
}