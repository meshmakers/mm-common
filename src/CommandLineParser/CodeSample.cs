using Meshmakers.Common.Shared;

namespace Meshmakers.Common.CommandLineParser;

/// <summary>
///     A single invocation example for a command. The sample is described as an ordered list of
///     <see cref="CodeSampleArgument" /> bindings; the rendered string is composed at format time from the
///     live argument definitions, so renaming a short or long term in <c>AddArgument(...)</c> updates every
///     sample referring to it automatically.
/// </summary>
public class CodeSample
{
    /// <summary>
    ///     Primary constructor: a sample described as a list of argument bindings plus a one-line description.
    ///     Optionally carries an expected stdout fragment which the documentation generator renders as an
    ///     output block; CLI help ignores it.
    /// </summary>
    public CodeSample(IEnumerable<CodeSampleArgument> arguments, string description, string? expectedOutput = null)
    {
        ArgumentValidation.Validate(nameof(arguments), arguments);
        ArgumentValidation.ValidateString(nameof(description), description);

        Arguments = arguments.ToList();
        Description = description;
        ExpectedOutput = expectedOutput;
    }

    /// <summary>
    ///     The arguments invoked in this sample, in the order they should appear after the command verb.
    /// </summary>
    public IReadOnlyList<CodeSampleArgument> Arguments { get; }

    /// <summary>
    ///     One-line description of the sample. Rendered both in CLI help (under SAMPLES) and in generated docs.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Optional expected stdout fragment. The documentation generator renders this as an <c>**Output:**</c>
    ///     fenced block; CLI help ignores it (consistent with the conventions of other CLIs like <c>kubectl</c>).
    /// </summary>
    public string? ExpectedOutput { get; }
}
