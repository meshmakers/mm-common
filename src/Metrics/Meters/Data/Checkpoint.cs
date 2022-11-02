namespace Meshmakers.Common.Metrics.Meters.Data;

internal class Checkpoint
{
    public const string StartName = "__start__checkpoint__";
    public const string StopName = "__stop__checkpoint__";

    public Checkpoint(string name, long deltaMilliseconds, long totalMilliseconds)
    {
        Name = name;
        DeltaMilliseconds = deltaMilliseconds;
        TotalMilliseconds = totalMilliseconds;
    }

    public string Name { get; }
    public long DeltaMilliseconds { get; }
    public long TotalMilliseconds { get; }
    public DateTime CreationDateTime { get; } = DateTime.Now;
}
