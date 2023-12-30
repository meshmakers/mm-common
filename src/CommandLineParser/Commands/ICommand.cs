namespace Meshmakers.Common.CommandLineParser.Commands;

/// <summary>
///     Interface for commands
/// </summary>
public interface ICommand
{
    /// <summary>
    ///     Returns the object describing the corresponding command argument.
    /// </summary>
    public ICommandArgumentValue CommandArgumentValue { get; }

    /// <summary>
    ///     Returns optional samples for the command.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<CodeSample>? GetSamples();

    /// <summary>
    ///     Validates preconditions for command execution, for example access keys.
    /// </summary>
    /// <returns></returns>
    Task PreValidate();

    /// <summary>
    ///     Executed the command.
    /// </summary>
    /// <returns></returns>
    Task Execute();
}
