using System.Diagnostics;
using System.Text;

using NLog;
using NLog.LayoutRenderers;

namespace Meshmakers.Common.Observability;

/// <summary>
/// Renders the trace id of the ambient <see cref="Activity" /> as 32 lowercase hex characters,
/// or nothing when no activity is current.
/// </summary>
/// <remarks>
/// <para>
/// AB#5478 section 2.3. Logs and traces are two unrelated worlds until every log record carries
/// the trace context of the operation that produced it. NLog 6.2 core ships no activity layout
/// renderer — verified by inspecting <c>NLog.dll</c>, which contains no reference to
/// <c>Activity</c> at all — and the maintained <c>NLog.DiagnosticSource</c> package would add a
/// dependency plus an assembly auto-load question for a renderer this small.
/// </para>
/// <para>
/// This lives in <c>Meshmakers.Common</c> rather than in an OctoMesh assembly because there is
/// nothing OctoMesh about it: it reads <see cref="Activity.Current" /> and formats hex. That also
/// makes it reachable from the SDK-based processes — the communication operator and the Loxone and
/// SAP adapters — which carry none of the <c>Meshmakers.Octo.Services.*</c> packages.
/// </para>
/// <para>
/// Registered declaratively from <c>nlog.config</c> via
/// <c>&lt;extensions&gt;&lt;add assembly="Meshmakers.Common.Observability" /&gt;</c>. NLog resolves
/// renderers from the assembly it is told about, not transitively, so that name has to match this
/// assembly exactly. Registering in code instead would have to happen before each process parses
/// its configuration file, which for this estate means before a call in sixteen separate entry
/// points — and an unregistered renderer does not fail, it renders as literal text.
/// </para>
/// <para>
/// Deliberately NOT marked <c>[ThreadAgnostic]</c>. <see cref="Activity.Current" /> is async-local
/// state; telling NLog the value is context-free would let an async target render it on a pool
/// thread that has no activity, which yields an empty id instead of an error.
/// </para>
/// </remarks>
[LayoutRenderer("otel-trace-id")]
public sealed class OtelTraceIdLayoutRenderer : LayoutRenderer
{
    protected override void Append(StringBuilder builder, LogEventInfo logEvent)
    {
        var activity = Activity.Current;
        if (activity is not null)
        {
            builder.Append(activity.TraceId.ToHexString());
        }
    }
}

/// <summary>
/// Renders the span id of the ambient <see cref="Activity" /> as 16 lowercase hex characters,
/// or nothing when no activity is current.
/// </summary>
/// <remarks>See <see cref="OtelTraceIdLayoutRenderer" /> for why this lives here.</remarks>
[LayoutRenderer("otel-span-id")]
public sealed class OtelSpanIdLayoutRenderer : LayoutRenderer
{
    protected override void Append(StringBuilder builder, LogEventInfo logEvent)
    {
        var activity = Activity.Current;
        if (activity is not null)
        {
            builder.Append(activity.SpanId.ToHexString());
        }
    }
}
