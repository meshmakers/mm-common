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
        if (command == null)
        {
            // Without a command to show help for, a help request can only mean the full usage.
            if (_parserService.IsHelpRequested)
            {
                ShowUsageInformation(ResolveApplicationExeName(applicationExeName));
                return;
            }

            throw new InvalidProgramException($"Command value '{commandString}' is invalid.");
        }

        if (_parserService.IsHelpRequested || command.CommandArgumentValue.IsHelpRequested)
        {
            ShowCommandUsageInformation(ResolveApplicationExeName(applicationExeName), command);
            return;
        }

        await command.PreValidate();
        await command.Execute();
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
