using Meshmakers.Common.Metrics.Providers;

namespace Meshmakers.Common.MetricsTests.Providers;

public class PrometheusMeasurementTests
{
    private const string Name = "Name";
    private const string Description = "Description";
    private static readonly string[] Labels = { "LabelA", "LabelB" };

    [Fact]
    public void CreateAndSet()
    {
        var measurement = new PrometheusMeasurement(Name, Description, Labels);
        measurement.Set(5, new[] { "1", "2" });
        Assert.Equal(Name, measurement.Name);
        Assert.Equal(Description, measurement.Description);
        Assert.Equal(Labels, measurement.Labels);
    }

    [Fact]
    public void CreateAndSet_WithoutLabels()
    {
        var measurement = new PrometheusMeasurement(Name, Description);
        measurement.Set(5);
        Assert.Equal(Name, measurement.Name);
        Assert.Equal(Description, measurement.Description);
        Assert.Null(measurement.Labels);
    }
}
