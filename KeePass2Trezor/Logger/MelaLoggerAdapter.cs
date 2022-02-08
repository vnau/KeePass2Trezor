extern alias MELA;
using Microsoft.Extensions.Logging;
using System;
using MelaLogLevel = MELA::Microsoft.Extensions.Logging.LogLevel;

namespace KeePass2Trezor.Logger
{
    internal class MelaLoggerAdapter<T> : MELA::Microsoft.Extensions.Logging.ILogger<T>
    {
        private readonly ILogger<T> logger;
        public MelaLoggerAdapter(ILogger<T> logger)
        {
            this.logger = logger;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return logger.BeginScope(state.GetType().Name);
        }

        public bool IsEnabled(MelaLogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(MelaLogLevel logLevel, MELA.Microsoft.Extensions.Logging.EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            switch (logLevel)
            {
                case MelaLogLevel.Trace:
                    logger.LogTrace(state);
                    break;
                case MelaLogLevel.Debug:
                    logger.LogDebug(formatter(state, exception));
                    break;
                case MelaLogLevel.Information:
                    logger.LogInformation(formatter(state, exception));
                    break;
                case MelaLogLevel.Error:
                case MelaLogLevel.Critical:
                    logger.LogError(exception, formatter(state, exception));
                    break;
                case MelaLogLevel.Warning:
                    logger.LogWarning(formatter(state, exception));
                    break;
            }
        }
    }
}
