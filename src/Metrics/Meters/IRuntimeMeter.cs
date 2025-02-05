namespace Meshmakers.Common.Metrics.Meters;

public interface IRuntimeMeter : IDisposable
{
    void Start();
    void SetCheckpoint(string name);
    void Stop();
    bool Disposed { get; }
    string Name { get; }
}
