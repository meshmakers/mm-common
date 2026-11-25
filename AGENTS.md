# AGENTS.md

Instructions for coding agents in this repository. Detail lives in `docs/`; read the matching file before changing the
code it covers.

## What this repo is

`mm-common` holds the `Meshmakers.Common.*` NuGet libraries (`net10.0`; `Shared` also targets `netstandard2.0`):
`Shared` (extensions, guards, task runner), `Configuration` (`settings.json` writer), `CommandLineParser`,
`Metrics` (runtime meters, Prometheus), `Observability` (trace correlation for NLog), plus the runnable
`CommandLineParser.Sample`. Almost every OctoMesh service consumes these packages, so a change here reaches all of
them. The repository is public.

## Read before you change

<!-- >>> generated: routing -->
| When you change | Read first |
|---|---|
| `src/Metrics/**`, `test/MetricsTests/**` | `docs/metrics-collections.md` |
<!-- <<< end generated: routing -->

## Build & test

Build locally with `-c DebugL`: it sets the version `999.0.0` and packs every library into its own `bin/DebugL`
folder (nothing is copied to a shared feed). The pipeline `devops-build/azure-pipelines.yml` builds and tests with
`Release`, sets the version from the build number, and publishes the packages (tags `r*` go to the public and the
private feed, `main` to the private feed; `devops-build/push-nuget-package.yml:15`).

```bash
dotnet build Meshmakers.Common.sln -c DebugL
dotnet test Meshmakers.Common.sln -c DebugL
# xUnit v3 test projects are executables; one project can be run directly
./test/MetricsTests/bin/DebugL/net10.0/Meshmakers.Common.MetricsTests
```

## Before you commit

1. `dotnet build Meshmakers.Common.sln -c DebugL` with 0 warnings: nullable is on and warnings are errors
   (`Directory.Build.props`).
2. `dotnet test Meshmakers.Common.sln -c DebugL`: all five test projects green.
3. Changed behaviour of a library: update its paragraph in `README.md` in the same commit.
4. Touched `AGENTS.md` or a file in `docs/`: `Test-OctoAgentDocs -Fix` (octo-tools) reports 0 errors. It rewrites the
   generated routing table, so run it before you stage.

## Rules

- **This repository is public.** No work item ids, internal hostnames, customer or tenant names, or secrets in docs
  and in new or changed code comments and test names; remove an existing one when you touch its line (six `AB#`
  comments remain under `src/` and `test/`).
- **A collection in a hot path is bounded by names, never by runs.** `RuntimeDataCollection` keeps one aggregate per
  meter name (`src/Metrics/Context/RuntimeDataCollection.cs:28`); a new collection says how it is bounded and comes
  with a test that adds many runs and checks that none stays reachable
  (`RuntimeDataCollectionTests.Add_ManyRuns_RetainsNoneOfTheirCheckpointsAndCountsEveryRun`). Exception: the
  checkpoints of one run live as long as that run (`src/Metrics/Meters/Data/RuntimeData.cs:7`). Detail:
  `docs/metrics-collections.md`.
- **New code builds without warnings** (`Directory.Build.props:5`, `TreatWarningsAsErrors`): fix a warning, never add
  `#pragma warning disable` or `NoWarn` (none exists today). ReSharper inspection suppressions are editor settings,
  not build warnings, and this rule does not cover them.
- **New code in `Shared` compiles for `netstandard2.0` and `net10.0`** (`src/Shared/Shared.csproj:9`; the build
  compiles both targets). The other libraries target `net10.0` only.
- **Internal types are tested through `InternalsVisibleTo` in the library project** (`src/Metrics/Metrics.csproj:14`,
  `src/CommandLineParser/CommandLineParser.csproj:14`, `src/Observability/Observability.csproj:16`), not by making
  them public (add the entry if the project has none yet; `Shared` and `Configuration` have none). A new behaviour
  gets its test in the matching `test/<Library>Tests` project.
