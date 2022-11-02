using System;
using System.Collections.Generic;
using System.Linq;
using Meshmakers.Common.Shared;
using Meshmakers.Common.Shared.Services;

namespace Meshmakers.Common.CommandLineParser;

public class ArgumentParser : IArgumentParser
{
    private readonly Dictionary<string, IArgument> _arguments;
    private readonly Dictionary<string, ArgumentValue> _argumentValues;

    /// <summary>
    ///     Constructor
    /// </summary>
    internal ArgumentParser()
    {
        _arguments = new Dictionary<string, IArgument>();
        _argumentValues = new Dictionary<string, ArgumentValue>();
    }

    /// <summary>
    ///     Adds a command argument
    /// </summary>
    /// <param name="shortTerm">Short term of argument e. g. c</param>
    /// <param name="longTerm">Long term of argument e. g. create</param>
    /// <param name="description">The description of the argument. Every array entry creates a single line in usage messages.</param>
    /// <param name="isMandatoryArgument">True, when argument is mandatory</param>
    /// <returns>The resulting command argument object.</returns>
    public ICommandArgument AddCommandArgument(string shortTerm, string longTerm, string[] description,
        bool isMandatoryArgument)
    {
        ArgumentValidation.ValidateString(nameof(shortTerm), shortTerm);
        ArgumentValidation.ValidateString(nameof(longTerm), longTerm);

        var argument = new CommandArgument(shortTerm, longTerm, description, isMandatoryArgument);
        _arguments.Add(argument.LongTerm, argument);
        return argument;
    }

    /// <summary>
    ///     Adds an argument definition
    /// </summary>
    /// <param name="shortTerm">Short term of argument e. g. c</param>
    /// <param name="longTerm">Long term of argument e. g. create</param>
    /// <param name="description">The description of the argument. Every array entry creates a single line in usage messages.</param>
    /// <param name="mandatoryValuesCount">Count of mandatory values</param>
    /// <param name="areOptionalValuesAllowed">Set to true, to allow optional arguments</param>
    public IArgument AddArgument(string shortTerm, string longTerm, string[] description,
        int mandatoryValuesCount, bool areOptionalValuesAllowed)
    {
        ArgumentValidation.ValidateString(nameof(shortTerm), shortTerm);
        ArgumentValidation.ValidateString(nameof(longTerm), longTerm);
        ArgumentValidation.ValidateInt(nameof(mandatoryValuesCount), mandatoryValuesCount, 0);

        var argument = new Argument(shortTerm, longTerm, description, false,
            mandatoryValuesCount, areOptionalValuesAllowed);
        _arguments.Add(argument.LongTerm, argument);
        return argument;
    }

    /// <summary>
    ///     Adds an argument definition
    /// </summary>
    /// <param name="shortTerm">Short term of argument e. g. c</param>
    /// <param name="longTerm">Long term of argument e. g. create</param>
    /// <param name="description">The description of the argument. Every array entry creates a single line in usage messages.</param>
    /// <param name="mandatoryValuesCount">Count of mandatory values</param>
    public IArgument AddArgument(string shortTerm, string longTerm, string[] description,
        int mandatoryValuesCount)
    {
        ArgumentValidation.ValidateString(nameof(shortTerm), shortTerm);
        ArgumentValidation.ValidateString(nameof(longTerm), longTerm);
        ArgumentValidation.ValidateInt(nameof(mandatoryValuesCount), mandatoryValuesCount, 0);

        var argument = new Argument(shortTerm, longTerm, description, false,
            mandatoryValuesCount, false);
        _arguments.Add(argument.LongTerm, argument);
        return argument;
    }

    /// <summary>
    ///     Adds an argument definition
    /// </summary>
    /// <param name="shortTerm">Short term of argument e. g. c</param>
    /// <param name="longTerm">Long term of argument e. g. create</param>
    /// <param name="description">The description of the argument. Every array entry creates a single line in usage messages.</param>
    public IArgument AddArgument(string shortTerm, string longTerm, string[] description)
    {
        ArgumentValidation.ValidateString(nameof(shortTerm), shortTerm);
        ArgumentValidation.ValidateString(nameof(longTerm), longTerm);

        var argument = new Argument(shortTerm, longTerm, description, false, 0,
            false);
        _arguments.Add(argument.LongTerm, argument);
        return argument;
    }

