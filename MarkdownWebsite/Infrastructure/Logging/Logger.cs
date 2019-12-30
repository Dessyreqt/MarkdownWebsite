namespace MarkdownWebsite.Infrastructure.Logging
{
    using System;
    using Serilog;

    public class Logger
    {
        private static readonly Lazy<Logger> Lazy = new Lazy<Logger>(() => new Logger());
        private ILogger _logger;

        private Logger()
        {
        }

        public static ILogger Instance
        {
            get
            {
                var instance = Lazy.Value;

                if (instance.WasInitialized)
                {
                    return instance._logger;
                }

                throw new InvalidOperationException("Logger must be initialized before it can be used.");
            }
        }

        private bool WasInitialized => _logger != null;

        public static void Initialize(LoggerSettings settings)
        {
            var config = new LoggerConfiguration().MinimumLevel.Is(settings.MinimumLogEventLevel);
#if DEBUG
            config.WriteTo.Debug();
#endif

            if (settings.RollingLogFileEnabled)
            {
                config.WriteTo.RollingFile(settings.RollingLogFile, retainedFileCountLimit: null);
            }

            var logger = config.CreateLogger();

            logger.Debug("Initialized Logger with Settings: {@settings}", settings);

            Lazy.Value._logger = logger;
        }
    }
}
