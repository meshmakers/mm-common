namespace Meshmakers.Common.CommandLineParser.Commands;

public interface ICommandParser
{
    /// <summary>
    ///     Shows all possible commands, arguments and corresponding descriptions
    ///     with samples
    /// </summary>
    /// <param name="applicationExeName">Name of application executable name</param>
    void ShowUsageInformation(string applicationExeName);

    /// <summary>
    ///     Parses command line parameters and validates them
    /// </summary>
    /// <returns></returns>
    Task ParseAndValidateAsync();
}
