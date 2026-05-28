namespace Meshmakers.Common.CommandLineParser;

public interface IParserService : IArgumentParser
{
    /// <summary>
    ///     Registers a sample for inclusion in <see cref="ShowUsageInformation" />.
    ///     The invocation string is composed at render time from <paramref name="commandVerb" />,
    ///     the application name, and the live argument definitions referenced by <paramref name="codeSample" />.
    /// </summary>
    /// <param name="commandVerb">The command verb (matches <see cref="ICommandArgumentValue.Value" />).</param>
    /// <param name="codeSample">The sample carrying ordered argument bindings and its description.</param>
    void AddSample(string commandVerb, CodeSample codeSample);

    /// <summary>
    ///     Shows all possible commands, arguments and corresponding descriptions
    ///     with samples
    /// </summary>
    /// <param name="applicationExeName">Name of application executable name</param>
    void ShowUsageInformation(string applicationExeName);

    /// <summary>
    ///     Parses and validates provided command line args
    /// </summary>
    /// <exception cref="ParserException"></exception>
    void ParseAndValidate();
}
