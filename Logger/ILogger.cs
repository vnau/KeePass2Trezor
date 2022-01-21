using System;

namespace TrezorKeyProviderPlugin.Logger
{
    public interface ILogger
    {
        void Log(string message, string region, Exception ex, LogLevel logLevel);
    }
}
