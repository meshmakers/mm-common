using System.Collections.Generic;
using Meshmakers.Common.Shared.Services;

namespace Meshmakers.Common.CommandLineParser;

public interface IArgumentParser
{
    /// <summary>
    ///     Adds an argument definition
    /// </summary>
    /// <param name="shortTerm">Short term of argument e. g. c</param>
    /// <param name="longTerm">Long term of argument e. g. create</param>
    /// <param name="description">The description of the argument. Every array entry creates a single line in usage messages.</param>
    /// <param name="isMandatoryArgument">True, if the argument is mandatory</param>
    ICommandArgument AddCommandArgument(string shortTerm, string longTerm, string[] description,
        bool isMandatoryArgument);

    IArgument AddArgument(string shortTerm, string longTerm, string[] description);
    IArgument AddArgument(string shortTerm, string longTerm, string[] description, int mandatoryValuesCount);
    IArgument AddArgument(string shortTerm, string longTerm, string[] description, bool isMandatoryArgument);

    IArgument AddArgument(string shortTerm, string longTerm, string[] description, int mandatoryValuesCount,
        bool optionalValuesCount);

    IArgument AddArgument(string shortTerm, string longTerm, string[] description, bool isMandatoryArgument,
        int mandatoryValuesCount);

    IArgument AddArgument(string shortTerm, string longTerm, string[] description, bool isMandatoryArgument,
        int mandatoryValuesCount, bool areOptionalValuesAllowed);

    IArgumentValue GetArgumentValue(IArgument argDefinition);

    bool IsArgumentUsed(IArgument argDefinition);

    /// <summary>
    ///     Parses and validates command line args of the current layer
    /// </summary>
    /// <param name="arguments">Value line args as string array</param>
    /// <exception cref="InvalidParameterException">
    ///     Thrown, if an argument has been found that is not defined by an argument
    ///     definition.
    /// </exception>
    /// <exception cref="MandatoryArgumentsMissingException">Thrown, if an argument is mandatory but not defined.</exception>
    /// <exception cref="ArgumentValueMissingException">
    ///     Thrown, if an the argument value count does not match the passed
    ///     argument values from command line.
    /// </exception>
    /// <returns>A list of arguments that are not parsed</returns>
    IEnumerable<string> ParseLayer(IEnumerable<string> arguments);

    /// <summary>
    ///     Shows usage information of the current layer
    /// </summary>
    /// <param name="emptySpacesOnStartCount">Amount of empty spaces on start</param>
    /// <param name="consoleService"></param>
    void ShowLayerUsage(int emptySpacesOnStartCount, IConsoleService consoleService);
}