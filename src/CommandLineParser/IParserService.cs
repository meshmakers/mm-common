namespace Meshmakers.Common.CommandLineParser;

public interface IParserService : IArgumentParser
{
    void AddSample(string sample, string description);

    void ShowUsageInformation(string applicationExeName);

    /// <summary>
    ///     Parses and validates command line args
    /// </summary>
    /// <param name="arguments">CommandValue line args as string array</param>
    /// <exception cref="ParserException"></exception>
    void ParseAndValidate(string[] arguments);

    /// <summary>
    ///     Parses and validates provided command line args
    /// </summary>
    /// <exception cref="ParserException"></exception>
    void ParseAndValidate();
}