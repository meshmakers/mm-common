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
    ///     Returns the documentation for the command — invocation samples, notes, and related-doc links.
    ///     Override this to declare per-command documentation alongside the class.
    ///     Default implementation returns <c>null</c>, in which case the CLI help and documentation
    ///     generator fall back to argument-only output.
    /// </summary>
    /// <returns></returns>
    public CommandDocumentation? GetDocumentation() => null;

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
