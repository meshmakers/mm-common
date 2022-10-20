using System;
using Meshmakers.Common.Shared;

namespace Meshmakers.Common.CommandLineParser;

public class CommandArgumentValue : ArgumentParser, ICommandArgumentValue
{
    public CommandArgumentValue(string commandValue, string commandDescription)
    {
        CommandValue = commandValue;
        CommandDescription = commandDescription;
    }

    public string CommandValue { get; }
    public string CommandDescription { get; }

    /// <summary>
    ///     Compares a string to the short and long parameter value
    /// </summary>
    /// <param name="value">String</param>
    /// <returns></returns>
    public bool Compare(string value)
    {
        ArgumentValidation.ValidateString(nameof(value), value);

        return string.Compare(CommandValue, value, StringComparison.OrdinalIgnoreCase) == 0;
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
        return nameArgData.GetValue<T>();
    }
}