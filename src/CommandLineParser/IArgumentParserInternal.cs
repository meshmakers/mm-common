using System.Collections.Generic;
using Meshmakers.Common.Shared.Services;

namespace Meshmakers.Common.CommandLineParser;

internal interface IArgumentParserInternal : IArgumentParser
{
    /// <summary>
    ///     Parses and validates command line args of the current layer
    /// </summary>
    /// <param name="arguments">CommandValue line args as string array</param>
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