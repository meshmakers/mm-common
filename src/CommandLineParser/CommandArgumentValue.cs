using Meshmakers.Common.Shared;

namespace Meshmakers.Common.CommandLineParser;

public class CommandArgumentValue : ArgumentParser, ICommandArgumentValue
{
    public CommandArgumentValue(string commandGroup, string commandValue, string commandDescription)
    {
        Group = commandGroup;
        Value = commandValue;
        Description = commandDescription;

        // Every command understands the help flag, so "-c <command> --help" can show the help of that
        // single command instead of the full usage of the tool.
        AddHelpArgument();
    }

    public string Group { get; }
    public string Value { get; }
    public string Description { get; }

    /// <summary>
    ///     Compares a string to the short and long parameter value
    /// </summary>
    /// <param name="value">String</param>
    /// <returns></returns>
    public bool Compare(string value)
    {
        ArgumentValidation.ValidateString(nameof(value), value);

        return string.Compare(Value, value, StringComparison.OrdinalIgnoreCase) == 0;
    }

    /// <summary>
    ///     Returns a value of an optional argument
    /// </summary>
    /// <param name="argument">Argument</param>
    /// <typeparam name="T">Type of scalar</typeparam>
    /// <returns>Value or default otherwise</returns>
    public T? GetArgumentScalarValueOrDefault<T>(IArgument argument)
    {
        if (IsArgumentUsed(argument))
        {
            var nameArgData = GetArgumentValue(argument);
            return nameArgData.GetValue<T>();
        }

        return default;
    }

    /// <summary>
    ///     Gets a mandatory scalar value
    /// </summary>
    /// <param name="argument">Argument object</param>
    /// <typeparam name="T">Type of scalar</typeparam>
    /// <returns>The value</returns>
    public T GetArgumentScalarValue<T>(IArgument argument)
    {
        var nameArgData = GetArgumentValue(argument);
        var value = nameArgData.GetValue<T>();
        if (value == null)
        {
            throw new InvalidParameterException($"Value of argument '{argument.LongTerm}' cannot be null.");
        }

        return value;
    }
}
