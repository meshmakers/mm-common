using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

// ReSharper disable MemberCanBeProtected.Global

namespace Meshmakers.Common.CommandLineParser.Commands;

/// <summary>
///     Implements the command based parser
/// </summary>
/// <typeparam name="TOptions"></typeparam>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CommandParser<TOptions> : ICommandParser
    where TOptions : class
{
    private readonly ICommandArgument _commandArg;
    private readonly IEnumerable<ICommand> _commands;
    private readonly IParserService _parserService;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="parserService">The underlying command line parser service</param>
    /// <param name="commands">A list of commands that corresponds to the command parser</param>
    /// <param name="options">The IOptions based options object</param>
    public CommandParser(IParserService parserService, IEnumerable<ICommand> commands, IOptions<TOptions> options)
    {
        _parserService = parserService;
        _commands = commands;

        _commandArg = _parserService.AddCommandArgument("c", "command",
            new[]
            {
                "Command that has to be executed:"
            }, true);

        foreach (var command in _commands)
        {
            _commandArg.AddCommandValue(command.CommandArgumentValue);

            var samples = command.GetSamples();
            if (samples != null)
                foreach (var sample in samples)
                    _parserService.AddSample(sample);
        }
    }


    /// <inheritdoc />
    public virtual void ShowUsageInformation(string applicationExeName)
    {
        _parserService.ShowUsageInformation(applicationExeName);
    }

    /// <inheritdoc />
    public virtual async Task ParseAndValidateAsync()
    {
        _parserService.ParseAndValidate();

        var commandArgData = _parserService.GetArgumentValue(_commandArg);
        var command = commandArgData.GetValue<string>()?.ToLower();

        var ospCommand = _commands.FirstOrDefault(c => c.CommandArgumentValue.Value.ToLower() == command);
        if (ospCommand == null) throw new InvalidProgramException($"Command value '{command}' is invalid.");

        await ospCommand.PreValidate();
        await ospCommand.Execute();
    }
}