using CommandLineParser.Sample;
using CommandLineParser.Sample.Commands;
using Meshmakers.Common.CommandLineParser;
using Meshmakers.Common.CommandLineParser.Commands;
using Meshmakers.Common.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var logger = LogManager.GetCurrentClassLogger();

try
{
    var servicesProvider = BuildDi();
    using (servicesProvider as IDisposable)
    {
        var runner = servicesProvider.GetRequiredService<Runner>();
        return await runner.DoActionAsync();
    }
}
catch (Exception ex)
{
    // NLog: catch any exception and log it.
    logger.Error(ex, "Stopped program because of exception");
    return -100;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}

static IServiceProvider BuildDi()
{
    var services = new ServiceCollection();

    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                $".{Constants.SettingsFolderName}{Path.DirectorySeparatorChar}settings.json"),
            true, true)
        .Build();

    // configure Logging with NLog
    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.SetMinimumLevel(LogLevel.Trace);
        loggingBuilder.AddNLog(config);
    });

    // The program sequence executor
    services.AddTransient<Runner>();

    // Add command parser services.
    services.AddSingleton<IConsoleService, ConsoleService>();
    services.AddSingleton<IEnvironmentService, EnvironmentService>();
    services.AddSingleton<IParserService, ParserService>();
    services.AddSingleton<ICommandParser, CommandParser<SampleOptions>>();

    // Add commands
    services.AddTransient<ICommand, ExecuteCommand>();
    services.AddTransient<ICommand, PostCommand>();

    var serviceProvider = services.BuildServiceProvider();
    return serviceProvider;
}
