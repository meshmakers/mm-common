using System;
using System.IO;
using JetBrains.Annotations;

namespace Meshmakers.Common.Shared;

/// <summary>
///     Helper class to support argument validation
/// </summary>
public static class ArgumentValidation
{
    /// <summary>
    ///     Validates a string
    /// </summary>
    /// <typeparam name="T">The expected object type</typeparam>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="value">Value of the parameter</param>
    public static T? ValidateAndCastToObject<T>([InvokerParameterName] string parameterName, object value)
        where T : class
    {
        Validate<T>(parameterName, value);
        return value as T;
    }

    /// <summary>
    ///     Validates a the type of object
    /// </summary>
    /// <typeparam name="T">The expected object type</typeparam>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="value">Value of the parameter</param>
    public static void Validate<T>([InvokerParameterName] string parameterName, object value)
    {
        if (!(value is T))
        {
            throw new ArgumentOutOfRangeException(parameterName,
                $@"The object of argument '{parameterName}' does not match the expected type '{value.GetType().FullName}'. Expected was object type '{typeof(T).FullName}'");
        }
    }

    /// <summary>
    ///     Validates a string
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="value">Value of the parameter</param>
    #if NETSTANDARD2_0  
    public static void ValidateString([InvokerParameterName] string parameterName, string? value)
    #else
    public static void ValidateString([InvokerParameterName] string parameterName, [System.Diagnostics.CodeAnalysis.NotNull] string? value)
    #endif
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    /// <summary>
    ///     Validates a string
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="value">Value of the parameter</param>
    /// <param name="length">Length of value</param>
#if NETSTANDARD2_0   
    public static void ValidateStringAndLength([InvokerParameterName] string parameterName, string? value, uint length)
#else 
    public static void ValidateStringAndLength([InvokerParameterName] string parameterName, [System.Diagnostics.CodeAnalysis.NotNull] string? value, uint length)
#endif

    {
        ValidateString(parameterName, value, length, length);
    }

    /// <summary>
    ///     Validates a string
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="value">Value of the parameter</param>
    /// <param name="minLength">Minimal length of value</param>
    /// <param name="maxLength">Maximal length of value</param>
    // ReSharper disable once MemberCanBePrivate.Global
    #if NETSTANDARD2_0
        public static void ValidateString([InvokerParameterName] string parameterName, string? value, uint? minLength,
            uint? maxLength)
    #else 
        public static void ValidateString([InvokerParameterName] string parameterName, [System.Diagnostics.CodeAnalysis.NotNull] string? value, uint? minLength,
        uint? maxLength)
    #endif
    {
        if (string.IsNullOrEmpty(value) || value == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        if (minLength.HasValue && value.Length < minLength)
        {
            throw new ArgumentOutOfRangeException(parameterName,
                $@"The value is limited to a minimum of {minLength} characters");
        }

        if (maxLength.HasValue && value.Length > maxLength)
        {
            throw new ArgumentOutOfRangeException(parameterName,
                $@"The value is limited to a maximum of {maxLength} characters");
        }
    }

    /// <summary>
    ///     Validates an integer
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="value">Value of the parameter</param>
    /// <param name="minValue">Minimal allowed value</param>
    /// <param name="maxValue">Maximal allowed value</param>
    public static void ValidateInt([InvokerParameterName] string parameterName, int value, int minValue, int maxValue)
    {
        if (value < minValue)
        {
            throw new ArgumentOutOfRangeException(parameterName,
                $@"The value is smaller than {minValue} signs");
        }

        if (value > maxValue)
        {
            throw new ArgumentOutOfRangeException(parameterName,
                $@"The value is greater than {maxValue} signs");
        }
    }

    /// <summary>
    ///     Validates an integer
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="value">Value of the parameter</param>
    /// <param name="minValue">Minimal allowed value</param>
    public static void ValidateInt([InvokerParameterName] string parameterName, int value, int minValue)
    {
        if (value < minValue)
        {
            throw new ArgumentOutOfRangeException(parameterName,
                $@"The value is smaller than {minValue} signs");
        }
    }

    /// <summary>
    ///     Validates an GUID
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="value">Value of the parameter</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void Validate([InvokerParameterName] string parameterName, Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentOutOfRangeException(parameterName, @"The value is empty");
        }
    }


    /// <summary>
    ///     Validates a file path (It is checked if the file exists)
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="filePath">The file path to validate</param>
    /// <exception cref="ArgumentNullException">Thrown, if string is invalid</exception>
    /// <exception cref="FileNotFoundException">Thrown, if file does not exist</exception>
    public static void ValidateExistingFile([InvokerParameterName] string parameterName, string filePath)
    {
        ValidateString(parameterName, filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file '{filePath}' does not exist.", parameterName);
        }
    }

    /// <summary>
    ///     Validates a file path (Only the string checked, the file must not exist)
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="filePath">The file path to validate</param>
    public static void ValidateFilePath([InvokerParameterName] string parameterName, string filePath)
    {
        ValidateString(parameterName, filePath);

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        Path.GetFileName(filePath);
        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        Path.GetDirectoryName(filePath);
    }

    /// <summary>
    ///     Validates a file path (Only the string checked, the file must not exist)
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="directoryPath">The directory path to validate</param>
    public static void ValidateDirectoryPath([InvokerParameterName] string parameterName, string directoryPath)
    {
        ValidateString(parameterName, directoryPath);

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        Path.GetDirectoryName(directoryPath);
    }
}