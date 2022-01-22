using Microsoft.Extensions.Logging;
using System;
using TrezorKeyProviderPlugin.Logger;

namespace TrezorKeyProviderPlugin.Hardware
{
    class EventLogger : ILogger
    {
        private ITrezorStateEventReceiver receiver;
        private string name;
        public EventLogger(ITrezorStateEventReceiver receiver, string name)
        {
            this.receiver = receiver;
            this.name = name;
        }

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

        public void LogInformation(string message, params object[] args)
        {
            if (message == "Write: Trezor.Net.Contracts.Common.ButtonAck")
                receiver.TrezorEventFired(new TrezorStateEvent(TrezorState.WaitConfirmation));
        }

        public void LogTrace<T>(T state)
        {
        }

        public void LogWarning(string message, params object[] args)
        {
        }
    }
}
