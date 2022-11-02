using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Meshmakers.Common.Shared;
using Meshmakers.Common.Shared.Services;

namespace Meshmakers.Common.CommandLineParser;

/// <summary>
///     Parses command line options
///     Attention! This class is equivalent to the class in MeshmakersBaseLibrary!
/// </summary>
[Export(typeof(IParserService))]
public class ParserService : ArgumentParser, IParserService
{
    private const string UsageSamplesHeader = "SAMPLES";
    private const string UsageNameHeader = "ARGUMENT NAME";
    private const string UsageDescriptionHeader = "DESCRIPTION";
    private readonly IConsoleService _consoleService;

    private readonly IEnvironmentService _environmentService;
    private readonly List<CodeSample> _sampleList;

    /// <summary>
    ///     Constructor
    /// </summary>
    [ImportingConstructor]
    public ParserService(IEnvironmentService environmentService, IConsoleService consoleService)
    {
        _environmentService = environmentService;
        _consoleService = consoleService;
        _sampleList = new List<CodeSample>();
    }

    /// <summary>
    ///     Adds a sample for usage information
    /// </summary>
    /// <param name="sample">The sample</param>
    /// <param name="description">Description of the sample</param>
    public void AddSample(string sample, string description)
    {
        ArgumentValidation.ValidateString(nameof(sample), sample);
        ArgumentValidation.ValidateString(nameof(description), description);

        _sampleList.Add(new CodeSample(sample, description));
    }

    /// <summary>
    ///     Adds a sample for usage information
    /// </summary>
    /// <param name="codeSample">The sample</param>
    public void AddSample(CodeSample codeSample)
    {
        _sampleList.Add(codeSample);
    }

    /// <summary>
    ///     Parses and validates provided command line args
    /// </summary>
    /// <exception cref="InvalidParameterException">
    ///     Thrown, if an argument has been found that is not defined by an argument
    ///     definition.
    /// </exception>
    /// <exception cref="MandatoryArgumentsMissingException">Thrown, if an argument is mandatory but not defined.</exception>
    /// <exception cref="ArgumentValueMissingException">
    ///     Thrown, if an the argument value count does not match the passed
    ///     argument values from command line.
    /// </exception>
    public void ParseAndValidate()
    {
        ParseAndValidate(_environmentService.GetCommandLineArgs());
    }

    /// <summary>
    ///     Returns the help string
    /// </summary>
    /// <returns>The help string</returns>
    public void ShowUsageInformation(string applicationExeName)
    {
        ArgumentValidation.ValidateString(nameof(applicationExeName), applicationExeName);

        _consoleService.WriteLineRegardSpace(
            "Usage of the tool (argument names case insensitive, values case insensitive where marked, " +
            "arguments can be given in any order):");
        _consoleService.WriteLineRegardSpace(
            $"{applicationExeName} [-[shortTerm] or [/ or --][longTerm] [argument value]] ...");
        _consoleService.WriteLine("");
        _consoleService.WriteColumnLine(UsageNameHeader, Constants.UsageNameLength, UsageDescriptionHeader);

        ShowLayerUsage(0, _consoleService);

        if (_sampleList.Any())
        {
            _consoleService.WriteLine("");
            _consoleService.WriteLine(UsageSamplesHeader);
            _consoleService.WriteLine("");


            foreach (var sample in _sampleList)
            {
                _consoleService.WriteLine(sample.SampleCode);
                _consoleService.WriteLine("  " + sample.Description);
                _consoleService.WriteLine("");
            }
        }
    }

    private void ParseAndValidate(string[] arguments)
    {
        ParseLayer(arguments.Skip(1) /* first arg contains name of executable */);
    }
}