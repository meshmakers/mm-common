using System.Net;
using Meshmakers.Common.Metrics.Context;
using Meshmakers.Common.MetricsTests.Helper;

namespace Meshmakers.Common.MetricsTests.Context;

public class MetricsRuntimeProviderTests : IDisposable
{
    private const string MetricsEndpoint = "/metrics";
    private const int MetricsPort = 9998;
    private readonly IMetricsContext _metrics;

    private readonly Random _random = new();

    public MetricsRuntimeProviderTests()
    {
        _metrics = new MetricsContext();
    }

    public void Dispose()
    {
        _metrics.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async void Test()
    {
        const int numIterations = 100;
        const int numRequests = 100;
        _metrics.InitProvider(1, MetricsPort);

        var simulate = new Task[numIterations].Select(_ => RuntimeHelper.Simulate(_metrics));
        var requests = new Task[numRequests].Select(_ => Request());

        var tasks = new List<Task>();
        tasks.AddRange(simulate);
        tasks.AddRange(requests);
        await Task.WhenAll(tasks);
    }

    private async Task<int> Request()
    {
        await Task.Delay(_random.Next(0, 4));
        var httpClient = new HttpClient { BaseAddress = new Uri($"http://localhost:{MetricsPort}") };
        var response = await httpClient.GetAsync(MetricsEndpoint);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return 1;
    }
}
