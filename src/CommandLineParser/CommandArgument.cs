using Meshmakers.Common.Shared.Services;

namespace Meshmakers.Common.CommandLineParser;

internal class CommandArgument : Argument, ICommandArgument
{
    private readonly Dictionary<string, ICommandArgumentValue> _commandArgumentValues;

    internal CommandArgument(string shortTerm, string longTerm, string[] description, bool isMandatoryArgument)
        : base(shortTerm, longTerm, description, isMandatoryArgument, 1, false)
    {
        _commandArgumentValues = new Dictionary<string, ICommandArgumentValue>();
    }

    public IEnumerable<ICommandArgumentValue> CommandValues => _commandArgumentValues.Values;

    public void AddCommandValue(ICommandArgumentValue commandArgumentValue)
    {
        if (_commandArgumentValues.ContainsKey(commandArgumentValue.Value))
        {
            throw new InvalidParameterException(
                $"Command value ‘{commandArgumentValue.Value}' already defined.");
        }

        _commandArgumentValues.Add(commandArgumentValue.Value, commandArgumentValue);
    }

    public bool TryGetCommandValue(string value, out ICommandArgumentValue? commandArgumentValue)
    {
        commandArgumentValue = null;

        foreach (var argumentValue in _commandArgumentValues.Values)
        {
            if (argumentValue.Compare(value))
            {
                commandArgumentValue = argumentValue;
                return true;
            }
        }

        return false;
    }

    public override void ShowUsage(int emptySpacesOnStartCount, IConsoleService consoleService)
    {
        base.ShowUsage(emptySpacesOnStartCount, consoleService);

        var prefix = "".PadRight(emptySpacesOnStartCount + Constants.TabCount);
        consoleService.WriteLineRegardSpace($"{prefix}Possible commands:");

        ShowGroupUsage(emptySpacesOnStartCount, consoleService, null, true);
    }

    public void ShowGroupUsage(int emptySpacesOnStartCount, IConsoleService consoleService, string? groupName,
        bool includeArguments)
    {
        var newEmptySpacesOnStartCount = emptySpacesOnStartCount + Constants.TabCount;
        var prefix = "".PadRight(newEmptySpacesOnStartCount);

        var commandArgumentValues = _commandArgumentValues.Values
            .Where(x => groupName == null || string.Equals(x.Group, groupName, StringComparison.OrdinalIgnoreCase));

        foreach (var commandArgumentValuesGroup in commandArgumentValues.GroupBy(x=> x.Group))
        {
            consoleService.WriteLine("");
            consoleService.WriteLine(commandArgumentValuesGroup.Key.ToUpper());
            consoleService.WriteLine("");

            foreach (var commandArgumentValue in commandArgumentValuesGroup.OrderBy(a=> a.Value))
            {
                consoleService.WriteColumnLine($"{prefix}{commandArgumentValue.Value}:", Constants.UsageNameLength,
                    commandArgumentValue.Description);

                if (!includeArguments)
                {
                    continue;
                }

                commandArgumentValue.ShowLayerUsage(newEmptySpacesOnStartCount + Constants.TabCount, consoleService);
                consoleService.WriteLine("");
            }
        }
    }
}
