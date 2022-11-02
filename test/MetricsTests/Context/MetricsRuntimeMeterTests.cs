using Meshmakers.Common.Metrics.Context;
using Meshmakers.Common.Metrics.Measurements;
using Meshmakers.Common.MetricsTests.Helper;

namespace Meshmakers.Common.MetricsTests.Context;

public class MetricsRuntimeMeterTests
{
    [Fact(Timeout = 500)]
    public async void MeasureAndAnalyseParallel()
    {
        const int numIterations = 100;
        const int numAnalysis = 10;
        IMetricsContext metrics = new MetricsContext();

        var simulate = new Task[numIterations].Select(_ => RuntimeHelper.Simulate(metrics));
        var analyse = new Task[numAnalysis].Select(_ => AnalyseResultsWithGet(metrics, numIterations));

        var tasks = new List<Task>();
        tasks.AddRange(simulate);
        tasks.AddRange(analyse);
        await Task.WhenAll(tasks);

        Assert.Single(metrics.GetRuntimeNames());
        var name = metrics.GetRuntimeNames().First();
        var result = metrics.GetRuntimeResult(name);
        Assert.Equal(numIterations, result.Runs);
    }

    [Fact(Timeout = 500)]
    public async void PopMeasurements()
    {
        const int numIterations = 10;
        var metrics = new MetricsContext();

        var simulate = new Task[numIterations].Select(_ => RuntimeHelper.Simulate(metrics));
        var tasks = new List<Task> { AnalyseResultsWithPop(metrics, numIterations) };
        tasks.AddRange(simulate);
        await Task.WhenAll(tasks);

        while (metrics.GetRuntimeNames().Any())
        {
            await Task.Delay(1);
        }

        Assert.Empty(metrics.GetRuntimeNames());
    }

    [Fact]
    public void GetRuntimeResult_InvalidName()
    {
        var metrics = new MetricsContext();
        var result = metrics.GetRuntimeResult("InvalidName");
        RuntimeHelper.ValidateEmptyResultSet(result);
    }

    [Fact]
    public void PopRuntimeResult_InvalidName()
    {
        var metrics = new MetricsContext();
        var result = metrics.PopRuntimeResult("InvalidName");
        RuntimeHelper.ValidateEmptyResultSet(result);
    }

    private static async Task<int> AnalyseResultsWithGet(IMetricsContext metricsContext, int expectedRuns)
    {
        RuntimeResult result;
        do
        {
            await Task.Delay(5);
            result = metricsContext.GetRuntimeResult(RuntimeHelper.SimulationName);
        } while (result.Runs < expectedRuns);

        return 1;
    }

    private static async Task<int> AnalyseResultsWithPop(IMetricsContext metricsContext, int expectedRuns)
    {
        var sumRuns = 0;
        do
        {
            await Task.Delay(5);
            var result = metricsContext.PopRuntimeResult(RuntimeHelper.SimulationName);
            sumRuns += result.Runs;
        } while (sumRuns < expectedRuns);

        return 1;
    }
}