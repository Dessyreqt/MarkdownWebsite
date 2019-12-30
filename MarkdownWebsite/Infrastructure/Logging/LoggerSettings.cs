namespace MarkdownWebsite.Infrastructure.Logging
{
    using System;
    using System.IO;
    using System.Reflection;
    using DependencyResolution;
    using LightInject;
    using Microsoft.Extensions.Configuration;
    using Serilog.Events;

    public class LoggerSettings
    {
        public LoggerSettings()
        {
            var config = IoC.Container.GetInstance<IConfiguration>();

            RollingLogFileEnabled = bool.Parse(config["Logging:RollingLogFileEnabled"] ?? "false");

            if (bool.Parse(config["Logging:AutoGenerateRollingLogFilePath"] ?? "true"))
            {
                var folder = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{nameof(MarkdownWebsite)}\\logs";
                RollingLogFilePath = folder;
            }
            else
            {
                var folder = config["Logging:RollingLogFilePath"];
                RollingLogFilePath = folder;
            }

            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var fileName = $"{assemblyName}.Log_{{Date}}.txt";
            var fullFolderPath = Path.Combine(RollingLogFilePath, fileName);
            RollingLogFile = fullFolderPath;


            if (Enum.TryParse<LogEventLevel>(config["Logging:MinimumLogEventLevel"], out var logEventLevel))
            {
                MinimumLogEventLevel = logEventLevel;
            }
            else
            {
                MinimumLogEventLevel = LogEventLevel.Information;
            }
        }

        public bool RollingLogFileEnabled { get; }
        public string RollingLogFilePath { get; }
        public string RollingLogFile { get; }
        public LogEventLevel MinimumLogEventLevel { get; }
    }
}
