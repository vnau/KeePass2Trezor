﻿using Device.Net;
using Hardwarewallets.Net.AddressManagement;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trezor.Net;
using Trezor.Net.Contracts.Crypto;
using Usb.Net.Windows;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using TrezorManagerBroker = TrezorKeyProviderPlugin.Trezor.Net.Manager.TrezorManagerBroker;

namespace TrezorKeyProviderPlugin.Device
{
    internal sealed partial class TrezorDevice : IKeyProviderDevice, IDisposable, ILoggerFactory, IDeviceStateEventReceiver
    {
        private static IDeviceFactory deviceFactory = null;

        #region Constructors
        public TrezorDevice()
        {
            // Source 256-bit message used to generate a master key in Trezor:
            // a239b39a000e121a7782e945fc5c178f739b3c4174a54076f54b7d3593ce5979
            this.salt = new byte[32] {
                0xa2, 0x39, 0xb3, 0x9a, 0x00, 0x0e, 0x12, 0x1a,
                0x77, 0x82, 0xe9, 0x45, 0xfc, 0x5c, 0x17, 0x8f,
                0x73, 0x9b, 0x3c, 0x41, 0x74, 0xa5, 0x40, 0x76,
                0xf5, 0x4b, 0x7d, 0x35, 0x93, 0xce, 0x59, 0x79};
        }
        #endregion Constructor

        #region Public Methods

        public void Close()
        {
            _cancellationToken.Cancel();
            if (_trezorManager != null)
            {
                _trezorManager.Device.Close();
            }
            _TrezorManagerBroker.Stop();
        }

