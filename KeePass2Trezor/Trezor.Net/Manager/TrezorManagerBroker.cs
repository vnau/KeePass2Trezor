extern alias MELA;
using Device.Net;
using Microsoft.Extensions.Logging;
using System;
using Trezor.Net;
using Trezor.Net.Contracts;
using Microsoft.Extensions.Logging.Abstractions;
using KeePass2Trezor.Logger;

namespace KeePass2Trezor.Override.Trezor.Net.Manager
{
    internal class TrezorManagerBroker : TrezorManagerBrokerBase<TrezorManager, MessageType>, IDisposable
    {
        #region Constructor
        public TrezorManagerBroker(
            EnterPinArgs enterPinArgs,
            EnterPinArgs enterPassphraseArgs,
            int? pollInterval,
            IDeviceFactory deviceFactory,
            ICoinUtility coinUtility = null,
            ILoggerFactory loggerFactory = null
            ) : base(
                enterPinArgs,
                enterPassphraseArgs,
                pollInterval,
                deviceFactory,
                coinUtility,
                loggerFactory
                )
        {
        }
        #endregion

        #region Protected Overrides
        protected override TrezorManager CreateTrezorManager(IDevice device)
        {
#if TREZORNET4
            return new TrezorManager(EnterPinArgs, EnterPassphraseArgs, device);
#else
            var logger = new MelaLoggerAdapter<TrezorManager>(LoggerFactory.CreateLogger<TrezorManager>());
            return new TrezorManager(EnterPinArgs, EnterPassphraseArgs, device, logger);
#endif
        }
        #endregion
    }
}
