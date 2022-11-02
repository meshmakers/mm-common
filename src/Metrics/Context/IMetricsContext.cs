using System.Runtime.CompilerServices;
using Meshmakers.Common.Metrics.Measurements;
using Meshmakers.Common.Metrics.Meters;

namespace Meshmakers.Common.Metrics.Context;

public interface IMetricsContext : IDisposable
{
    /// <summary>
    ///     Creates and starts a runtime meter. User-defined checkpoints can be created and the measurement can be stopped.
    ///     If the meter goes out of context, it is automatically stopped.
    /// </summary>
    /// <param name="memberName">
    ///     The name of the method in which CreateRuntimeMeter is called is automatically used.
    ///     A name can also optionally be assigned manually.
    /// </param>
    /// <returns></returns>
    public RuntimeMeter CreateRuntimeMeter([CallerMemberName] string memberName = "");

    /// <summary>
    ///     Initiates a prometheus-provider that publishes the collected date at a specified interval.
    ///     The results can then be retrieved via endpoint http://localhost:9184/metrics, for example.
    /// </summary>
    /// <param name="intervalMs">The interval at which the results for the endpoint are updated.</param>
    /// <param name="port">The port under which the endpoint is published.</param>
    public void InitProvider(int intervalMs, int port);

    /// <summary>
    ///     Returns the names of all currently available measurements.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetRuntimeNames();

    /// <summary>
    ///     Queries the interpretation of measurements by their name.
    /// </summary>
    /// <param name="name">The name of the desired measurement.</param>
    /// <returns>Contains the result of the desired measurement.</returns>
    public RuntimeResult GetRuntimeResult(string name);

    /// <summary>
    ///     Queries and removes the interpretation of measurements by their name.
    /// </summary>
    /// <param name="name">The name of the desired measurement.</param>
    /// <returns>Contains the result of the desired measurement.</returns>
    public RuntimeResult PopRuntimeResult(string name);
}
