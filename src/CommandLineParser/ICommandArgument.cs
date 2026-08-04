using Meshmakers.Common.Shared.Services;

namespace Meshmakers.Common.CommandLineParser;

public interface ICommandArgument : IArgument
{
    IEnumerable<ICommandArgumentValue> CommandValues { get; }

    void AddCommandValue(ICommandArgumentValue commandArgumentValue);

    bool TryGetCommandValue(string value, out ICommandArgumentValue? commandArgumentValue);

    /// <summary>
    ///     Writes the commands grouped by <see cref="ICommandArgumentValue.Group" />, each group under its own
    ///     heading. Shared by the full usage listing and the help of a single group.
    /// </summary>
    /// <param name="emptySpacesOnStartCount">Amount of empty spaces on start</param>
    /// <param name="consoleService">Console abstraction to write to</param>
    /// <param name="groupName">
    ///     Restricts the output to this group (compared case insensitively); null writes every group.
    /// </param>
    /// <param name="includeArguments">
    ///     When true every command is followed by its arguments, as in the full usage listing. When false only
    ///     the command and its description are written, which keeps a large group readable.
    /// </param>
    void ShowGroupUsage(int emptySpacesOnStartCount, IConsoleService consoleService, string? groupName,
        bool includeArguments);
}
