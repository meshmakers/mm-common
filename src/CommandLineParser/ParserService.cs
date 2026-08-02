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
    private const string UsageNotesHeader = "NOTES";
    private const string UsageNameHeader = "ARGUMENT NAME";
    private const string UsageDescriptionHeader = "DESCRIPTION";
    private const string UsageGroupHeader = "COMMAND GROUP";
    private const string UsageCommandCountHeader = "COMMANDS";
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
    public void AddSample(IArgument commandArgument, string commandValue, CodeSample codeSample)
    {
        ArgumentValidation.Validate(nameof(commandArgument), commandArgument);
        ArgumentValidation.ValidateString(nameof(commandValue), commandValue);
        ArgumentValidation.Validate(nameof(codeSample), codeSample);

        _sampleList.Add(new RegisteredSample(commandArgument, commandValue, codeSample));
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

        if (CommandArgument != null)
        {
            _consoleService.WriteLineRegardSpace(
                "Add --help (-?) to a single command to show only the help of that command, for example: " +
                $"{applicationExeName} -{CommandArgument.ShortTerm} <command> --help");
            _consoleService.WriteLineRegardSpace(
                $"Run '{applicationExeName} --help' for the command groups, " +
                $"'{applicationExeName} --help <group>' for the commands of one group.");
        }

        _consoleService.WriteLine("");
        _consoleService.WriteColumnLine(UsageNameHeader, Constants.UsageNameLength, UsageDescriptionHeader);

        ShowLayerUsage(0, _consoleService);

        ShowSamples(applicationExeName, _sampleList);
    }

    /// <inheritdoc />
    public void ShowGroupOverviewInformation(string applicationExeName, ICommandArgument commandArgument)
    {
        ArgumentValidation.ValidateString(nameof(applicationExeName), applicationExeName);
        ArgumentValidation.Validate(nameof(commandArgument), commandArgument);

        var groups = commandArgument.CommandValues
            .GroupBy(x => x.Group)
            .OrderBy(x => x.Key)
            .ToList();

        _consoleService.WriteLineRegardSpace(
            $"{applicationExeName} groups its commands by topic. Pick a group to see its commands:");
        _consoleService.WriteLine("");
        _consoleService.WriteColumnLine(UsageGroupHeader, Constants.UsageNameLength, UsageCommandCountHeader);

        var prefix = "".PadRight(Constants.TabCount);
        foreach (var group in groups)
        {
            _consoleService.WriteColumnLine($"{prefix}{group.Key}", Constants.UsageNameLength,
                group.Count().ToString());
        }

        _consoleService.WriteLine("");
        _consoleService.WriteLineRegardSpace(
            $"Run '{applicationExeName} --help <group>' for the commands of a group, " +
            $"'{applicationExeName} -{commandArgument.ShortTerm} <command> --help' for a single command, " +
            $"'{applicationExeName} --help {Constants.AllCommandsHelpTopic}' for every command with all arguments.");
    }

    /// <inheritdoc />
    public void ShowGroupUsageInformation(string applicationExeName, ICommandArgument commandArgument,
        string groupName, string? shadowedCommandValue)
    {
        ArgumentValidation.ValidateString(nameof(applicationExeName), applicationExeName);
        ArgumentValidation.Validate(nameof(commandArgument), commandArgument);
        ArgumentValidation.ValidateString(nameof(groupName), groupName);

        var commandCount = commandArgument.CommandValues
            .Count(x => string.Equals(x.Group, groupName, StringComparison.OrdinalIgnoreCase));

        _consoleService.WriteLineRegardSpace(
            $"Commands of group '{groupName}' ({commandCount}):");

        commandArgument.ShowGroupUsage(0, _consoleService, groupName, false);

        _consoleService.WriteLine("");
        _consoleService.WriteLineRegardSpace(
            $"Run '{applicationExeName} -{commandArgument.ShortTerm} <command> --help' for the arguments of a " +
            "single command.");

        if (shadowedCommandValue != null)
        {
            _consoleService.WriteLineRegardSpace(
                $"Note: '{shadowedCommandValue}' is also a command — run " +
                $"'{applicationExeName} -{commandArgument.ShortTerm} {shadowedCommandValue} --help' for its help.");
        }
    }

    /// <inheritdoc />
    public void ShowCommandUsageInformation(string applicationExeName, IArgument commandArgument,
        ICommandArgumentValue commandArgumentValue, CommandDocumentation? documentation)
    {
        ArgumentValidation.ValidateString(nameof(applicationExeName), applicationExeName);
        ArgumentValidation.Validate(nameof(commandArgument), commandArgument);
        ArgumentValidation.Validate(nameof(commandArgumentValue), commandArgumentValue);

        _consoleService.WriteLineRegardSpace(
            $"Usage of command '{commandArgumentValue.Value}' (argument names case insensitive, " +
            "arguments can be given in any order):");
        _consoleService.WriteLineRegardSpace(
            $"{applicationExeName} -{commandArgument.ShortTerm} {commandArgumentValue.Value} " +
            "[-[shortTerm] or [/ or --][longTerm] [argument value]] ...");
        _consoleService.WriteLine("");
        _consoleService.WriteLine(commandArgumentValue.Group.ToUpper());
        _consoleService.WriteLine("");
        _consoleService.WriteLineRegardSpace(commandArgumentValue.Description);
        _consoleService.WriteLine("");
        _consoleService.WriteColumnLine(UsageNameHeader, Constants.UsageNameLength, UsageDescriptionHeader);

        commandArgumentValue.ShowLayerUsage(Constants.TabCount, _consoleService);

        ShowSamples(applicationExeName,
            _sampleList.Where(x => string.Equals(x.CommandValue, commandArgumentValue.Value,
                StringComparison.OrdinalIgnoreCase)));
        ShowNotes(documentation?.Notes);
    }

    private void ShowSamples(string applicationExeName, IEnumerable<RegisteredSample> samples)
    {
        var sampleList = samples.ToList();
        if (sampleList.Count == 0)
        {
            return;
        }

        _consoleService.WriteLine("");
        _consoleService.WriteLine(UsageSamplesHeader);
        _consoleService.WriteLine("");

        foreach (var entry in sampleList)
        {
            _consoleService.WriteLine(ComposeInvocation(applicationExeName, entry));
            _consoleService.WriteLine("  " + entry.Sample.Description);
            _consoleService.WriteLine("");
        }
    }

    private void ShowNotes(IEnumerable<string>? notes)
    {
        var noteList = notes?.ToList();
        if (noteList == null || noteList.Count == 0)
        {
            return;
        }

        _consoleService.WriteLine(UsageNotesHeader);
        _consoleService.WriteLine("");

        foreach (var note in noteList)
        {
            _consoleService.WriteLineRegardSpace($"- {note}");
        }

        _consoleService.WriteLine("");
    }

    private static string ComposeInvocation(string applicationExeName, RegisteredSample entry)
    {
        var sb = new StringBuilder();
        sb.Append(applicationExeName)
            .Append(" -").Append(entry.CommandArgument.ShortTerm)
            .Append(' ').Append(entry.CommandValue);
        foreach (var arg in entry.Sample.Arguments)
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

    private sealed record RegisteredSample(IArgument CommandArgument, string CommandValue, CodeSample Sample);
}
