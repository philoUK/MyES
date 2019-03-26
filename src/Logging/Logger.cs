namespace Core.Logging
{
    using System;
    using System.Security.Claims;
    using Destructurama;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Events;
    using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;

    public static class Logger
    {
        public static void Configure(IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("Logging:Serilog");
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var instrumentationKey = configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.ConfigurationSection(configurationSection);

            Configure(loggerConfiguration, environment, instrumentationKey);
        }

        public static ILogger Configure(LoggerConfiguration loggerConfiguration, string environment, string appInsightsInstrumentationKey)
        {
            loggerConfiguration
                .Enrich.FromLogContext()
                .Destructure.UsingAttributes()
                .Destructure.ByTransforming<ClaimsPrincipal>(principal => new { principal?.Identity?.IsAuthenticated, principal?.Identity?.Name })
                .Enrich.WithProperty("Environment", environment);

            if (environment.ToUpperInvariant() == "DEVELOPMENT")
            {
                loggerConfiguration
                    .WriteTo.ColoredConsole()
                    .WriteTo.Seq("http://localhost:5341");
            }
            else
            {
                loggerConfiguration
                    .WriteTo.ApplicationInsights(appInsightsInstrumentationKey, new TraceTelemetryConverter(), LogEventLevel.Information);
            }

            Log.Logger = loggerConfiguration.CreateLogger();
            Log.Information("Logging initialised");

            return Log.Logger;
        }
    }
}
