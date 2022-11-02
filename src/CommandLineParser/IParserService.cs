namespace Meshmakers.Common.CommandLineParser;

public interface IParserService : IArgumentParser
{
    /// <summary>
    ///     Adds a sample
    /// </summary>
    /// <param name="sample">Sample code</param>
    /// <param name="description">Description to sample</param>
    void AddSample(string sample, string description);

    /// <summary>
    ///     Adds a sample
    /// </summary>
    /// <param name="codeSample">Sample object with sample code and description</param>
    void AddSample(CodeSample codeSample);

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
