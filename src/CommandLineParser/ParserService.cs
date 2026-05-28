using System.Composition;
using System.Text;
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
    private readonly List<RegisteredSample> _sampleList;

    /// <summary>
    ///     Constructor
    /// </summary>
    [ImportingConstructor]
    public ParserService(IEnvironmentService environmentService, IConsoleService consoleService)
    {
        _environmentService = environmentService;
        _consoleService = consoleService;
        _sampleList = new List<RegisteredSample>();
    }

    /// <inheritdoc />
    public void AddSample(string commandVerb, CodeSample codeSample)
    {
        ArgumentValidation.ValidateString(nameof(commandVerb), commandVerb);
        ArgumentValidation.Validate(nameof(codeSample), codeSample);

        _sampleList.Add(new RegisteredSample(commandVerb, codeSample));
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

            foreach (var entry in _sampleList)
            {
                _consoleService.WriteLine(ComposeInvocation(applicationExeName, entry.CommandVerb, entry.Sample));
                _consoleService.WriteLine("  " + entry.Sample.Description);
                _consoleService.WriteLine("");
            }
        }
    }

    private static string ComposeInvocation(string applicationExeName, string commandVerb, CodeSample sample)
    {
        var sb = new StringBuilder();
        sb.Append(applicationExeName).Append(" -c ").Append(commandVerb);
        foreach (var arg in sample.Arguments)
        {
            sb.Append(" -").Append(arg.Argument.ShortTerm);
            if (arg.Value != null)
            {
                sb.Append(" \"").Append(arg.Value).Append('"');
            }
        }
        return sb.ToString();
    }

    private void ParseAndValidate(string[] arguments)
    {
        ParseLayer(arguments.Skip(1) /* first arg contains name of executable */);
    }

    private sealed record RegisteredSample(string CommandVerb, CodeSample Sample);
}