    /// <summary>
    ///     Adds an argument definition
    /// </summary>
    /// <param name="shortTerm">Short term of argument e. g. c</param>
    /// <param name="longTerm">Long term of argument e. g. create</param>
    /// <param name="description">The description of the argument. Every array entry creates a single line in usage messages.</param>
    /// <param name="mandatoryValuesCount">Count of mandatory values</param>
    /// <param name="areOptionalValuesAllowed">When true, optional values are allowed</param>
    /// <param name="isMandatoryArgument">When true, the argument is mandatory</param>
    public IArgument AddArgument(string shortTerm, string longTerm, string[] description,
        bool isMandatoryArgument, int mandatoryValuesCount, bool areOptionalValuesAllowed)
    {
        ArgumentValidation.ValidateString(nameof(shortTerm), shortTerm);
        ArgumentValidation.ValidateString(nameof(longTerm), longTerm);
        ArgumentValidation.ValidateInt(nameof(mandatoryValuesCount), mandatoryValuesCount, 0);

        var argument = new Argument(shortTerm, longTerm, description,
            isMandatoryArgument, mandatoryValuesCount, areOptionalValuesAllowed);
        _arguments.Add(argument.LongTerm, argument);
        return argument;
    }

    /// <summary>
    ///     Adds an argument definition
    /// </summary>
    /// <param name="shortTerm">Short term of argument e. g. c</param>
    /// <param name="longTerm">Long term of argument e. g. create</param>
    /// <param name="description">The description of the argument. Every array entry creates a single line in usage messages.</param>
    /// <param name="mandatoryValuesCount">Count of mandatory values</param>
    /// <param name="isMandatoryArgument">When true, the argument is mandatory</param>
    public IArgument AddArgument(string shortTerm, string longTerm, string[] description,
        bool isMandatoryArgument, int mandatoryValuesCount)
    {
        ArgumentValidation.ValidateString(nameof(shortTerm), shortTerm);
        ArgumentValidation.ValidateString(nameof(longTerm), longTerm);
        ArgumentValidation.ValidateInt(nameof(mandatoryValuesCount), mandatoryValuesCount, 0);

        var argument = new Argument(shortTerm, longTerm, description,
            isMandatoryArgument, mandatoryValuesCount, false);
        _arguments.Add(argument.LongTerm, argument);
        return argument;
    }

    /// <summary>
    ///     Adds an argument definition
    /// </summary>
    /// <param name="shortTerm">Short term of argument e. g. c</param>
    /// <param name="longTerm">Long term of argument e. g. create</param>
    /// <param name="description">The description of the argument. Every array entry creates a single line in usage messages.</param>
    /// <param name="isMandatoryArgument">When true, the argument is mandatory</param>
    public IArgument AddArgument(string shortTerm, string longTerm, string[] description,
        bool isMandatoryArgument)
    {
        ArgumentValidation.ValidateString(nameof(shortTerm), shortTerm);
        ArgumentValidation.ValidateString(nameof(longTerm), longTerm);

        var argument = new Argument(shortTerm, longTerm, description,
            isMandatoryArgument, 0, false);
        _arguments.Add(argument.LongTerm, argument);
        return argument;
    }

    /// <summary>
    ///     Returns true when an argument has been defined
    /// </summary>
    /// <param name="argDefinition">The parameter object for check</param>
    /// <returns>True, when a argument has been used</returns>
    public bool IsArgumentUsed(IArgument argDefinition)
    {
        return _argumentValues.ContainsKey(argDefinition.LongTerm);
    }

    /// <summary>
    ///     Returns the argument object of the parameter
    /// </summary>
    /// <param name="argDefinition">Argument definition object</param>
    /// <returns>Count of arguments</returns>
    public IArgumentValue GetArgumentValue(IArgument argDefinition)
    {
        if (_argumentValues.ContainsKey(argDefinition.LongTerm))
        {
            return _argumentValues[argDefinition.LongTerm];
        }

        throw new UnknownArgumentException($"Argument '{argDefinition.LongTerm}' is not defined.");
    }

    public void ShowLayerUsage(int emptySpacesOnStartCount, IConsoleService consoleService)
    {
        foreach (var argument in _arguments.Values)
        {
            argument.ShowUsage(emptySpacesOnStartCount, consoleService);
        }
    }

