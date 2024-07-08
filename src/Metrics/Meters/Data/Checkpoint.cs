namespace Meshmakers.Common.Metrics.Meters.Data;

internal class Checkpoint(string name, long deltaMilliseconds, long totalMilliseconds)
{
    public const string StartName = "__start__checkpoint__";
    public const string StopName = "__stop__checkpoint__";

    public string Name { get; } = name;
    public long DeltaMilliseconds { get; } = deltaMilliseconds;
    public long TotalMilliseconds { get; } = totalMilliseconds;
    public DateTime CreationDateTime { get; } = DateTime.Now;
}
