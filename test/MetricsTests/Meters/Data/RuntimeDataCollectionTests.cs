using System.Runtime.CompilerServices;
using Meshmakers.Common.Metrics.Context;
using Meshmakers.Common.Metrics.Meters;
using Meshmakers.Common.Metrics.Meters.Data;
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
        await Task.Delay(1, TestContext.Current.CancellationToken);
        Assert.Empty(collection.GetNames());
    }

    /// <summary>
    ///     AB#7010: the collection must not keep the checkpoints of the runs it was given. It kept them
    ///     in a bag until popped, and a service that never pops grew by the checkpoints of every query.
    /// </summary>
    [Fact]
    public void Add_DoesNotRetainTheCheckpointsOfTheRuns()
    {
        var collection = new RuntimeDataCollection();
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));

        var weak = AddRunAndTrackItsCheckpoint(collection);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(weak.IsAlive, "the collection retains the checkpoint of a run it has aggregated");
        GC.KeepAlive(collection);
    }

    // Not inlined and returning only the weak reference: the frame holding the strong reference ends
    // here, even in an unoptimised build where locals otherwise live to the end of the test method.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference AddRunAndTrackItsCheckpoint(RuntimeDataCollection collection)
    {
        var checkpoint = new Checkpoint(Checkpoint.StopName, 1, 1);
        collection.Add(RuntimeHelper.RuntimeName(1),
            new RuntimeData([new Checkpoint(Checkpoint.StartName, 0, 0), checkpoint]));
        return new WeakReference(checkpoint);
    }

    /// <summary>
    ///     Structural bound instead of a heap-size measurement (which depends on agent, GC mode and parallel
    ///     tests): none of the checkpoints of many aggregated runs may stay reachable, yet every run is counted.
    /// </summary>
    [Fact]
    public void Add_ManyRuns_RetainsNoneOfTheirCheckpointsAndCountsEveryRun()
    {
        const int runs = 2_000;
        var collection = new RuntimeDataCollection();

        var tracked = AddRunsAndTrackTheirCheckpoints(collection, runs);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var retained = tracked.Count(w => w.IsAlive);
        Assert.True(retained == 0, $"{retained} of {tracked.Length} checkpoints are still reachable from the collection");
        Assert.Equal(runs, collection.GetByName(RuntimeHelper.RuntimeName(1)).Runs);
        GC.KeepAlive(collection);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] AddRunsAndTrackTheirCheckpoints(RuntimeDataCollection collection, int runs)
    {
        var tracked = new WeakReference[runs * 2];
        for (var i = 0; i < runs; i++)
        {
            var start = new Checkpoint(Checkpoint.StartName, 0, 0);
            var stop = new Checkpoint(Checkpoint.StopName, 1, 1);
            collection.Add(RuntimeHelper.RuntimeName(1), new RuntimeData([start, stop]));
            tracked[i * 2] = new WeakReference(start);
            tracked[i * 2 + 1] = new WeakReference(stop);
        }

        return tracked;
    }

    [Fact]
    public void GetByName_ReturnsASnapshotThatLaterRunsDoNotChange()
    {
        var collection = new RuntimeDataCollection();
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));
        var snapshot = collection.GetByName(RuntimeHelper.RuntimeName(1));

        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));

        Assert.Equal(1, snapshot.Runs);
        Assert.Equal(2, collection.GetByName(RuntimeHelper.RuntimeName(1)).Runs);
    }

    [Fact]
    public void PopByName_ReturnsTheCollectedRunsAndStartsOver()
    {
        var collection = new RuntimeDataCollection();
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));

        var popped = collection.PopByName(RuntimeHelper.RuntimeName(1));
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));

        Assert.Equal(2, popped.Runs);
        Assert.Equal(1, collection.GetByName(RuntimeHelper.RuntimeName(1)).Runs);
    }

    [Fact]
    public async Task ConcurrentAddAndPop_LoseNoRun()
    {
        const int adders = 8;
        const int runsPerAdder = 5_000;
        var collection = new RuntimeDataCollection();
        var done = false;
        var popped = 0;

        var popper = Task.Run(() =>
        {
            while (!Volatile.Read(ref done))
            {
                try
                {
                    Interlocked.Add(ref popped, collection.PopByName(RuntimeHelper.RuntimeName(1)).Runs);
                }
                catch (MeterException)
                {
                    // nothing collected yet
                }
            }
        }, TestContext.Current.CancellationToken);

        await Task.WhenAll(Enumerable.Range(0, adders).Select(_ => Task.Run(() =>
        {
            for (var i = 0; i < runsPerAdder; i++)
            {
                collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));
            }
        }, TestContext.Current.CancellationToken)));
        Volatile.Write(ref done, true);
        await popper;

        var rest = collection.GetNames().Any() ? collection.PopByName(RuntimeHelper.RuntimeName(1)).Runs : 0;
        Assert.Equal(adders * runsPerAdder, Volatile.Read(ref popped) + rest);
    }

    [Fact]
    public void PopResults_AccumulatesOverAllPops()
    {
        var collection = new RuntimeDataCollection();
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));
        collection.PopResults();
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));

        var result = Assert.Single(collection.PopResults());

        Assert.Equal(3, result.Runs);
        Assert.Empty(collection.GetNames());
    }

    private static Task<int> AddAndPop(RuntimeDataCollection collection)
    {
        collection.Add(RuntimeHelper.RuntimeName(1), RuntimeHelper.CreateRuntimeData(1));
        Assert.Single(collection.PopResults());
        return Task.FromResult(1);
    }
}
