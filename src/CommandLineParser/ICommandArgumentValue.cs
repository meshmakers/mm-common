namespace Meshmakers.Common.CommandLineParser;

public interface ICommandArgumentValue : IArgumentParser
{
    string CommandValue { get; }
    string CommandDescription { get; }

    /// <summary>
    ///     Compares a string to the short and long parameter value
    /// </summary>
    /// <param name="value">String</param>
    /// <returns></returns>
    bool Compare(string value);

    /// <summary>
    ///     Returns a value of an optional argument
    /// </summary>
    /// <param name="argument">Argument</param>
    /// <typeparam name="T">Type of scalar</typeparam>
    /// <returns>Value or default otherwise</returns>
    T? GetArgumentScalarValueOrDefault<T>(IArgument argument);

    /// <summary>
    ///     Gets a mandatory scalar value
    /// </summary>
    /// <param name="argument">Argument object</param>
    /// <typeparam name="T">Type of scalar</typeparam>
    /// <returns>The value</returns>
    T? GetArgumentScalarValue<T>(IArgument argument);
}