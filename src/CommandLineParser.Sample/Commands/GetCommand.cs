using Meshmakers.Common.CommandLineParser.Commands;
using Meshmakers.Common.Shared.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meshmakers.Common.CommandLineParser.Sample.Commands;

public class ExecuteCommand : Command<SampleOptions>
{
    private readonly IConsoleService _consoleService;
    private readonly IArgument _uri;

    public ExecuteCommand(ILogger<ExecuteCommand> logger,
        IConsoleService consoleService,
        IOptions<SampleOptions> options)
        : base(logger, "get", "Gets content from a given URI", options)
    {
        _consoleService = consoleService;
        _uri = CommandArgumentValue.AddArgument("u", "uri", new[] { "URI to call" },
            true, 1);
    }

    public override IEnumerable<CodeSample>? GetSamples()
    {
        return new[]
        {
            new CodeSample($"{Texts.Tool_Name} -c get -u 'https://www.google.at/", "Gets content from a given URI")
        };
    }

    public override async Task Execute()
    {
        var uriArgData = CommandArgumentValue.GetArgumentValue(_uri);
        var uri = uriArgData.GetValue<string>();

        Logger.LogInformation("Getting uri '{Uri}'", uri);

        var cli = new HttpClient();
        var response = await cli.GetAsync(uri);

        var result = await response.Content.ReadAsStringAsync();
        Logger.LogInformation("{Result}", result);
        _consoleService.WriteLine(result);
    }
}
