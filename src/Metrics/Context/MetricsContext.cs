using System.Runtime.CompilerServices;
using Meshmakers.Common.Metrics.Measurements;
using Meshmakers.Common.Metrics.Meters;
using Meshmakers.Common.Metrics.Meters.Data;
using Meshmakers.Common.Metrics.Providers;

namespace Meshmakers.Common.Metrics.Context;

public class MetricsContext : IMetricsContext
{
    private const int DefaultPublisherIntervalMs = 10000;
    private readonly RuntimeDataCollection _runtimeDataCollection = new();
    private int _activePublisherIntervalMs;
    private PrometheusProvider? _prometheusProvider;

    private bool _publishActive = true;
    private RuntimePublisher? _runtimePublisher;

    public RuntimeMeter CreateRuntimeMeter([CallerMemberName] string memberName = "")
    {
        var timeMeter = new RuntimeMeter(memberName);
        timeMeter.RunCompleted += HandleRunCompleted;
        timeMeter.Start();
        return timeMeter;
    }

    public void InitProvider(int intervalMs = DefaultPublisherIntervalMs, int port = PrometheusProvider.DefaultPort)
    {
        _activePublisherIntervalMs = intervalMs;
        _prometheusProvider = new PrometheusProvider(port);
        _prometheusProvider.Start();
        _runtimePublisher = new RuntimePublisher();
        var thread = new Thread(Publish);
        thread.Start();
    }

    public IEnumerable<string> GetRuntimeNames()
    {
        return _runtimeDataCollection.GetNames();
    }

    public RuntimeResult GetRuntimeResult(string name)
    {
        try
        {
            var data = _runtimeDataCollection.GetByName(name);
            return new RuntimeResult(name, data);
        }
        catch (MeterException)
        {
            return RuntimeResult.EmptyResult();
        }
    }

    public RuntimeResult PopRuntimeResult(string name)
    {
        try
        {
            var data = _runtimeDataCollection.PopByName(name);
            return new RuntimeResult(name, data);
        }
        catch (MeterException)
        {
            return RuntimeResult.EmptyResult();
        }
    }

    public void Dispose()
    {
        _publishActive = false;
        GC.SuppressFinalize(this);
    }

    ~MetricsContext()
    {
        Dispose();
    }

    private void HandleRunCompleted(object? sender, RuntimeDataArgs args)
    {
        _runtimeDataCollection.Add(args.Name, args.RuntimeData);
    }

    private void Publish()
    {
        while (_publishActive)
        {
            foreach (var runtimeResult in _runtimeDataCollection.PopResults())
                _runtimePublisher?.Publish(runtimeResult);
            Thread.Sleep(_activePublisherIntervalMs);
        }
    }
}