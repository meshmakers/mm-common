namespace Meshmakers.Common.CommandLineParser;

/// <summary>
///     All documentation a command surfaces for the user. Returned by
///     <see cref="Commands.Command{TOptions}.GetDocumentation" /> and consumed by the CLI help renderer
///     (samples only) and the external documentation generator (all sections).
/// </summary>
/// <param name="Samples">Invocation samples. Rendered in the SAMPLES section of CLI help and the Examples section of generated docs.</param>
/// <param name="Notes">Free-form notes about the command. Documentation-only; CLI help does not render these.</param>
public sealed record CommandDocumentation(
    IEnumerable<CodeSample>? Samples = null,
    IEnumerable<string>? Notes = null);
