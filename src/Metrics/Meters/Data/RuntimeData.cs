using System.Collections.Concurrent;

namespace Meshmakers.Common.Metrics.Meters.Data;

internal class RuntimeData
{
    private readonly ConcurrentBag<Checkpoint> _checkpoints;

    public RuntimeData(IEnumerable<Checkpoint> checkpoints)
    {
        _checkpoints = new ConcurrentBag<Checkpoint>(checkpoints);
    }

    public void Add(RuntimeData runtimeData)
    {
        runtimeData._checkpoints.ToList().ForEach(cp => _checkpoints.Add(cp));
    }

    public List<Checkpoint> GetCheckpoints()
    {
        return _checkpoints.ToArray().ToList();
    }
}