using Meshmakers.Common.Metrics.Measurements;

namespace Meshmakers.Common.Metrics.Providers;

internal class RuntimePublisher
{
    private const string RuntimeCalls = "CallsPerMethod";
    private const string RuntimeCallsDescription = "Shows the number of calls per method";

    private const string RuntimePrefix = "Runtime_";
    private const string RuntimeDescription = "Shows the minimum, maximum and average runtime of a method";

    private const string CheckpointPrefix = "Checkpoints_";
    private const string CheckpointDescription = "Shows the minimum, maximum and average runtime of checkpoints";

    private const string TotalRuntimeName = "Total";

    public RuntimePublisher()
    {
        Runtimes = new Dictionary<string, PrometheusMeasurement>();
        Checkpoints = new Dictionary<string, PrometheusMeasurement>();
    }

    internal PrometheusMeasurement? Calls { get; private set; }
    internal Dictionary<string, PrometheusMeasurement> Runtimes { get; }
    internal Dictionary<string, PrometheusMeasurement> Checkpoints { get; }

    public void Publish(RuntimeResult result)
    {
        if (!result.IsValid())
            return;
        PublishCalls(result.Name, result.Runs);
        PublishMethod(result);
        PublishCheckpoints(result);
    }

    private void PublishCalls(string name, int calls)
    {
        Calls ??= new PrometheusMeasurement(RuntimeCalls, RuntimeCallsDescription, new[] { "Name" });
        Calls.Set(calls, new[] { name });
    }

    private void PublishMethod(RuntimeResult result)
    {
        if (!Runtimes.TryGetValue(result.Name, out var measurement))
            measurement = CreateAndAddMethodMeasurement(result.Name);
        measurement.Set(result.GetMinTotalInMs(), new[] { "min" });
        measurement.Set(result.GetMaxTotalInMs(), new[] { "max" });
        measurement.Set(result.GetAverageTotalInMs(), new[] { "avg" });
    }

    private void PublishCheckpoints(RuntimeResult result)
    {
        if (result.NumCustomCheckpoints == 0)
            return;
        if (!Checkpoints.TryGetValue(result.Name, out var measurement))
            measurement = CreateAndAddCheckpointsMeasurement(result.Name);
        foreach (var checkpoint in result.GetCustomCheckpoints())
            SetCheckpointMeasurements(measurement, checkpoint);
        var stop = result.GetStopCheckpoint();
        SetCheckpointMeasurements(
            measurement,
            TotalRuntimeName,
            stop.MinOfDeltaInMs,
            stop.MaxOfDeltaInMs,
            stop.AvgOfDeltaInMs);
    }

    private static void SetCheckpointMeasurements(PrometheusMeasurement measurement, CheckpointResult checkpoint)
    {
        SetCheckpointMeasurements(
            measurement,
            checkpoint.Name,
            checkpoint.MinOfDeltaInMs,
            checkpoint.MaxOfDeltaInMs,
            checkpoint.AvgOfDeltaInMs);
    }

    private static void SetCheckpointMeasurements(PrometheusMeasurement measurement, string name, long min, long max,
        long avg)
    {
        measurement.Set(min, new[] { name, "min" });
        measurement.Set(max, new[] { name, "max" });
        measurement.Set(avg, new[] { name, "avg" });
    }

    private PrometheusMeasurement CreateAndAddMethodMeasurement(string name)
    {
        var runtime = new PrometheusMeasurement(RuntimeName(name), RuntimeDescription, new[] { "measurement" });
        Runtimes.Add(name, runtime);
        return runtime;
    }

    private PrometheusMeasurement CreateAndAddCheckpointsMeasurement(string name)
    {
        var checkpoints = new PrometheusMeasurement(CheckpointsName(name), CheckpointDescription,
            new[] { "checkpoint", "measurement" });
        Checkpoints.Add(name, checkpoints);
        return checkpoints;
    }

    private static string RuntimeName(string name)
    {
        return $"{RuntimePrefix}{name}";
    }

    private static string CheckpointsName(string name)
    {
        return $"{CheckpointPrefix}{name}";
    }
}