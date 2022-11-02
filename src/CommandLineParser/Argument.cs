using System;
using System.Linq;
using Meshmakers.Common.Shared;
using Meshmakers.Common.Shared.Services;

namespace Meshmakers.Common.CommandLineParser;

/// <summary>
///     Abstracts a command line parameter like --doSomething
/// </summary>
internal class Argument : IArgument
{
    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="shortTerm">Short term of argument e. g. c</param>
    /// <param name="longTerm">Long term of argument e. g. create</param>
    /// <param name="description">The description of the argument. Every array entry creates a single line in usage messages.</param>
    /// <param name="isMandatoryArgument">When true, the argument is mandatory</param>
    /// <param name="mandatoryValuesCount">The amount of mandatory values of this argument</param>
    /// <param name="areOptionalValuesAllowed">When true, optional values are allowed</param>
    internal Argument(string shortTerm, string longTerm, string[] description,
        bool isMandatoryArgument, int mandatoryValuesCount, bool areOptionalValuesAllowed)
    {
        ArgumentValidation.ValidateString(nameof(shortTerm), shortTerm);
        ArgumentValidation.ValidateString(nameof(longTerm), longTerm);

        LongTerm = longTerm;
        ShortTerm = shortTerm;
        Description = description;
        MandatoryValuesCount = mandatoryValuesCount;
        AreOptionalValuesAllowed = areOptionalValuesAllowed;
        IsMandatoryArgument = isMandatoryArgument;
    }

    /// <summary>
    ///     Count of mandatory values of the argument
    /// </summary>
    public int MandatoryValuesCount { get; }

    /// <summary>
    ///     Returns true if optional values are allowed
    /// </summary>
    public bool AreOptionalValuesAllowed { get; }

    /// <summary>
    ///     Returns true, when the parameter is mandatory
    /// </summary>
    public bool IsMandatoryArgument { get; }

    /// <summary>
    ///     Return the long term
    /// </summary>
    public string LongTerm { get; }

    /// <summary>
    ///     Return the short term
    /// </summary>
    public string ShortTerm { get; }

    /// <summary>
    ///     Gets or Sets the Description of the parameter
    /// </summary>
    public string[] Description { get; }

    /// <summary>
    ///     Compares a string to the short and long parameter value
    /// </summary>
    /// <param name="value">String</param>
    /// <returns></returns>
    public bool Compare(string value)
    {
        ArgumentValidation.ValidateString(nameof(value), value);

        // check long term
        if (value.StartsWith("--"))
        {
            if (string.Compare(LongTerm, 0, value, 2, Math.Max(value.Length - 2, ShortTerm.Length), true) == 0)
            {
                return true;
            }
        }

        if (value.StartsWith("/"))
        {
            if (string.Compare(LongTerm, 0, value, 1, Math.Max(value.Length - 1, ShortTerm.Length), true) == 0)
            {
                return true;
            }
        }

        // check short term
        if (value.StartsWith("-"))
        {
            if (string.Compare(ShortTerm, 0, value, 1, Math.Max(value.Length - 1, ShortTerm.Length), true) == 0)
            {
                return true;
            }
        }

        return false;
    }

    public virtual void ShowUsage(int emptySpacesOnStartCount, IConsoleService consoleService)
    {
        var prefix = "".PadRight(emptySpacesOnStartCount);

        var usageType = IsMandatoryArgument ? "Required. " : "Optional. ";
        consoleService.WriteColumnLine($"{prefix}--{LongTerm} (-{ShortTerm})", Constants.UsageNameLength,
            $"{usageType}{Description[0]}");

        foreach (var description in Description.Skip(1))
        {
            consoleService.WriteColumnLine("", Constants.UsageNameLength, description);
        }
    }
}
