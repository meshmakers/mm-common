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
    ///     Shows the arguments, samples and notes of a single command
    /// </summary>
    /// <param name="applicationExeName">Name of application executable name</param>
    /// <param name="command">The command whose help is shown</param>
    void ShowCommandUsageInformation(string applicationExeName, ICommand command);

    /// <summary>
    ///     Parses command line parameters and validates them, then executes the selected command. When the
    ///     command line asks for help, the corresponding help is shown instead and no command is executed.
    /// </summary>
    /// <param name="applicationExeName">
    ///     Name of the application executable used in help output. Falls back to the name of the running
    ///     assembly when omitted.
    /// </param>
    /// <returns></returns>
    Task ParseAndValidateAsync(string? applicationExeName = null);
}
