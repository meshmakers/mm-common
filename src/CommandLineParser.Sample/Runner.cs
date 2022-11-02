using Meshmakers.Common.CommandLineParser;
using Meshmakers.Common.CommandLineParser.Commands;
using Microsoft.Extensions.Logging;

namespace CommandLineParser.Sample;

internal class Runner
{
    private readonly ILogger<Runner> _logger;
    private readonly ICommandParser _parser;

    public Runner(ILogger<Runner> logger, ICommandParser parser)
    {
        _logger = logger;
        _parser = parser;
    }

    public async Task<int> DoActionAsync()
    {
        try
        {
            _logger.LogInformation("Sample Command Tool, Version 1.0.0");

            await _parser.ParseAndValidateAsync();

            return 0;
        }
        catch (MandatoryArgumentsMissingException ex)
        {
            _logger.LogError("{Message}", ex.Message);
            _parser.ShowUsageInformation(Texts.Tool_Name);
            return -1;
        }
        catch (InvalidProgramException ex)
        {
            _logger.LogError("{Message}", ex.Message);
            _parser.ShowUsageInformation(Texts.Tool_Name);
            return -1;
        }
        catch (Exception ex)
        {
            var tmp = ex;
            while (tmp != null)
            {
                _logger.LogCritical(tmp, "{Message}", tmp.Message);
                tmp = tmp.InnerException;
            }

            return -99;
        }
    }
}