        /// <summary>
        /// TODO: This should be made in to a unit test but it's annoying to add the UI for a unit test as the Trezor requires human intervention for the pin
        /// public async Task<byte[]> GetEncryptionKey(Func<int> GetPinHandler, byte[] salt)
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> GetKeyByRequest(string request)
        {
            try
            {
                if (deviceFactory == null)
                {
                    //This only needs to be done once.
#if TREZOR_NET_4
                    //Register the factory for creating Usb devices. Trezor One Firmware 1.7.x / Trezor Model T
                    WindowsUsbDeviceFactory.Register(UsbLogger, new DebugTracer());

                    //Register the factory for creating Hid devices. Trezor One Firmware 1.6.x
                    WindowsHidDeviceFactory.Register(UsbLogger, new DebugTracer());
#endif
                    //Logger.Log("Register devices factory.", null, null, LogLevel.Information);
                    var usbFactory = TrezorManager.DeviceDefinitions.CreateWindowsUsbDeviceFactory();
                    // var hidFactory = TrezorManager.DeviceDefinitions.CreateWindowsHidDeviceFactory();
                    //deviceFactory = usbFactory.Aggregate(hidFactory/*, _loggerFactory*/);
                    deviceFactory = usbFactory;
                }


                //Logger.Log("Waiting for Trezor... Please plug it in if it is not connected.", null, null, LogLevel.Information);
                //if (_TrezorManagerBroker == null)
                _TrezorManagerBroker = new TrezorManagerBroker(this.GetPin, this.GetPassphrase, 2000, deviceFactory, new DefaultCoinUtility(), this);
                _TrezorManagerBroker.Start();
                using (_TrezorManagerBroker)
                {
                    //{
                    //    //Logger.Log("Trezor connected.", null, null, LogLevel.Information);
                    //};

                    //    (object sender, TrezorManagerConnectionEventArgs<Trezor.Net.Contracts.MessageType> e) =>
                    //{
                    //    if (_trezorManager == e.TrezorManager)
                    //    {
                    //        Logger.Log("Trezor disconnected.", null, null, LogLevel.Information);
                    //        _connectionClosed.Set();
                    //        SetState(TrezorState.Disconnected);
                    //    }
                    //};

                    _trezorManager = await _TrezorManagerBroker.WaitForFirstTrezorAsync(_cancellationToken.Token).ConfigureAwait(false);
                    await _trezorManager.InitializeAsync();
                    //_trezorManager.Logger = this;

                    //private void _TrezorManagerBroker_TrezorDisconnected(object sender, TrezorManagerConnectionEventArgs<Trezor.Net.Contracts.MessageType> e)

                    //using (_trezorManager)
                    {
                        SetState(KeyDeviceState.Connected, string.Format("{0} Model {1} connection recognized", _trezorManager.Features.Label, _trezorManager.Features.Model));
                        //Logger.Log("Trezor connection recognized", null, null, LogLevel.Information);


                        var cipherKeyValue = new CipherKeyValue()
                        {
                            Key = request,
                            AskOnDecrypt = true,
                            AskOnEncrypt = true,
                            Encrypt = true,
                            Value = salt,
                            AddressNs = AddressPathBase.Parse<BIP44AddressPath>("m/10016'/0"/*"m/1'/2'/3'"*/).ToArray()
                        };
                        var res = await _trezorManager.SendMessageAsync<CipheredKeyValue, CipherKeyValue>(cipherKeyValue);
                        SetState(KeyDeviceState.Confirmed, "Operation confirmed");
                        //Logger.Log("All good", null, null, LogLevel.Information);
                        return res.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                //if (ex is FailureException<Trezor.Net.Contracts.Common.Failure> cmn)
                //{
                //    SetState(TrezorState.Error, cmn.Failure.Message);
                //    Logger.Log(cmn.Failure.Message, null, cmn, LogLevel.Error);
                //}
                //else
                {
                    SetState(KeyDeviceState.Error, ex.Message);
                    //Logger.Log(ex.ToString(), null, ex, LogLevel.Error);
                }
                throw;
            }
            finally
            {

            }
        }

        public void SetPin(string pin)
        {
            //Logger.Log("Setting pin...", null, null, LogLevel.Information);
            if (pin == null)
                throw new ArgumentNullException("pin");
            _lastPin = pin;
            if (State == KeyDeviceState.WaitPIN)
                SetState(KeyDeviceState.Processing);
            _pinEvent.Set();
        }

        public KeyDeviceState State
        {
            get
            {
                return state;
            }
        }

        public string StateMessage
        {
            get
            {
                return stateMessage;
            }
        }

        private void SetState(KeyDeviceState state, string message = null)
        {
            if (this.state != state)
            {
                this.state = state;
                this.stateMessage = message;
                OnChangeState(this, new KeyDeviceStateEvent(state, message));
            }
        }

        public event EventHandler<KeyDeviceStateEvent> OnChangeState;

        public void Dispose()
        {
            _cancellationToken.Dispose();
            _connectionClosed.Dispose();
            _pinEvent.Dispose();
        }

        #endregion Public Methods

        #region Private Fields
        private TrezorManager _trezorManager;
        private static IDeviceFactory usbFactory = null;
        private static TrezorManagerBroker _TrezorManagerBroker = null;
        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private readonly ManualResetEvent _pinEvent = new ManualResetEvent(false);
        private readonly AutoResetEvent _connectionClosed = new AutoResetEvent(false);
        private string _lastPin = null;
        private KeyDeviceState state;
        private string stateMessage;
        //private readonly static ILogger Logger = new Logger(@"r:\trezor.log");
        //private readonly static ILogger DevLogger = new DeviceLogger(@"r:\trezor_dev.log");
        private readonly byte[] salt;

        //private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => _ = builder.AddDebug().SetMinimumLevel(LogLevel.Trace));
        #endregion Private Fields

        #region Private Methods

        private Task<string> GetPin()
        {
            return Task.Run(() =>
                    {
                        //Logger.Log("Waiting for the pin", null, null, LogLevel.Information);
                        _pinEvent.Reset();
                        SetState(KeyDeviceState.WaitPIN);
                        int index = WaitHandle.WaitAny(new[] { _pinEvent, _cancellationToken.Token.WaitHandle, _connectionClosed });
                        _cancellationToken.Token.ThrowIfCancellationRequested();
                        SetState(KeyDeviceState.Processing);
                        _pinEvent.Reset();
                        if (index == 0)
                        {
                            //Logger.Log("Pin applied", null, null, LogLevel.Information);
                            return _lastPin;
                        }
                        else
                        {
                            return null;
                        }
                    });
        }

        private Task<string> GetPassphrase()
        {
            return GetPin();
        }

        #endregion Private Methods

        #region ILoggerFactory implementation
        public ILogger CreateLogger(string name)
        {
            return new EventLogger(this, name);
        }
        #endregion ILoggerFactory implementation

        #region ITrezorStateEventReceiver implementation
        public void KeyDeviceEventFired(KeyDeviceStateEvent e)
        {
            SetState(e.State, e.Message);
        }
        #endregion ITrezorStateEventReceiver implementation
    }
}
