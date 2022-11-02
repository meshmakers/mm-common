using Meshmakers.Common.Metrics.Meters.Data;

namespace Meshmakers.Common.Metrics.Measurements;

public class RuntimeResult
{
    private const string EmptyResultName = "__empty_result__";

    private readonly List<CheckpointResult> _checkpointResults = new();

    internal RuntimeResult(string name)
    {
        Name = name;
    }

    internal RuntimeResult(string name, RuntimeData runtimeData) : this(name)
    {
        Add(runtimeData);
    }

    public string Name { get; }
    public int Runs => GetRuns();
    public int NumCustomCheckpoints => GetNumCustomCheckpoints();

    internal static RuntimeResult EmptyResult()
    {
        return new RuntimeResult(EmptyResultName);
    }

    internal void Add(RuntimeData runtimeData)
    {
        runtimeData.GetCheckpoints().ForEach(AddCheckpoint);
    }

    private void AddCheckpoint(Checkpoint checkpoint)
    {
        var cpResult = _checkpointResults.Find(cp => cp.Name.Equals(checkpoint.Name));
        if (cpResult == null)
        {
            cpResult = new CheckpointResult(checkpoint.Name);
            _checkpointResults.Add(cpResult);
        }

        cpResult.Add(checkpoint);
    }

    public bool IsValid()
    {
        return _checkpointResults.Count > 0;
    }

    public long GetMinTotalInMs(string checkpointName = Checkpoint.StopName)
    {
        var cpr = _checkpointResults.Find(cpr => cpr.Name.Equals(checkpointName));
        if (cpr == null)
        {
            throw new MeasurementException($"There is no checkpoint with the name '{checkpointName}'");
        }

        return cpr.MinOfTotalInMs;
    }

    public long GetMaxTotalInMs(string checkpointName = Checkpoint.StopName)
    {
        var cpr = _checkpointResults.Find(cpr => cpr.Name.Equals(checkpointName));
        if (cpr == null)
        {
            throw new MeasurementException($"There is no checkpoint with the name '{checkpointName}'");
        }

        return cpr.MaxOfTotalInMs;
    }

    public long GetAverageTotalInMs(string checkpointName = Checkpoint.StopName)
    {
        var cpr = _checkpointResults.Find(cpr => cpr.Name.Equals(checkpointName));
        if (cpr == null)
        {
            throw new MeasurementException($"There is no checkpoint with the name '{checkpointName}'");
        }

        return cpr.AvgOfTotalInMs;
    }

    public HashSet<CheckpointResult> GetCustomCheckpoints()
    {
        var checkpoints = new HashSet<CheckpointResult>();
        _checkpointResults.ForEach(cpr =>
        {
            var name = cpr.Name;
            if (name.Equals(Checkpoint.StartName) || name.Equals(Checkpoint.StopName))
            {
                return;
            }

            checkpoints.Add(cpr);
        });
        return checkpoints;
    }

    public HashSet<string> GetCustomCheckpointNames()
    {
        return GetCustomCheckpoints().Select(cpr => cpr.Name).ToHashSet();
    }

    public CheckpointResult GetStopCheckpoint()
    {
        var final = _checkpointResults.Find(cpr => cpr.Name.Equals(Checkpoint.StopName));
        if (final == null)
        {
            throw new MeasurementException("There is no data available");
        }

        return final;
    }

    public DateTime GetFirstStart()
    {
        var cpr = _checkpointResults.MinBy(cpr => cpr.FirstStart);
        if (cpr == null)
        {
            throw new MeasurementException("There is no data available");
        }

        return cpr.FirstStart;
    }

    public DateTime GetLastStop()
    {
        var cpr = _checkpointResults.MaxBy(cpr => cpr.LastStop);
        if (cpr == null)
        {
            throw new MeasurementException("There is no data available");
        }

        return cpr.LastStop;
    }

    private int GetRuns()
    {
        var checkpointResult = _checkpointResults.Find(cpr => cpr.Name.Equals(Checkpoint.StartName));
        return checkpointResult?.Calls ?? 0;
    }

    private int GetNumCustomCheckpoints()
    {
        return GetCustomCheckpointNames().Count;
    }
}