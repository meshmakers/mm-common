using Meshmakers.Common.Metrics.Context;
using Meshmakers.Common.Metrics.Measurements;
using Meshmakers.Common.Metrics.Meters;
using Meshmakers.Common.Metrics.Meters.Data;

namespace Meshmakers.Common.MetricsTests.Helper;

internal static class RuntimeHelper
{
    public const string DefaultRuntimeName = "RuntimeName";
    public const string DefaultCheckpointName = "Checkpoint";

    public const string SimulationName = "Simulate";

    public const int DefaultDelta = 3;
    public const int DefaultTotal = 6;

    public static Checkpoint CreateCheckpoint(string name = DefaultCheckpointName, int delta = DefaultDelta,
        int total = DefaultTotal)
    {
        return new Checkpoint(name, delta, total);
    }

    public static IEnumerable<Checkpoint> CreateCheckpoints(int numCheckpoints)
    {
        var checkpoints = new List<Checkpoint>();
        for (var cp = 0; cp < numCheckpoints; cp++)
            checkpoints.Add(new Checkpoint(CheckpointName(cp), DefaultDelta, _total(cp)));
        return checkpoints;
    }

    public static RuntimeData CreateRuntimeData(int numCheckpoints)
    {
        var checkpoints = new List<Checkpoint> { new(Checkpoint.StartName, 0, 0) };
        checkpoints.AddRange(CreateCheckpoints(numCheckpoints));
        checkpoints.Add(new Checkpoint(Checkpoint.StopName, DefaultDelta, _total(numCheckpoints)));
        return new RuntimeData(checkpoints);
    }

    public static RuntimeResult CreateRuntimeResult(int numCheckpoints)
    {
        return new RuntimeResult(DefaultRuntimeName, CreateRuntimeData(numCheckpoints));
    }

    public static RuntimeDataArgs CreateRuntimeDataArgs(string runtimeName, int numCheckpoints)
    {
        var checkpoints = CreateCheckpoints(numCheckpoints);
        return new RuntimeDataArgs(runtimeName, new RuntimeData(checkpoints));
    }

    public static async void Simulate(RuntimeMeter meter, int delta = DefaultDelta)
    {
        meter.Start();
        await Task.Delay(delta);
        meter.SetCheckpoint(DefaultCheckpointName);
        await Task.Delay(delta);
        meter.Stop();
    }

    public static async Task<int> Simulate(IMetricsContext metricsContext, int delta = 3)
    {
        using var meter = metricsContext.CreateRuntimeMeter();
        Assert.Equal(meter.Name, SimulationName);
        await Task.Delay(delta);
        meter.SetCheckpoint(DefaultCheckpointName);
        await Task.Delay(delta);
        return 1;
    }

    public static void ValidateCheckpoints(IEnumerable<Checkpoint> checkpoints, string name, int delta, int total)
    {
        var checkpoint = checkpoints.First(c => c.Name.Equals(name));
        Assert.True(delta <= total);
        Assert.True(checkpoint.DeltaMilliseconds >= delta, $"Delta: {checkpoint.DeltaMilliseconds} >= {delta}");
        Assert.True(checkpoint.TotalMilliseconds >= total, $"Total: {checkpoint.TotalMilliseconds} >= {total}");
    }

    public static string CheckpointName(int num)
    {
        return $"{DefaultCheckpointName}_{num}";
    }

    public static string RuntimeName(int num)
    {
        return $"{DefaultRuntimeName}_{num}";
    }

    private static long _total(int num)
    {
        return DefaultDelta * num;
    }

    public static void ValidateEmptyResultSet(RuntimeResult result)
    {
        Assert.Equal(0, result.Runs);
        Assert.Equal(0, result.NumCustomCheckpoints);
        Assert.Empty(result.GetCustomCheckpointNames());
        Assert.Throws<MeasurementException>(() => result.GetAverageTotalInMs());
        Assert.Throws<MeasurementException>(() => result.GetFirstStart());
        Assert.Throws<MeasurementException>(() => result.GetLastStop());
    }
}