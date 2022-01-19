using Device.Net;
using System;
using Trezor.Net;
using Trezor.Net.Contracts;

namespace TrezorKeyProviderPlugin.Trezor.Net.Manager
{
    public class TrezorManagerBroker : TrezorManagerBrokerBase<TrezorManager, MessageType>, IDisposable
    {
        #region Constructor
        public TrezorManagerBroker(
            EnterPinArgs enterPinArgs,
            EnterPinArgs enterPassphraseArgs,
            int? pollInterval,
            IDeviceFactory deviceFactory,
            ICoinUtility coinUtility = null
            //ILoggerFactory loggerFactory = null
            ) : base(
                enterPinArgs,
                enterPassphraseArgs,
                pollInterval,
                deviceFactory,
                coinUtility
                //loggerFactory
                )
        {
        }
        #endregion

        #region Protected Overrides
        protected override TrezorManager CreateTrezorManager(IDevice device) => new TrezorManager(EnterPinArgs, EnterPassphraseArgs, device/*, LoggerFactory.CreateLogger<TrezorManager>()*/);
        #endregion
    }
}
