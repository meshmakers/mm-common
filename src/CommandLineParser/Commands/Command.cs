using System.Collections.Generic;
using System.Threading.Tasks;
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
        CommandArgumentValue = new CommandArgumentValue(commandValue, commandDescription);
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
    ///     Returns optional samples for the command.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<CodeSample>? GetSamples()
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
