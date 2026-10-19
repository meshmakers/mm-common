using System.Collections.Concurrent;
using Meshmakers.Common.Metrics.Measurements;
using Meshmakers.Common.Metrics.Meters;
using Meshmakers.Common.Metrics.Meters.Data;

namespace Meshmakers.Common.Metrics.Context;

/// <summary>
///     Collects the runs of all runtime meters per meter name.
/// </summary>
/// <remarks>
///     <para>
///         🔴 AB#7010: the collection holds an <b>aggregate</b> per name (calls, min, max, average per
///         checkpoint), never the individual <see cref="Checkpoint" /> objects of the runs. It used to
///         keep every checkpoint of every run in a bag until somebody popped it, and nobody does when no
///         publisher is started (<see cref="MetricsContext.InitProvider" /> is not called by any
///         service). The engine measures every runtime query with a meter, so a service that queries
///         often grew without bound: the energyiq mesh adapter on prod-1 retained about 1.6 GiB in
///         three days.
///     </para>
///     <para>
///         Memory is now bounded by the number of distinct meter names and checkpoint names, whatever
///         the number of runs.
///     </para>
/// </remarks>
internal class RuntimeDataCollection
{
    private ConcurrentDictionary<string, Accumulator> RuntimeDataSet { get; } = new();
    private ConcurrentDictionary<string, RuntimeResult> RuntimeResults { get; } = new();

    public void Add(string name, RuntimeData runtimeData)
    {
        while (true)
        {
            var accumulator = RuntimeDataSet.GetOrAdd(name, static n => new Accumulator(n));
            lock (accumulator)
            {
                // A concurrent pop retired this accumulator after we looked it up: the pop has taken
                // its content, so a write now would be lost. Look up the replacement and try again.
                if (accumulator.Retired)
                {
                    continue;
                }

                accumulator.Result.Add(runtimeData);
                return;
            }
        }
    }

    public IEnumerable<string> GetNames()
    {
        return RuntimeDataSet.Keys;
    }

    /// <summary>
    ///     A snapshot of what has been collected under <paramref name="name" /> since the last pop.
    /// </summary>
    public RuntimeResult GetByName(string name)
    {
        if (!RuntimeDataSet.TryGetValue(name, out var accumulator))
        {
            throw new MeterException($"Unable to pop data with name '{name}'");
        }

        lock (accumulator)
        {
            return accumulator.Result.Snapshot();
        }
    }

    /// <summary>
    ///     Removes and returns what has been collected under <paramref name="name" />.
    /// </summary>
    public RuntimeResult PopByName(string name)
    {
        if (!RuntimeDataSet.TryRemove(name, out var accumulator))
        {
            throw new MeterException($"Unable to pop data with name '{name}'");
        }

        lock (accumulator)
        {
            accumulator.Retired = true;
            return accumulator.Result;
        }
    }

    public IEnumerable<RuntimeResult> PopResults()
    {
        foreach (var name in GetNames())
        {
            RuntimeResult popped;
            try
            {
                popped = PopByName(name);
            }
            catch (MeterException)
            {
                // Popped by somebody else since the names were listed.
                continue;
            }

            var total = RuntimeResults.GetOrAdd(name, static n => new RuntimeResult(n));
            lock (total)
            {
                total.Merge(popped);
            }
        }

        return RuntimeResults.Values;
    }

    private sealed class Accumulator(string name)
    {
        public RuntimeResult Result { get; } = new(name);

        public bool Retired { get; set; }
    }
}
