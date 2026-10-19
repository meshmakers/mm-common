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
    public async Task Add()
    {
        var result = new CheckpointResult(RuntimeHelper.CreateCheckpoint());
        await Task.Delay(10, TestContext.Current.CancellationToken);
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

    [Fact]
    public async Task Merge_EqualsAddingTheCheckpointsOneByOne()
    {
        var left = new CheckpointResult(RuntimeHelper.CreateCheckpoint(delta: 10, total: 30));
        left.Add(RuntimeHelper.CreateCheckpoint(delta: 20, total: 40));
        await Task.Delay(10, TestContext.Current.CancellationToken);
        var right = new CheckpointResult(RuntimeHelper.CreateCheckpoint(delta: 5, total: 100));
        right.Add(RuntimeHelper.CreateCheckpoint(delta: 25, total: 50));

        var expected = new CheckpointResult(RuntimeHelper.CreateCheckpoint(delta: 10, total: 30));
        expected.Add(RuntimeHelper.CreateCheckpoint(delta: 20, total: 40));
        expected.Add(RuntimeHelper.CreateCheckpoint(delta: 5, total: 100));
        expected.Add(RuntimeHelper.CreateCheckpoint(delta: 25, total: 50));

        left.Merge(right);

        Assert.Equal(4, left.Calls);
        Assert.Equal(expected.MinOfDeltaInMs, left.MinOfDeltaInMs);
        Assert.Equal(expected.MaxOfDeltaInMs, left.MaxOfDeltaInMs);
        Assert.Equal(expected.MinOfTotalInMs, left.MinOfTotalInMs);
        Assert.Equal(expected.MaxOfTotalInMs, left.MaxOfTotalInMs);
        // 15 = (10 + 20 + 5 + 25) / 4 and 55 = (30 + 40 + 100 + 50) / 4; the weighted mean is exact here.
        Assert.Equal(15, left.AvgOfDeltaInMs);
        Assert.Equal(55, left.AvgOfTotalInMs);
        Assert.True(left.FirstStart < left.LastStop);
    }

    [Fact]
    public void Merge_IntoAnEmptyResultCopiesTheOther()
    {
        var empty = new CheckpointResult(RuntimeHelper.DefaultCheckpointName);
        var other = new CheckpointResult(RuntimeHelper.CreateCheckpoint(delta: 7, total: 9));

        empty.Merge(other);

        Assert.Equal(1, empty.Calls);
        Assert.Equal(7, empty.MinOfDeltaInMs);
        Assert.Equal(9, empty.MaxOfTotalInMs);
    }

    [Fact]
    public void Merge_InvalidName()
    {
        var result = new CheckpointResult(RuntimeHelper.CreateCheckpoint());
        var other = new CheckpointResult(RuntimeHelper.CreateCheckpoint("Other"));
        Assert.Throws<MeasurementException>(() => result.Merge(other));
    }
}
