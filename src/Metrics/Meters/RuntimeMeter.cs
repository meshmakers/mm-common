using System.Diagnostics;
using Meshmakers.Common.Metrics.Meters.Data;

namespace Meshmakers.Common.Metrics.Meters;

public class RuntimeMeter : BaseMeter
{
    private readonly List<Checkpoint> _checkpoints = new();
    private readonly Stopwatch _totalTime = new();
    private long _timeSinceLastCheckpoint;

    internal RuntimeMeter(string name) : base(name)
    {
    }

    internal event EventHandler<RuntimeDataArgs>? RunCompleted;

    public void Start()
    {
        if (IsRunning())
        {
            Stop();
        }

        _totalTime.Reset();
        CreateCheckpoint(Checkpoint.StartName);
        _totalTime.Start();
    }

    public void SetCheckpoint(string name)
    {
        if (!IsRunning())
        {
            throw new MeterException("RuntimeMeter has not yet been started");
        }

        _totalTime.Stop();
        CreateCheckpoint(name);
        _totalTime.Start();
    }

    public void Stop()
    {
        if (!IsRunning())
        {
            throw new MeterException("RuntimeMeter has not yet been started");
        }

        _totalTime.Stop();
        CreateCheckpoint(Checkpoint.StopName);
        EmitCheckpoints();
        _timeSinceLastCheckpoint = 0;
    }

    internal bool IsRunning()
    {
        return _totalTime.IsRunning;
    }

    private void CreateCheckpoint(string name)
    {
        var total = _totalTime.ElapsedMilliseconds;
        var delta = total - _timeSinceLastCheckpoint;
        _checkpoints.Add(new Checkpoint(name, delta, total));
        _timeSinceLastCheckpoint = total;
    }

    private void EmitCheckpoints()
    {
        var runtimeData = new RuntimeData(_checkpoints);
        _checkpoints.Clear();
        RunCompleted?.Invoke(this, new RuntimeDataArgs(Name, runtimeData));
    }

    protected override void DisposingExplicit()
    {
        if (IsRunning())
        {
            Stop();
        }
    }
}
