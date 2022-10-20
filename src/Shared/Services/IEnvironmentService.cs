namespace Meshmakers.Common.Shared.Services;

/// <summary>
/// </summary>
public interface IEnvironmentService
{
    /// <summary>
    ///     Returns a string array containing the command-line arguments for the current process.
    /// </summary>
    /// <returns>
    ///     An array of string where each element contains a command-line argument. The first element is the executable
    ///     file name, and the following zero or more elements contain the remaining command-line arguments.
    /// </returns>
    string[] GetCommandLineArgs();
}