# CLAUDE.md

Guidance for Claude Code in `mm-common` (Meshmakers.Common.* NuGet libraries: CommandLineParser, Configuration,
Metrics, Observability, Shared). Consumed by almost every OctoMesh service, so a change here reaches all of them.

## Build and test

```bash
dotnet build Meshmakers.Common.sln -c DebugL      # DebugL packs 999.0.0 for the local chain
# xUnit v3 test projects are executables; run them directly
./test/MetricsTests/bin/DebugL/net10.0/Meshmakers.Common.MetricsTests
```

Nullable is on and warnings are errors.

## Metrics: collections are bounded by names, never by runs (AB#7010)

`Meshmakers.Common.Metrics` is called from hot paths: the engine wraps every runtime query in a `RuntimeMeter`
(`IMetricsContext.CreateRuntimeMeter`). No service starts the publisher (`MetricsContext.InitProvider`), so nothing
ever pops the collected data.

Rule: `RuntimeDataCollection` keeps one **aggregate** (`RuntimeResult`: calls, min, max, average per checkpoint) per
meter name, and never the individual `Checkpoint` objects of the runs. Memory is bounded by the number of meter and
checkpoint names, whatever the number of runs.

It used to append every checkpoint of every run to a `ConcurrentBag` until somebody popped it. The energyiq mesh
adapter on prod-1 (about 4000 pipeline executions per hour) retained 1.6 GiB of managed heap in three days
(gen2 22 -> 1663 MiB, LOH 27 -> 429 MiB), found with `dotnet-gcdump`: `Checkpoint` objects and their bag arrays.

A new collection in a hot path must say how it is bounded, and needs a test that adds many runs and asserts the heap
(or a `WeakReference` to a run's object) does not grow. See `RuntimeDataCollectionTests`.
