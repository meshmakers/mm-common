using System.Collections.Generic;

namespace Meshmakers.Common.CommandLineParser;

public interface ICommandArgument : IArgument
{
    IEnumerable<CommandArgumentValue> CommandValues { get; }

    ICommandArgumentValue AddCommandValue(string commandValue, string commandDescription);

    bool TryGetCommandValue(string value, out ICommandArgumentValue? commandArgumentValue);
}