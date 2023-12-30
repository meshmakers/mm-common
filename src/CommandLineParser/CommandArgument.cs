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

        var newEmptySpacesOnStartCount = emptySpacesOnStartCount + Constants.TabCount;
        var prefix = "".PadRight(newEmptySpacesOnStartCount);
        consoleService.WriteLineRegardSpace($"{prefix}Possible commands:");

        foreach (var argumentValue in _commandArgumentValues.Values)
        {
            consoleService.WriteColumnLine($"{prefix}{argumentValue.Value}:", Constants.UsageNameLength,
                argumentValue.Description);

            argumentValue.ShowLayerUsage(newEmptySpacesOnStartCount + Constants.TabCount, consoleService);
            consoleService.WriteLine("");
        }
    }
}
