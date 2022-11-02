using Meshmakers.Common.Metrics.Meters;
using Meshmakers.Common.Metrics.Meters.Data;
using Meshmakers.Common.MetricsTests.Helper;

namespace Meshmakers.Common.MetricsTests.Meters;

public class RuntimeMeterTest
{
    [Fact(Timeout = 100)]
    public async void CreateAndMeasure()
    {
        var meter = new RuntimeMeter(RuntimeHelper.DefaultRuntimeName);
        Assert.Equal(RuntimeHelper.DefaultRuntimeName, meter.Name);
        var checkpoints = new List<Checkpoint>();
        meter.RunCompleted += (_, args) => checkpoints = args.RuntimeData.GetCheckpoints();

        RuntimeHelper.Simulate(meter);
        while (checkpoints.Count < 3)
        {
            await Task.Delay(10);
        }

        Assert.Equal(3, checkpoints.Count);

        RuntimeHelper.ValidateCheckpoints(checkpoints, Checkpoint.StartName, 0, 0);
        RuntimeHelper.ValidateCheckpoints(checkpoints, RuntimeHelper.DefaultCheckpointName, 2, 2);
        RuntimeHelper.ValidateCheckpoints(checkpoints, Checkpoint.StopName, 2, 4);
    }

    [Fact]
    public async void SecondStartEmitsCheckpoints()
    {
        var meter = new RuntimeMeter(RuntimeHelper.DefaultRuntimeName);
        var checkpoints = new List<Checkpoint>();
        meter.RunCompleted += (_, args) => checkpoints = args.RuntimeData.GetCheckpoints();

        meter.Start();
        Assert.True(meter.IsRunning());
        Assert.Empty(checkpoints);

        meter.Start();
        await Task.Delay(10);
        Assert.True(meter.IsRunning());
        Assert.Equal(2, checkpoints.Count);
    }

    [Fact]
    public void SetCheckpoint_NotStarted()
    {
        var meter = new RuntimeMeter(RuntimeHelper.DefaultRuntimeName);
        Assert.Throws<MeterException>(() => meter.SetCheckpoint(RuntimeHelper.DefaultCheckpointName));
    }

    [Fact]
    public void Stop_NotStarted()
    {
        var meter = new RuntimeMeter(RuntimeHelper.DefaultRuntimeName);
        Assert.Throws<MeterException>(() => meter.Stop());
    }

    [Fact(Timeout = 100)]
    public async void ValidateArgs()
    {
        var meter = new RuntimeMeter(RuntimeHelper.DefaultRuntimeName);
        RuntimeDataArgs? checkpointsArgs = null;
        meter.RunCompleted += (_, args) => checkpointsArgs = args;

        meter.Start();
        meter.Stop();
        while (checkpointsArgs is null)
        {
            await Task.Delay(10);
        }

        Assert.Equal(RuntimeHelper.DefaultRuntimeName, checkpointsArgs.Name);
        Assert.Equal(2, checkpointsArgs.RuntimeData.GetCheckpoints().Count);
    }

    [Fact]
    public void Dispose()
    {
        var meter = new RuntimeMeter(RuntimeHelper.DefaultRuntimeName);
        var checkpoints = new List<Checkpoint>();
        meter.RunCompleted += (_, args) => checkpoints = args.RuntimeData.GetCheckpoints();

        meter.Start();
        meter.Dispose();
        Assert.True(meter.Disposed);
        Assert.NotEmpty(checkpoints);
    }
}
