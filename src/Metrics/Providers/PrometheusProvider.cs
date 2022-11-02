using Prometheus;
using ProMetrics = Prometheus.Metrics;

namespace Meshmakers.Common.Metrics.Providers;

internal class PrometheusProvider : IDisposable
{
    public const int DefaultPort = 9184;
    private readonly MetricServer _server;


    public PrometheusProvider(int port = DefaultPort)
    {
        _server = new MetricServer(port);
        ProMetrics.SuppressDefaultMetrics();
    }

    public void Dispose()
    {
        _server.Stop();
        GC.SuppressFinalize(this);
    }

    ~PrometheusProvider()
    {
        Dispose();
    }

    public void Start()
    {
        _server.Start();
    }

    public void Stop()
    {
        _server.Stop();
    }
}
