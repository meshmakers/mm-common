# Meshmakers.Common

Foundational .NET libraries shared across Meshmakers and OctoMesh services. The repository publishes a set of small, dependency-light NuGet packages multi-targeting `net8.0`/`net9.0`/`net10.0` (with `netstandard2.0` for `Shared`).

## Published packages

- **Meshmakers.Common.Shared** - common primitives and helpers: extension methods for strings, enumerables, dates, types, URIs and exceptions; `ArgumentValidation` guards; `MmPath` path normalization; `RandomGenerator`; INI file reading/writing (`IniFile`); a localization-provider abstraction (`ILocalizationProvider`); MEF-based composition helpers (`CompositionExtensions`); abstracted services for compression, console and environment access (`ICompressionService`, `IConsoleService`, `IEnvironmentService`); and a small dependency-aware task runner (`TaskEngine` / `TaskBase`). Targets `net8.0`, `net9.0` and `netstandard2.0`.
- **Meshmakers.Common.Configuration** - persists strongly-typed `IOptions<T>` settings to a per-user `settings.json` under the user profile via `IConfigWriter` / `ConfigWriter`. Built on `Microsoft.Extensions.Options` and `System.IO.Abstractions`.
- **Meshmakers.Common.CommandLineParser** - command/argument parser for console applications. `IParserService` parses and validates arguments, models commands and their arguments, renders usage information, and supports registered code samples. `ICommandParser` additionally serves an implicit help flag (`--help`, `-?`, and `-h` unless the command declares `-h` itself) that narrows the output down in three levels: on its own it lists the command groups, followed by a group name (`--help "Identity Services"`, prefixes and lower case accepted) it lists that group's commands, and together with a command (`-c <command> --help`) it renders that command's arguments, samples and notes. The reserved topic `--help all` still prints the full listing. Built on `Microsoft.Extensions.Logging.Abstractions` and `Microsoft.Extensions.Options`.
- **Meshmakers.Common.Metrics** - runtime measurement and Prometheus publishing. `IMetricsContext` creates scoped `IRuntimeMeter` instances with named checkpoints and exposes results, optionally publishing them on a Prometheus metrics endpoint (default port `9184`). Built on `prometheus-net` and `prometheus-net.AspNetCore`.

## Project structure

- `src/` - the four packable libraries plus `CommandLineParser.Sample`, a runnable console example.
- `test/` - xUnit-style test projects for each library (`SharedTests`, `ConfigurationTests`, `CommandLineParserTests`, `MetricsTests`).

## Build

```bash
dotnet build Meshmakers.Common.sln
```

## Test

```bash
dotnet test Meshmakers.Common.sln
```

## Documentation

The complete OctoMesh documentation is available at https://docs.meshmakers.cloud.

## License

Released under the MIT License.
