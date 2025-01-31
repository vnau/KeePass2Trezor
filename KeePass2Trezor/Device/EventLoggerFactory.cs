using Microsoft.Extensions.Logging;

namespace KeePass2Trezor.Device
{
    internal class EventLoggerFactory : ILoggerFactory
    {
        private readonly IDeviceStateEventReceiver _eventReceiver;
        private readonly ILogger _logger;

        public EventLoggerFactory(IDeviceStateEventReceiver eventReceiver, ILogger logger)
        {
            _eventReceiver = eventReceiver;
            _logger = logger;
        }

        #region ILoggerFactory implementation
        public ILogger CreateLogger(string name)
        {
            return new EventLogger(_eventReceiver, name, _logger);
        }
        #endregion ILoggerFactory implementation
    }
}
