using System.Collections.Generic;
using Meshmakers.Common.Shared.Services;

namespace Meshmakers.Common.CommandLineParser;

internal class CommandArgument : Argument, ICommandArgument
{
    private readonly Dictionary<string, CommandArgumentValue> _commandArgumentValues;

    internal CommandArgument(string shortTerm, string longTerm, string[] description, bool isMandatoryArgument)
        : base(shortTerm, longTerm, description, isMandatoryArgument, 1, false)
    {
        _commandArgumentValues = new Dictionary<string, CommandArgumentValue>();
    }

    public IEnumerable<CommandArgumentValue> CommandValues => _commandArgumentValues.Values;

    public ICommandArgumentValue AddCommandValue(string commandValue, string commandDescription)
    {
        var commandArgumentValue = new CommandArgumentValue(commandValue, commandDescription);
        _commandArgumentValues.Add(commandValue, commandArgumentValue);
        return commandArgumentValue;
    }

    public bool TryGetCommandValue(string value, out ICommandArgumentValue? commandArgumentValue)
    {
        commandArgumentValue = null;

        foreach (ICommandArgumentValue argumentValue in _commandArgumentValues.Values)
            if (argumentValue.Compare(value))
            {
                commandArgumentValue = argumentValue;
                return true;
            }

        return false;
    }

    public override void ShowUsage(int emptySpacesOnStartCount, IConsoleService consoleService)
    {
        base.ShowUsage(emptySpacesOnStartCount, consoleService);

        var newEmptySpacesOnStartCount = emptySpacesOnStartCount + Constants.TabCount;
        var prefix = "".PadRight(newEmptySpacesOnStartCount);
        consoleService.WriteLineRegardSpace($"{prefix}Possible commands:");

        foreach (var argumentValue in _commandArgumentValues.Values)
        {
            consoleService.WriteColumnLine($"{prefix}{argumentValue.CommandValue}:", Constants.UsageNameLength,
                argumentValue.CommandDescription);

            argumentValue.ShowLayerUsage(newEmptySpacesOnStartCount + Constants.TabCount, consoleService);
            consoleService.WriteLine("");
        }
    }
}