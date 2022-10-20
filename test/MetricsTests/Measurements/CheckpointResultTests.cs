using Meshmakers.Common.Metrics.Measurements;
using Meshmakers.Common.MetricsTests.Helper;

namespace Meshmakers.Common.MetricsTests.Measurements;

public class CheckpointResultTests
{
    [Fact]
    public void Create()
    {
        var result = new CheckpointResult(RuntimeHelper.CreateCheckpoint());
        Assert.Equal(RuntimeHelper.DefaultCheckpointName, result.Name);
        Assert.Equal(1, result.Calls);
        Assert.Equal(RuntimeHelper.DefaultDelta, result.AvgOfDeltaInMs);
        Assert.Equal(RuntimeHelper.DefaultTotal, result.AvgOfTotalInMs);

        var now = DateTime.Now;
        Assert.Equal(result.FirstStart, result.LastStop);
        Assert.InRange(result.FirstStart, now.AddSeconds(-1), now);
    }

    [Fact]
    public async void Add()
    {
        var result = new CheckpointResult(RuntimeHelper.CreateCheckpoint());
        await Task.Delay(10);
        result.Add(RuntimeHelper.CreateCheckpoint(delta: RuntimeHelper.DefaultDelta * 10,
            total: RuntimeHelper.DefaultTotal * 10));

        Assert.Equal(RuntimeHelper.DefaultCheckpointName, result.Name);
        Assert.Equal(2, result.Calls);
        Assert.Equal(RuntimeHelper.DefaultDelta * 11 / 2, result.AvgOfDeltaInMs);
        Assert.Equal(RuntimeHelper.DefaultTotal * 11 / 2, result.AvgOfTotalInMs);
        Assert.True(result.FirstStart < result.LastStop);
    }

    [Fact]
    public void Add_InvalidName()
    {
        var result = new CheckpointResult(RuntimeHelper.CreateCheckpoint());
        Assert.Throws<MeasurementException>(() => result.Add(RuntimeHelper.CreateCheckpoint("InvalidName")));
    }

    [Fact]
    public void FirstDataTime_NoData()
    {
        var result = new CheckpointResult(RuntimeHelper.DefaultCheckpointName);
        Assert.Throws<MeasurementException>(() => result.FirstStart);
    }

    [Fact]
    public void LastFirstDataTime_NoData()
    {
        var result = new CheckpointResult(RuntimeHelper.DefaultCheckpointName);
        Assert.Throws<MeasurementException>(() => result.LastStop);
    }

    [Fact]
    public void GetDeltaMinMaxAvg()
    {
        const int min = 2;
        const int max = 4;
        const int avg = (min + max) / 2;
        var result = CreateCheckpointResult(min, max);
        Assert.Equal(min, result.MinOfDeltaInMs);
        Assert.Equal(max, result.MaxOfDeltaInMs);
        Assert.Equal(avg, result.AvgOfDeltaInMs);
    }

    [Fact]
    public void GetTotalMinMaxAvg()
    {
        const int min = 3;
        const int max = 6;
        const int avg = (min + max) / 2;
        var result = CreateCheckpointResult(min, max);
        Assert.Equal(min, result.MinOfTotalInMs);
        Assert.Equal(max, result.MaxOfTotalInMs);
        Assert.Equal(avg, result.AvgOfTotalInMs);
    }

    private static CheckpointResult CreateCheckpointResult(int min, int max)
    {
        var result = new CheckpointResult(RuntimeHelper.DefaultCheckpointName);
        result.Add(RuntimeHelper.CreateCheckpoint(delta: min, total: min));
        result.Add(RuntimeHelper.CreateCheckpoint(delta: max, total: max));
        return result;
    }
}