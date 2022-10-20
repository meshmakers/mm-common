namespace Meshmakers.Common.Metrics.Meters;

public abstract class BaseMeter : IDisposable
{
    protected BaseMeter(string name)
    {
        Name = name;
    }

    public bool Disposed { get; private set; }
    public string Name { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BaseMeter()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (Disposed)
            return;
        if (disposing)
            DisposingExplicit();
        Disposed = true;
    }

    protected virtual void DisposingExplicit()
    {
    }
}