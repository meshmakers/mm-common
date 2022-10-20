using Meshmakers.Common.Metrics.Measurements;
using Meshmakers.Common.Metrics.Meters.Data;
using Meshmakers.Common.MetricsTests.Helper;

namespace Meshmakers.Common.MetricsTests.Measurements;

public class RuntimeResultTests
{
    private const int DefaultRuns = 10;
    private const int DefaultNumCheckpoints = 4;

    private readonly List<Checkpoint> _checkpoints = new();

    [Fact]
    public async void Create()
    {
        await SetupCheckpoints();
        var result = new RuntimeResult(RuntimeHelper.DefaultRuntimeName, new RuntimeData(_checkpoints));
        Assert.Equal(RuntimeHelper.DefaultRuntimeName, result.Name);
        Assert.Equal(DefaultRuns, result.Runs);
        Assert.Equal(DefaultNumCheckpoints, result.NumCustomCheckpoints);
        Assert.True(result.IsValid());
        for (var cp = 1; cp <= DefaultNumCheckpoints; cp++)
            Assert.Contains(RuntimeHelper.CheckpointName(cp), result.GetCustomCheckpointNames());

        const int lastCheckpoint = 1;
        const long total = DefaultNumCheckpoints * RuntimeHelper.DefaultDelta + lastCheckpoint;
        Assert.Equal(total, result.GetAverageTotalInMs());
        Assert.Equal(RuntimeHelper.DefaultDelta,
            result.GetAverageTotalInMs(RuntimeHelper.CheckpointName(1)));

        var now = DateTime.Now;
        Assert.True(result.GetFirstStart() < result.GetLastStop());
        Assert.InRange(result.GetFirstStart(), now.AddSeconds(-1), now);
        Assert.InRange(result.GetLastStop(), now.AddSeconds(-1), now);
    }

    [Fact]
    public void EmptyResult()
    {
        RuntimeHelper.ValidateEmptyResultSet(RuntimeResult.EmptyResult());
    }

    [Fact]
    public void Invalid()
    {
        Assert.False(RuntimeResult.EmptyResult().IsValid());
    }

    [Fact]
    public async void Add()
    {
        var result = new RuntimeResult(RuntimeHelper.DefaultRuntimeName);
        Assert.Equal(0, result.Runs);

        await SetupCheckpoints(1, 0);
        result.Add(new RuntimeData(_checkpoints));
        Assert.Equal(1, result.Runs);
        Assert.True(result.IsValid());
    }

    [Fact]
    public async void GetTotalResults()
    {
        var result = new RuntimeResult(RuntimeHelper.DefaultRuntimeName);
        await SetupCheckpoints(1, 0);
        result.Add(new RuntimeData(_checkpoints));
        Assert.True(result.GetMinTotalInMs() > 0);
        Assert.True(result.GetMaxTotalInMs() > 0);
        Assert.True(result.GetAverageTotalInMs() > 0);
    }

    [Fact]
    public void TryGetInvalidCheckpoint()
    {
        var result = new RuntimeResult(RuntimeHelper.DefaultRuntimeName);
        const string invalid = "__invalid__";
        Assert.Throws<MeasurementException>(() => result.GetMinTotalInMs(invalid));
        Assert.Throws<MeasurementException>(() => result.GetMaxTotalInMs(invalid));
        Assert.Throws<MeasurementException>(() => result.GetAverageTotalInMs(invalid));
        Assert.Throws<MeasurementException>(() => result.GetStopCheckpoint());
    }

    [Fact]
    public void GetFirstStart_NoData()
    {
        var result = new RuntimeResult(RuntimeHelper.DefaultRuntimeName);
        Assert.False(result.IsValid());
        Assert.Throws<MeasurementException>(() => result.GetFirstStart());
    }

    [Fact]
    public void GetLastStop_NoData()
    {
        var result = new RuntimeResult(RuntimeHelper.DefaultRuntimeName);
        Assert.False(result.IsValid());
        Assert.Throws<MeasurementException>(() => result.GetLastStop());
    }

    private async Task SetupCheckpoints(int numRuns = DefaultRuns,
        int numCustomCheckpoints = DefaultNumCheckpoints, int delta = RuntimeHelper.DefaultDelta)
    {
        _checkpoints.Clear();
        for (var run = 1; run <= numRuns; run++)
            await AddCheckpoints(numCustomCheckpoints, delta);
    }

    private async Task AddCheckpoints(int numCustomCheckpoints = DefaultNumCheckpoints,
        int delta = RuntimeHelper.DefaultDelta)
    {
        await AddCheckpoint(Checkpoint.StartName, 0, 0);
        for (var cp = 1; cp <= numCustomCheckpoints; cp++)
            await AddCheckpoint(RuntimeHelper.CheckpointName(cp), delta, delta * cp);
        await AddCheckpoint(Checkpoint.StopName, delta, delta * numCustomCheckpoints + 1);
    }

    private async Task AddCheckpoint(string name, int delta, int total)
    {
        if (!name.Equals(Checkpoint.StartName))
            await Task.Delay(1);
        _checkpoints.Add(new Checkpoint(name, delta, total));
    }
}