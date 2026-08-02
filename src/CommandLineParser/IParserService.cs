namespace Meshmakers.Common.CommandLineParser;

public interface IParserService : IArgumentParser
{
    /// <summary>
    ///     Registers a sample for inclusion in <see cref="ShowUsageInformation" />.
    ///     The invocation string is composed at render time from <paramref name="commandArgument" />'s
    ///     live <see cref="IArgument.ShortTerm" />, <paramref name="commandValue" />, and the live
    ///     argument definitions referenced by <paramref name="codeSample" />. Sourcing the
    ///     selector flag from the argument (rather than a hard-coded literal) keeps every rendered
    ///     sample in sync if the command-selector short term ever changes.
    /// </summary>
    /// <param name="commandArgument">The argument representing the command selector (e.g. <c>-c</c> from <c>AddCommandArgument</c>).</param>
    /// <param name="commandValue">The command verb (matches <see cref="ICommandArgumentValue.Value" />).</param>
    /// <param name="codeSample">The sample carrying ordered argument bindings and its description.</param>
    void AddSample(IArgument commandArgument, string commandValue, CodeSample codeSample);

    /// <summary>
    ///     Shows all possible commands, arguments and corresponding descriptions
    ///     with samples
    /// </summary>
    /// <param name="applicationExeName">Name of application executable name</param>
    void ShowUsageInformation(string applicationExeName);

    /// <summary>
    ///     Shows the arguments, samples and notes of a single command, so the caller does not have to scroll
    ///     through the usage of the whole tool. Only the samples registered for
    ///     <paramref name="commandArgumentValue" /> are rendered.
    /// </summary>
    /// <param name="applicationExeName">Name of application executable name</param>
    /// <param name="commandArgument">The argument representing the command selector (e.g. <c>-c</c>).</param>
    /// <param name="commandArgumentValue">The command whose help is shown.</param>
    /// <param name="documentation">Optional documentation of the command; its notes are rendered.</param>
    void ShowCommandUsageInformation(string applicationExeName, IArgument commandArgument,
        ICommandArgumentValue commandArgumentValue, CommandDocumentation? documentation);

    /// <summary>
    ///     Parses and validates provided command line args
    /// </summary>
    /// <exception cref="ParserException"></exception>
    void ParseAndValidate();
}