    /// <summary>
    ///     Parses and validates command line args
    /// </summary>
    /// <param name="arguments">Command line args as string array</param>
    /// <exception cref="InvalidParameterException">
    ///     Thrown, if an argument has been found that is not defined by an argument
    ///     definition.
    /// </exception>
    /// <exception cref="MandatoryArgumentsMissingException">Thrown, if an argument is mandatory but not defined.</exception>
    /// <exception cref="ArgumentValueMissingException">
    ///     Thrown, if an the argument value count does not match the passed
    ///     argument values from command line.
    /// </exception>
    public virtual IEnumerable<string> ParseLayer(IEnumerable<string> arguments)
    {
        ArgumentValue? argumentValue = null;
        ArgumentValue? previousArgumentValue = null;

        _argumentValues.Clear();

        var commandLineArguments = new Queue<string>(arguments);
        if (commandLineArguments.Count == 0)
        {
            // Are mandatory arguments missing?
            ValidateMandatoryArguments();

            return new List<string>();
        }

        do
        {
            var str = commandLineArguments.Dequeue();

            if (string.IsNullOrEmpty(str))
            {
                continue;
            }

            if (argumentValue != null)
            {
                previousArgumentValue = argumentValue;
            }

            // Is an argument?
            var bFound = TryAddArgumentValue(str, out argumentValue);

            if (!bFound && IsArgumentCandidate(str) && this is ICommandArgumentValue)
            {
                var list = new List<string> { str };
                list.AddRange(commandLineArguments);
                commandLineArguments = new Queue<string>(list);
                break;
            }

            if (!bFound &&
                previousArgumentValue != null &&
                previousArgumentValue.Argument?.MandatoryValuesCount > previousArgumentValue.Values.Count)
            {
                previousArgumentValue.AddValue(str);
            }
            else if (!bFound &&
                     previousArgumentValue != null &&
                     previousArgumentValue.Argument?.AreOptionalValuesAllowed == true)
            {
                previousArgumentValue.AddValue(str);
            }
            else if (!bFound)
            {
                throw new InvalidParameterException($"{str} is an unknown parameter");
            }

            if (!bFound && previousArgumentValue?.Argument is ICommandArgument commandArgument)
            {
                if (commandArgument.TryGetCommandValue(str, out var commandArgumentValue) &&
                    commandArgumentValue != null)
                {
                    commandLineArguments = new Queue<string>(commandArgumentValue.ParseLayer(commandLineArguments));
                }
                else
                {
                    throw new InvalidParameterException(
                        $"{str} is an unknown command value for argument --'{previousArgumentValue.Argument.LongTerm}'");
                }
            }
        } while (commandLineArguments.Count > 0);

        // Are mandatory arguments missing?
        ValidateMandatoryArguments();

        // Are mandatory values of arguments missing?
        ValidateArgumentValues();

        return commandLineArguments;
    }

    private bool IsArgumentCandidate(string term)
    {
        return term.StartsWith("-") || term.StartsWith("--");
    }

    private bool TryAddArgumentValue(string term, out ArgumentValue? argumentValue)
    {
        argumentValue = null;

        foreach (var argument in _arguments.Values)
        {
            if (argument.Compare(term))
            {
                _argumentValues.TryGetValue(argument.LongTerm, out argumentValue);
                if (argumentValue == null)
                {
                    argumentValue = new ArgumentValue(argument);
                    _argumentValues.Add(argument.LongTerm, argumentValue);
                }

                return true;
            }
        }

        return false;
    }

    private void ValidateArgumentValues()
    {
        foreach (var commandLineValueIt in _argumentValues.Values)
        {
            if (commandLineValueIt.Argument.MandatoryValuesCount > commandLineValueIt.Values.Count)
            {
                throw new ArgumentValueMissingException(
                    $"{commandLineValueIt.Argument.LongTerm} is missing mandatory argument values. " +
                    $"Expect count of argument value are '{commandLineValueIt.Argument.MandatoryValuesCount}', passed argument value count was '{commandLineValueIt.Values.Count}'.");
            }
        }
    }

    private void ValidateMandatoryArguments()
    {
        var missingMandatoryArgsList =
            _arguments.Values.Where(x => !_argumentValues.ContainsKey(x.LongTerm) && x.IsMandatoryArgument);
        // ReSharper disable once PossibleMultipleEnumeration
        if (missingMandatoryArgsList.Any())
        {
            var message = "Mandatory arguments are missing:" + Environment.NewLine;
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var argDefinition in missingMandatoryArgsList)
            {
                message += $"--{argDefinition.LongTerm} (-{argDefinition.ShortTerm})";
                if (!string.IsNullOrEmpty(message))
                {
                    message += Environment.NewLine;
                }
            }

            throw new MandatoryArgumentsMissingException(message);
        }
    }
}