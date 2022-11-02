using System.Collections.Concurrent;
using Meshmakers.Common.Metrics.Measurements;
using Meshmakers.Common.Metrics.Meters;
using Meshmakers.Common.Metrics.Meters.Data;

namespace Meshmakers.Common.Metrics.Context;

internal class RuntimeDataCollection
{
    private ConcurrentDictionary<string, RuntimeData> RuntimeDataSet { get; } = new();
    private ConcurrentDictionary<string, RuntimeResult> RuntimeResults { get; } = new();

    public void Add(string name, RuntimeData runtimeData)
    {
        RuntimeDataSet.AddOrUpdate(name, runtimeData, (_, bag) =>
        {
            bag.Add(runtimeData);
            return bag;
        });
    }

    public IEnumerable<string> GetNames()
    {
        return RuntimeDataSet.Keys;
    }

    public RuntimeData GetByName(string name)
    {
        if (!RuntimeDataSet.TryGetValue(name, out var result))
        {
            throw new MeterException($"Unable to pop data with name '{name}'");
        }

        return result;
    }

    public RuntimeData PopByName(string name)
    {
        if (!RuntimeDataSet.TryRemove(name, out var result))
        {
            throw new MeterException($"Unable to pop data with name '{name}'");
        }

        return result;
    }

    public IEnumerable<RuntimeResult> PopResults()
    {
        foreach (var name in GetNames())
        {
            var runtimeData = PopByName(name);
            var runtimeResult = new RuntimeResult(name, runtimeData);
            RuntimeResults.AddOrUpdate(name, runtimeResult, (_, bag) =>
            {
                bag.Add(runtimeData);
                return bag;
            });
        }

        return RuntimeResults.Values;
    }
}
