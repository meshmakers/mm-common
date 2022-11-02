using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Meshmakers.Common.Shared;

namespace Meshmakers.Common.CommandLineParser;

/// <summary>
///     Implements an data holder
/// </summary>
internal class ArgumentValue : IArgumentValue
{
    private readonly List<string> _dataList;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="argument">The assigned argument definition</param>
    internal ArgumentValue(IArgument argument)
    {
        Argument = argument;
        _dataList = new List<string>();
    }

    /// <summary>
    ///     Returns the argument definition object
    /// </summary>
    public IArgument Argument { get; }

    /// <summary>
    ///     List of values of the argument
    /// </summary>
    public ReadOnlyCollection<string> Values => new(_dataList);

    /// <summary>
    ///     Returns the cast value at the given position. If no value is found on the given
    ///     position, an exception is thrown.
    /// </summary>
    /// <typeparam name="T">Type the argument value have to be cast</typeparam>
    /// <param name="index">Position of the argument value, the default value is 0</param>
    /// <returns>The value cast to the defined type.</returns>
    /// <exception cref="KeyNotFoundException">Exception, thrown if no value at the given position hasn't been found.</exception>
    /// <exception cref="FormatException">Exception, thrown if the cast to the given type is not possible</exception>
    public T? GetValue<T>(int index = 0)
    {
        ArgumentValidation.ValidateInt(nameof(index), index, 0);

        return GetValue<T>(index,
            () => throw new KeyNotFoundException($"There is no argument value at position \"{index}\"!"));
    }

    /// <summary>
    ///     Returns the cast value of the first position
    /// </summary>
    /// <typeparam name="T">Type the argument value have to be cast</typeparam>
    /// <param name="defaultValue">The default value, if no value has been found.</param>
    /// <returns>The value cast to the defined type.</returns>
    /// <exception cref="FormatException">Exception, thrown if the cast to the given type is not possible</exception>
    public T GetValue<T>(T defaultValue)
    {
        return GetValue(0, () => defaultValue);
    }

    /// <summary>
    ///     Returns the cast value of the given position
    /// </summary>
    /// <typeparam name="T">Type the argument value have to be cast</typeparam>
    /// <param name="index">Position of the argument value</param>
    /// <param name="defaultValue">The default value, if no value has been found.</param>
    /// <returns>The value cast to the defined type.</returns>
    /// <exception cref="FormatException">Exception, thrown if the cast to the given type is not possible</exception>
    public T GetValue<T>(int index, T defaultValue)
    {
        ArgumentValidation.ValidateInt(nameof(index), index, 0);

        return GetValue(index, () => defaultValue);
    }

    /// <summary>
    ///     Adds an argument
    /// </summary>
    /// <param name="value">The argument value</param>
    internal void AddValue(string value)
    {
        ArgumentValidation.ValidateString(nameof(value), value);

        _dataList.Add(value);
    }

    private T GetValue<T>(int index, Func<T> notFoundFunction)
    {
        if (Values.Count <= index)
            return notFoundFunction();

        var converter = TypeDescriptor.GetConverter(typeof(T));
        var value = Values[index];
        if (value == null)
            throw new ParserException("Unable to convert value");
        var result = converter.ConvertFromString(value);
        if (result == null)
            throw new ParserException("Unable to convert value");
        return (T)result;
    }
}