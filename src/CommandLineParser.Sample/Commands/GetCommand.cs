using Meshmakers.Common.CommandLineParser;
using Meshmakers.Common.CommandLineParser.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CommandLineParser.Sample.Commands;

public class ExecuteCommand : Command<SampleOptions>
{
    private readonly IArgument _uri;

    public ExecuteCommand(ILogger<ExecuteCommand> logger,
        IOptions<SampleOptions> options) 
        : base(logger, "get", "Gets content from a given URI", options)
    {
        _uri = CommandArgumentValue.AddArgument("u", "uri", new[] {"URI to call"},
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

        Logger.LogInformation("{Result}", await response.Content.ReadAsStringAsync());
    }
}