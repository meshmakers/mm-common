using Meshmakers.Common.Metrics.Measurements;
using Meshmakers.Common.Metrics.Providers;
using Meshmakers.Common.MetricsTests.Helper;

namespace Meshmakers.Common.MetricsTests.Providers;

public class RuntimePublisherTests
{
    [Fact]
    public void Publish_NoCheckpoints()
    {
        var publisher = new RuntimePublisher();
        publisher.Publish(RuntimeHelper.CreateRuntimeResult(0));
        Assert.NotNull(publisher.Calls);
        Assert.Single(publisher.Runtimes);
        Assert.Empty(publisher.Checkpoints);
    }

    [Fact]
    public void Publish_WithCheckpoints()
    {
        var publisher = new RuntimePublisher();
        publisher.Publish(RuntimeHelper.CreateRuntimeResult(2));
        Assert.NotNull(publisher.Calls);
        Assert.Single(publisher.Runtimes);
        Assert.Single(publisher.Checkpoints);
    }

    [Fact]
    public void Publish_EmptyResult()
    {
        var publisher = new RuntimePublisher();
        publisher.Publish(new RuntimeResult("EmptyResult"));
        Assert.Null(publisher.Calls);
        Assert.Empty(publisher.Runtimes);
        Assert.Empty(publisher.Checkpoints);
    }
}