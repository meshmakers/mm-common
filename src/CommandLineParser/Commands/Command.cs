using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meshmakers.Common.CommandLineParser.Commands;

/// <summary>
///     Base class for commands
/// </summary>
/// <typeparam name="TOptions">Type of options class</typeparam>
public abstract class Command<TOptions> : ICommand
    where TOptions : class
{
    private const string DefaultCommandGroup = "GENERAL";

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="logger">Logger abstraction object</param>
    /// <param name="commandValue">Command key</param>
    /// <param name="commandDescription">Description of command</param>
    /// <param name="options">IOptions instance</param>
    protected Command(ILogger<Command<TOptions>> logger, string commandValue, string commandDescription,
        IOptions<TOptions> options)
    {
        CommandArgumentValue = new CommandArgumentValue(DefaultCommandGroup, commandValue, commandDescription);
        Logger = logger;
        Options = options;
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="logger">Logger abstraction object</param>
    /// <param name="commandGroup">Group of the command, for example, "User Management"</param>
    /// <param name="commandValue">Command key</param>
    /// <param name="commandDescription">Description of command</param>
    /// <param name="options">IOptions instance</param>
    protected Command(ILogger<Command<TOptions>> logger, string commandGroup, string commandValue, string commandDescription,
        IOptions<TOptions> options)
    {
        CommandArgumentValue = new CommandArgumentValue(commandGroup, commandValue, commandDescription);
        Logger = logger;
        Options = options;
    }

    /// <summary>
    ///     Returns the logging abstraction object.
    /// </summary>
    protected ILogger<Command<TOptions>> Logger { get; }

    /// <summary>
    ///     Returns the options object.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    protected IOptions<TOptions> Options { get; }

    /// <summary>
    ///     Returns the object describing the corresponding command argument.
    /// </summary>
    public ICommandArgumentValue CommandArgumentValue { get; }

    /// <summary>
    ///     Returns the documentation for the command — invocation samples, notes, and related-doc links.
    ///     Override to declare per-command documentation alongside the class. Default returns <c>null</c>.
    /// </summary>
    /// <returns></returns>
    public virtual CommandDocumentation? GetDocumentation()
    {
        return null;
    }

    /// <summary>
    ///     Validates preconditions for command execution, for example access keys.
    /// </summary>
    /// <returns></returns>
    public virtual Task PreValidate()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Executed the command.
    /// </summary>
    /// <returns></returns>
    public abstract Task Execute();
}
