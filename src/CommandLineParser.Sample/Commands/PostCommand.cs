using System.Text;
using Meshmakers.Common.CommandLineParser.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meshmakers.Common.CommandLineParser.Sample.Commands;

public class PostCommand : Command<SampleOptions>
{
    private readonly IArgument _body;
    private readonly IArgument _contentType;
    private readonly IArgument _uri;

    public PostCommand(ILogger<PostCommand> logger,
        IOptions<SampleOptions> options)
        : base(logger, "Post", "Posts content to the given URI", options)
    {
        _uri = CommandArgumentValue.AddArgument("u", "uri", new[] { "URI to call" },
            true, 1);
        _contentType = CommandArgumentValue.AddArgument("ct", "contentType", new[] { "Content type of body" },
            true, 1);
        _body = CommandArgumentValue.AddArgument("b", "body", new[] { "The body content" },
            true, 1);
    }

    public override async Task Execute()
    {
        var uriArgData = CommandArgumentValue.GetArgumentValue(_uri);
        var uri = uriArgData.GetValue<string>();

        var contentTypeData = CommandArgumentValue.GetArgumentValue(_contentType);
        var contentType = contentTypeData.GetValue<string>();

        var bodyArgData = CommandArgumentValue.GetArgumentValue(_body);
        var body = bodyArgData.GetValue<string>();

        if (string.IsNullOrEmpty(body))
        {
            Logger.LogError("Body argument is invalid");
            return;
        }

        Logger.LogInformation("Getting uri '{Uri}'", uri);

        var data = new StringContent(body, Encoding.UTF8, contentType);

        var cli = new HttpClient();
        var response = await cli.PostAsync(uri, data);

        Logger.LogInformation("{Result}", response.StatusCode);
    }
}
