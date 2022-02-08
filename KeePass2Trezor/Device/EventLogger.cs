#if TREZORNET4
using Device.Net;
#else
using Microsoft.Extensions.Logging;
#endif
using System;
using KeePass2Trezor.Logger;

namespace KeePass2Trezor.Device
{
    /// <summary>
    /// The EventLogger class parses the Trezor.Net log to detect the necessary events
    /// (waiting for the user to click a confirm button, etc.)
    /// Trezor.Net does not provide any other method to track these events.
    /// </summary>
    internal class EventLogger : ILogger
    {
        private IDeviceStateEventReceiver receiver;
        private string name;
        public EventLogger(IDeviceStateEventReceiver receiver, string name)
        {
            this.receiver = receiver;
            this.name = name;
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
            return new DummyDisposable();
        }

        public void LogDebug(string message, params object[] args)
        {
        }

        public void LogError(EventId eventId, Exception exception, string message, params object[] args)
        {
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
        }

        public void LogTrace<T>(T state)
        {
        }

        public void LogWarning(string message, params object[] args)
        {
        }

        public void LogInformation(string message, params object[] args)
        {
            if (message == "Write: Trezor.Net.Contracts.Common.ButtonAck")
                receiver.KeyDeviceEventFired(new KeyDeviceStateEvent(KeyDeviceState.WaitConfirmation));
            //if (message == "Closing device ... {deviceId}")
            //    receiver.KeyDeviceEventFired(new KeyDeviceStateEvent(KeyDeviceState.Disconnected));
        }
#endif
    }
}
