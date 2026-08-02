using Meshmakers.Common.Shared;

namespace Meshmakers.Common.CommandLineParser;

/// <summary>
///     The implicit help flag a layer understands once <see cref="IArgumentParser.AddHelpArgument" /> has been
///     called. It is deliberately kept outside the layer's argument dictionary: it is matched only after every
///     declared argument had the chance to claim the term, so a command declaring its own <c>-h</c> (for example
///     <c>--host</c>) keeps that meaning and its help stays reachable via <c>--help</c> or <c>-?</c>.
/// </summary>
internal sealed class HelpArgument : Argument
{
    private static readonly string[] Terms =
        ["-h", "--h", "/h", "-help", "--help", "/help", "-?", "--?", "/?"];

    internal HelpArgument()
        : base("?", "help",
            [
                "Shows this help. Combined with a command it shows the help of that command only",
                "Also accepted as -h, unless the command uses -h for an argument of its own"
            ],
            false, 0, false)
    {
    }

    /// <summary>
    ///     Matches the fixed set of help terms exactly. The base implementation compares prefixes, which would let
    ///     help swallow unrelated terms such as <c>-hostname</c>.
    /// </summary>
    /// <param name="value">The command line term to test.</param>
    /// <returns>True when the term is one of the help terms.</returns>
    public override bool Compare(string value)
    {
        ArgumentValidation.ValidateString(nameof(value), value);

        return Terms.Contains(value, StringComparer.OrdinalIgnoreCase);
    }
}
