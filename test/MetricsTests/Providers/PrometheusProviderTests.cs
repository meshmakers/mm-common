using System.Net;
using Meshmakers.Common.Metrics.Providers;

namespace Meshmakers.Common.MetricsTests.Providers;

public class PrometheusProviderTests : IDisposable
{
    private const string MetricsEndpoint = "/metrics";
    private const int MetricsPort = 9999;
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri($"http://localhost:{MetricsPort}") };

    private readonly PrometheusProvider _provider;

    public PrometheusProviderTests()
    {
        _provider = new PrometheusProvider(MetricsPort);
    }

    public void Dispose()
    {
        _provider.Stop();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async void StartStop()
    {
        await Assert.ThrowsAsync<HttpRequestException>(() => _httpClient.GetAsync(MetricsEndpoint));
        _provider.Start();

        var response = await _httpClient.GetAsync(MetricsEndpoint);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _provider.Stop();
        await Assert.ThrowsAsync<HttpRequestException>(() => _httpClient.GetAsync(MetricsEndpoint));
    }

    [Fact]
    public async void StartDispose()
    {
        await Assert.ThrowsAsync<HttpRequestException>(() => _httpClient.GetAsync(MetricsEndpoint));
        _provider.Start();

        var response = await _httpClient.GetAsync(MetricsEndpoint);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _provider.Dispose();
        await Assert.ThrowsAsync<HttpRequestException>(() => _httpClient.GetAsync(MetricsEndpoint));
    }

    [Fact]
    public async void SuppressDefaultMetrics()
    {
        _provider.Start();
        var response = await _httpClient.GetAsync(MetricsEndpoint);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("prometheus_net_", content);
    }

    [Fact]
    public async void ContainsData()
    {
        const string name = "MyRuntime";
        const string description = "MyDescription";
        _provider.Start();
        Assert.NotNull(new PrometheusMeasurement(name, description));
        var response = await _httpClient.GetAsync(MetricsEndpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
        Assert.Contains(name, content);
        Assert.Contains(description, content);
        Assert.Contains($"{name} 0", content);
    }

    [Fact]
    public async void ContainsComplexData()
    {
        const string name = "MyRuntime";
        const string description = "MyDescription";
        const string label1 = "Label_1";
        const string label2 = "Label_2";
        const double value = 123.456;
        const string detail1 = "detail_text";
        const bool detail2 = true;

        _provider.Start();
        var measurement = new PrometheusMeasurement(name, description, new[] { label1, label2 });
        measurement.Set(value, new[] { detail1, detail2.ToString() });
        var response = await _httpClient.GetAsync(MetricsEndpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
        Assert.Contains(name, content);
        Assert.Contains(description, content);
        Assert.Contains($"{label1}=\"{detail1}\"", content);
        Assert.Contains($"{label2}=\"{detail2}\"", content);
    }
}