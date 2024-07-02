using Meshmakers.Common.Metrics.Context;
using Meshmakers.Common.MetricsTests.Helper;

namespace Meshmakers.Common.MetricsTests.Meters.Data;

public class RuntimeDataCollectionTests
{
    [Fact]
    public void GetNames()
    {
        var collection = new RuntimeDataCollection();
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));
        collection.Add(RuntimeHelper.RuntimeName(2), RuntimeHelper.CreateRuntimeData(1));
        Assert.Equal(2, collection.GetNames().Count());
        Assert.Contains(RuntimeHelper.RuntimeName(1), collection.GetNames());
        Assert.Contains(RuntimeHelper.RuntimeName(2), collection.GetNames());
    }

    [Fact]
    public void GetByName()
    {
        var collection = new RuntimeDataCollection();
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));
        Assert.Single(collection.GetNames());
        Assert.NotNull(collection.GetByName(RuntimeHelper.RuntimeName(1)));
        Assert.Single(collection.GetNames());
    }

    [Fact]
    public void PopByName()
    {
        var collection = new RuntimeDataCollection();
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));
        Assert.Single(collection.GetNames());
        Assert.NotNull(collection.PopByName(RuntimeHelper.RuntimeName(1)));
        Assert.Empty(collection.GetNames());
    }

    [Fact]
    public async Task PopResults()
    {
        var collection = new RuntimeDataCollection();
        await Task.WhenAll(new Task[100].Select(_ => AddAndPop(collection)));
        await Task.Delay(1);
        Assert.Empty(collection.GetNames());
    }

    private static Task<int> AddAndPop(RuntimeDataCollection collection)
    {
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));
        Assert.Single(collection.PopResults());
        return Task.FromResult(1);
    }
}
