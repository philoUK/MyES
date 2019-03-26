namespace OrderingExample.Logging
{
    using System;
    using Attributes;
    using Core.Logging;
    using Microsoft.Azure.WebJobs.Host.Config;
    using Serilog;

    internal class LoggerExtensionsConfigProvider : IExtensionConfigProvider
    {
        private const string EnvironmentKey = "AzureWebJobsEnv";

        private const string AppInsightsKey = "APPINSIGHTS_INSTRUMENTATIONKEY";

        public void Initialize(ExtensionConfigContext context)
        {
            var environment = Environment.GetEnvironmentVariable(EnvironmentKey) ?? "Development";
            var instrumentationKey = Environment.GetEnvironmentVariable(AppInsightsKey);

            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.WithProperty("Client", "OrderingExample")
                .Enrich.WithProperty("Application", "OrderingExample.Functions");

            var log = Logger.Configure(loggerConfiguration, environment, instrumentationKey);

            context.AddBindingRule<LoggerAttribute>()
                .BindToInput(attr => log.ForContext("Function", attr.Function));
        }
    }
}
