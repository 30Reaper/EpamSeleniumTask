using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog.Extensions.Logging;

namespace EpamSeleniumTask.Core;

public static class Logging
{
    private static ILoggerFactory? _factory;

    public static void Init(IConfiguration configuration)
    {
        var minLevelText = configuration["Logging:MinimumLevel"] ?? "Information";
        var logDir = configuration["Logging:LogDirectory"] ?? "logs";
        Directory.CreateDirectory(logDir);

        var level = ParseLevel(minLevelText);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(level)
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(logDir, "tests-.log"), rollingInterval: RollingInterval.Day)
            .CreateLogger();

        _factory = new SerilogLoggerFactory(Log.Logger);
        Log.Information("Logger initialized. MinimumLevel={Level}", level);
    }

    public static ILoggerFactory? LoggerFactory => _factory;

    public static ILogger<T> CreateLogger<T>() => _factory?.CreateLogger<T>() ?? NullLogger<T>.Instance;

    private static LogEventLevel ParseLevel(string level)
    {
        return level.ToLowerInvariant() switch
        {
            "verbose" => LogEventLevel.Verbose,
            "debug" => LogEventLevel.Debug,
            "information" => LogEventLevel.Information,
            "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}
