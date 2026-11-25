# AGENTS.md

Instructions for coding agents in this repository. Detail lives in `docs/`; read the matching file before changing the
code it covers.

## What this repo is

Guidance for Claude Code in `mm-common` (Meshmakers.Common.* NuGet libraries: CommandLineParser, Configuration,
Metrics, Observability, Shared). Consumed by almost every OctoMesh service, so a change here reaches all of them.

## Read before you change

<!-- >>> generated: routing -->
| When you change | Read first |
|---|---|
| `src/Metrics/**`, `test/MetricsTests/**` | `docs/metrics-collections.md` |
<!-- <<< end generated: routing -->

## Build & test

```bash
dotnet build Meshmakers.Common.sln -c DebugL      # DebugL packs 999.0.0 for the local chain
# xUnit v3 test projects are executables; run them directly
./test/MetricsTests/bin/DebugL/net10.0/Meshmakers.Common.MetricsTests
```

Nullable is on and warnings are errors.

## Before you commit

Build and run the tests as described above.

## Rules

The rule for hot-path collections is in `docs/metrics-collections.md`.
