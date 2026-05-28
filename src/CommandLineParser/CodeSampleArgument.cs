using Meshmakers.Common.Shared;

namespace Meshmakers.Common.CommandLineParser;

/// <summary>
///     Pairs an argument definition with the value that should appear next to it in a <see cref="CodeSample" />.
///     The renderer (CLI help or documentation generator) reads <see cref="IArgument.ShortTerm" /> from the live
///     argument at format time, so renaming the short/long term in <c>AddArgument(...)</c> automatically updates
///     every sample that references the field.
/// </summary>
public sealed class CodeSampleArgument
{
    /// <summary>
    ///     Constructs a sample entry for an argument that takes a value.
    /// </summary>
    public CodeSampleArgument(IArgument argument, string value)
    {
        ArgumentValidation.Validate(nameof(argument), argument);
        ArgumentValidation.ValidateString(nameof(value), value);

        if (argument.MandatoryValuesCount == 0 && !argument.AreOptionalValuesAllowed)
        {
            throw new ArgumentException(
                $"Argument '--{argument.LongTerm}' is a flag and does not accept a value. " +
                "Use the flag-only constructor: new CodeSampleArgument(argument).",
                nameof(value));
        }

        Argument = argument;
        Value = value;
    }

    /// <summary>
    ///     Constructs a sample entry for a flag (an argument that takes no value).
    /// </summary>
    public CodeSampleArgument(IArgument argument)
    {
        ArgumentValidation.Validate(nameof(argument), argument);

        if (argument.MandatoryValuesCount > 0)
        {
            throw new ArgumentException(
                $"Argument '--{argument.LongTerm}' requires a value. " +
                "Use the value-taking constructor: new CodeSampleArgument(argument, \"...\").",
                nameof(argument));
        }

        Argument = argument;
        Value = null;
    }

    public IArgument Argument { get; }

    /// <summary>
    ///     The literal value rendered after the argument flag, or <c>null</c> when the argument is a flag.
    /// </summary>
    public string? Value { get; }
}
