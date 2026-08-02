namespace Meshmakers.Common.CommandLineParser;

/// <summary>
///     All documentation a command surfaces for the user. Returned by
///     <see cref="Commands.Command{TOptions}.GetDocumentation" /> and consumed by the CLI help renderer and the
///     external documentation generator.
/// </summary>
/// <param name="Samples">Invocation samples. Rendered in the SAMPLES section of CLI help and the Examples section of generated docs.</param>
/// <param name="Notes">
///     Free-form notes about the command. Rendered in the NOTES section of the per-command CLI help
///     (<c>-c &lt;command&gt; --help</c>) and in generated docs; the full usage of the tool omits them.
/// </param>
public sealed record CommandDocumentation(
    IEnumerable<CodeSample>? Samples = null,
    IEnumerable<string>? Notes = null);
