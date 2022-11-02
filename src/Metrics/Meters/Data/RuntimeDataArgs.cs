namespace Meshmakers.Common.Metrics.Meters.Data;

internal class RuntimeDataArgs : EventArgs
{
    public RuntimeDataArgs(string name, RuntimeData runtimeData)
    {
        Name = name;
        RuntimeData = runtimeData;
    }

    public string Name { get; }
    public RuntimeData RuntimeData { get; }
}
