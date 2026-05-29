namespace Meshmakers.Common.CommandLineParser;

/// <summary>
///     All documentation a command surfaces for the user. Returned by
///     <see cref="Commands.Command{TOptions}.GetDocumentation" />. CLI help (via
///     <see cref="ParserService" />) renders only <see cref="Samples" />; <see cref="Notes" /> and
///     <see cref="SeeAlso" /> are exposed for external documentation generators to consume — this
///     library does not render them itself.
/// </summary>
/// <param name="Samples">Invocation samples. <see cref="ParserService" /> renders them in the SAMPLES section of CLI help.</param>
/// <param name="Notes">Free-form notes about the command. Intended for external documentation generators; CLI help does not render this.</param>
/// <param name="SeeAlso">Links to related documentation. Intended for external documentation generators; CLI help does not render this.</param>
public sealed record CommandDocumentation(
    IEnumerable<CodeSample>? Samples = null,
    IEnumerable<string>? Notes = null,
    IEnumerable<SeeAlsoLink>? SeeAlso = null);
