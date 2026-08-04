using Meshmakers.Common.Shared;

// ReSharper disable MemberCanBeProtected.Global

namespace Meshmakers.Common.CommandLineParser.Commands;

/// <summary>
///     Implements the command based parser
/// </summary>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CommandParser : ICommandParser
{
    private readonly ICommandArgument _commandArg;
    private readonly IEnumerable<ICommand> _commands;
    private readonly IParserService _parserService;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="parserService">The underlying command line parser service</param>
    /// <param name="commands">A list of commands that corresponds to the command parser</param>
    public CommandParser(IParserService parserService, IEnumerable<ICommand> commands)
    {
        _parserService = parserService;
        _commands = commands;

        _commandArg = _parserService.AddCommandArgument("c", "command",
            new[]
            {
                "Command that has to be executed:"
            }, true);

        // Understood on its own (full usage) and together with a command (help of that command only).
        _parserService.AddHelpArgument();

        foreach (var command in _commands)
        {
            _commandArg.AddCommandValue(command.CommandArgumentValue);

            var samples = command.GetDocumentation()?.Samples;
            if (samples != null)
            {
                foreach (var sample in samples)
                {
                    _parserService.AddSample(_commandArg, command.CommandArgumentValue.Value, sample);
                }
            }
        }
    }


    /// <inheritdoc />
    public virtual void ShowUsageInformation(string applicationExeName)
    {
        _parserService.ShowUsageInformation(applicationExeName);
    }

    /// <inheritdoc />
    public virtual void ShowGroupOverviewInformation(string applicationExeName)
    {
        _parserService.ShowGroupOverviewInformation(applicationExeName, _commandArg);
    }

    /// <inheritdoc />
    public virtual void ShowGroupUsageInformation(string applicationExeName, string groupName)
    {
        ArgumentValidation.ValidateString(nameof(groupName), groupName);

        _parserService.ShowGroupUsageInformation(applicationExeName, _commandArg, groupName,
            FindCommand(groupName)?.CommandArgumentValue.Value);
    }

    /// <inheritdoc />
    public virtual void ShowCommandUsageInformation(string applicationExeName, ICommand command)
    {
        ArgumentValidation.Validate(nameof(command), command);

        _parserService.ShowCommandUsageInformation(applicationExeName, _commandArg, command.CommandArgumentValue,
            command.GetDocumentation());
    }

    /// <inheritdoc />
    public virtual async Task ParseAndValidateAsync(string? applicationExeName = null)
    {
        _parserService.ParseAndValidate();

        var command = ResolveCommand(out var commandString);
        if (command == null && !_parserService.IsHelpRequested)
        {
            throw new InvalidProgramException($"Command value '{commandString}' is invalid.");
        }

        if (_parserService.IsHelpRequested || command?.CommandArgumentValue.IsHelpRequested == true)
        {
            ShowHelp(ResolveApplicationExeName(applicationExeName), command);
            return;
        }

        await command!.PreValidate();
        await command.Execute();
    }

    /// <summary>
    ///     Narrows help down to what the command line asked for. The order is deliberate: an exact group name
    ///     is served before an exact command name, because a command stays reachable through the command
    ///     selector while a group has no second way in. Fuzzy matching applies to groups only.
    /// </summary>
    /// <param name="applicationExeName">Name of the application executable used in the output</param>
    /// <param name="command">The command selected on the command line, if any</param>
    private void ShowHelp(string applicationExeName, ICommand? command)
    {
        if (command != null)
        {
            ShowCommandUsageInformation(applicationExeName, command);
            return;
        }

        var topic = _parserService.HelpTopic;
        if (string.IsNullOrEmpty(topic))
        {
            ShowGroupOverviewInformation(applicationExeName);
            return;
        }

        if (string.Equals(topic, Constants.AllCommandsHelpTopic, StringComparison.OrdinalIgnoreCase))
        {
            ShowUsageInformation(applicationExeName);
            return;
        }

        var group = FindGroup(x => string.Equals(x, topic, StringComparison.OrdinalIgnoreCase));
        if (group != null)
        {
            ShowGroupUsageInformation(applicationExeName, group);
            return;
        }

        var commandOfTopic = FindCommand(topic);
        if (commandOfTopic != null)
        {
            ShowCommandUsageInformation(applicationExeName, commandOfTopic);
            return;
        }

        group = FindGroup(x => x.StartsWith(topic, StringComparison.OrdinalIgnoreCase))
                ?? FindGroup(x => x.Contains(topic, StringComparison.OrdinalIgnoreCase));
        if (group != null)
        {
            ShowGroupUsageInformation(applicationExeName, group);
            return;
        }

        throw new InvalidParameterException(
            $"'{topic}' is neither a command group nor a command. Known groups: " +
            $"{string.Join(", ", Groups)}, {Constants.AllCommandsHelpTopic}.");
    }

    private IEnumerable<string> Groups =>
        _commands.Select(c => c.CommandArgumentValue.Group).Distinct().OrderBy(x => x);

    /// <summary>
    ///     Returns the single group matching the predicate. An ambiguous match counts as no match, so that a
    ///     shortened topic never silently picks one of several groups.
    /// </summary>
    private string? FindGroup(Func<string, bool> predicate)
    {
        var matches = Groups.Where(predicate).Take(2).ToList();

        return matches.Count == 1 ? matches[0] : null;
    }

    private ICommand? FindCommand(string commandValue)
    {
        return _commands.FirstOrDefault(c =>
            string.Equals(c.CommandArgumentValue.Value, commandValue, StringComparison.OrdinalIgnoreCase));
    }

    private ICommand? ResolveCommand(out string? commandString)
    {
        commandString = null;

        // Help may be asked for without naming a command; asking for the value of an unused argument throws.
        if (_parserService.IsHelpRequested && !_parserService.IsArgumentUsed(_commandArg))
        {
            return null;
        }

        var commandArgData = _parserService.GetArgumentValue(_commandArg);
        var value = (_parserService.IsHelpRequested
            ? commandArgData.GetValue(string.Empty)
            : commandArgData.GetValue<string>())?.ToLower();
        commandString = value;

        return _commands.FirstOrDefault(c => c.CommandArgumentValue.Value.ToLower() == value);
    }

    private static string ResolveApplicationExeName(string? applicationExeName)
    {
        return string.IsNullOrEmpty(applicationExeName)
            ? AppDomain.CurrentDomain.FriendlyName
            : applicationExeName;
    }
}
