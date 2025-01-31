using KeePass2Trezor.Logger;
using Microsoft.Extensions.Logging;
using System;
using Trezor.Net.Contracts.Common;

namespace KeePass2Trezor.Device
{
    /// <summary>
    /// The EventLogger class parses the Trezor.Net log to detect the necessary events
    /// (waiting for the user to click a confirm button, etc.)
    /// Trezor.Net does not provide any other method to track these events.
    /// </summary>
    internal class EventLogger : ILogger
    {
        private readonly IDeviceStateEventReceiver _receiver;
        private readonly string _name;
        private readonly ILogger _logger;

        public EventLogger(IDeviceStateEventReceiver receiver, string name, ILogger logger)
        {
            _receiver = receiver;
            _name = name;
            _logger = logger;
        }

#if TREZORNET4
        public void Log(string message, string region, Exception ex, LogLevel logLevel)
        {
            if (message == "Write: Trezor.Net.Contracts.Common.ButtonAck")
                receiver.KeyDeviceEventFired(new KeyDeviceStateEvent(KeyDeviceState.WaitConfirmation));
        }
#else
        public IDisposable BeginScope(string messageFormat, params object[] args)
        {
            _logger?.BeginScope(messageFormat, args);
            return new DummyDisposable();
        }

        public void LogDebug(string message, params object[] args)
        {
        }

        public void LogError(EventId eventId, Exception exception, string message, params object[] args)
        {
            _logger?.LogError(eventId, exception, message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger?.LogError(exception, message, args);
        }

        public void LogTrace<T>(T state)
        {
            _logger?.LogTrace(state);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger?.LogWarning(message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger?.LogInformation(message, args);
            if (message.StartsWith("Write: ") && message.EndsWith(nameof(ButtonAck)))
                _receiver.KeyDeviceEventFired(new KeyDeviceStateEvent(KeyDeviceState.WaitConfirmation));
            //if (message == "Closing device ... {deviceId}")
            //    receiver.KeyDeviceEventFired(new KeyDeviceStateEvent(KeyDeviceState.Disconnected));
        }
#endif
    }
}
