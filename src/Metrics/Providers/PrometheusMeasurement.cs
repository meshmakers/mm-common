using Prometheus;
using ProMetrics = Prometheus.Metrics;

namespace Meshmakers.Common.Metrics.Providers;

internal class PrometheusMeasurement
{
    private readonly Gauge _measurement;

    public PrometheusMeasurement(string name, string description, string[]? labels = null)
    {
        Name = name;
        Description = description;
        Labels = labels;

        var configuration = Labels is null
            ? null
            : new GaugeConfiguration { LabelNames = Labels };
        _measurement = ProMetrics.CreateGauge(Name, Description, configuration);
    }

    public string Name { get; }
    public string Description { get; }
    public string[]? Labels { get; }

    public void Set(double value, string[]? details = null)
    {
        if (details is null)
        {
            _measurement.Set(value);
        }
        else
        {
            _measurement.WithLabels(details).Set(value);
        }
    }
}