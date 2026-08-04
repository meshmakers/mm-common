namespace Meshmakers.Common.CommandLineParser;

internal static class Constants
{
    internal const int TabCount = 2;
    internal const int UsageNameLength = 45;

    /// <summary>
    ///     Help topic reserved for the full usage listing, so that the bare help flag can show the compact
    ///     group overview instead. A command group of this name would be unreachable — it is listed among the
    ///     known topics on an unknown topic, which makes the name visibly taken.
    /// </summary>
    internal const string AllCommandsHelpTopic = "all";
}
