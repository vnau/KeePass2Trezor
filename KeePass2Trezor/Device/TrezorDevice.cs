﻿
using Device.Net;
using Hardwarewallets.Net.AddressManagement;
using KeePass2Trezor.Properties;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trezor.Net;
using Trezor.Net.Contracts.Crypto;
using Trezor.Net.Manager;
#if TREZORNET4
#else
#endif

namespace KeePass2Trezor.Device
{
    internal sealed partial class TrezorDevice : IKeyProviderDevice, IDisposable, IDeviceStateEventReceiver
    {
        #region Constructors
        public TrezorDevice(ILogger logger)
        {
            // Source 256-bit message used to generate a master key in Trezor:
            // a239b39a000e121a7782e945fc5c178f739b3c4174a54076f54b7d3593ce5979
            _salt = new byte[32] {
                0xa2, 0x39, 0xb3, 0x9a, 0x00, 0x0e, 0x12, 0x1a,
                0x77, 0x82, 0xe9, 0x45, 0xfc, 0x5c, 0x17, 0x8f,
                0x73, 0x9b, 0x3c, 0x41, 0x74, 0xa5, 0x40, 0x76,
                0xf5, 0x4b, 0x7d, 0x35, 0x93, 0xce, 0x59, 0x79};
            _logger = logger;
        }
        #endregion Constructor

        #region Public Methods

        public void Close()
        {
            if (_cancellationToken != null)
                _cancellationToken.Cancel();

            if (_trezorManager != null)
            {
                _trezorManager.Device.Close();
            }

            if (_trezorManagerBroker != null)
            {
                _trezorManagerBroker.TrezorDisconnected -= _TrezorManagerBroker_TrezorDisconnected;
                _trezorManagerBroker.Stop();
            }
        }

        /// <summary>
        /// Get a key matching specified request from a Trezor device.
        /// </summary>
        /// <returns>An encryption key</returns>
        public async Task<byte[]> GetKeyByRequest(string request)
        {
            try
            {
                if (_deviceFactory == null)
                {
                    //This only needs to be done once.
#if TREZORNET4
                    //Register the factory for creating Usb devices. Trezor One Firmware 1.7.x / Trezor Model T
                    WindowsUsbDeviceFactory.Register(new DebugLogger(), new DebugTracer());

                    //Register the factory for creating Hid devices. Trezor One Firmware 1.6.x
                    //WindowsHidDeviceFactory.Register(UsbLogger, new DebugTracer());
#else
                    _deviceFactory = new IDeviceFactory[]
                    {
                        new LibUsbDeviceFactory(new EventLoggerFactory(this, _logger)),
                        //TrezorManager.DeviceDefinitions.CreateWindowsUsbDeviceFactory(this),
                        //TrezorManager.DeviceDefinitions.CreateWindowsHidDeviceFactory(this)
                    }.Aggregate();
#endif
                }

                _trezorManagerBroker = new TrezorManagerBroker(this.GetPin, this.GetPassphrase, 2000, _deviceFactory, new DefaultCoinUtility(), new EventLoggerFactory(this, _logger) as ILoggerFactory);
                _trezorManagerBroker.TrezorDisconnected += _TrezorManagerBroker_TrezorDisconnected;
                _trezorManagerBroker.Start();

                _trezorManager = await _trezorManagerBroker.WaitForFirstTrezorAsync(_cancellationToken.Token).ConfigureAwait(false);
                await _trezorManager.InitializeAsync();
                SetState(KeyDeviceState.Connected, string.Format(Resources.TrezorConnectedMessage, _trezorManager.Features.Label, _trezorManager.Features.Model));

                var cipherKeyValue = new CipherKeyValue()
                {
                    Key = request,
                    AskOnDecrypt = true,
                    AskOnEncrypt = true,
                    Encrypt = true,
                    Value = _salt,
                    AddressNs = AddressPathBase.Parse<BIP44AddressPath>("m/10016'/0").ToArray()
                };
                var res = await _trezorManager.SendMessageAsync<CipheredKeyValue, CipherKeyValue>(cipherKeyValue);
                SetState(KeyDeviceState.Confirmed, Resources.OperationConfirmedMessage);
                return res.Value;
            }
            catch (Exception ex)
            {
                var message = string.Join("\r\n\r\n", new string[] {
                    ex.Message,
                    ex.InnerException!=null? ex.InnerException.Message:null,
                    ex.InnerException!=null? ex.InnerException.StackTrace:null
                }.Where(v => v != null));

                SetState(KeyDeviceState.Error, message);

                // if operation cancelled return null instead of throwing an exception
                if ((_cancellationToken == null || _cancellationToken.IsCancellationRequested) && ex is OperationCanceledException)
                    return null;
                else
                {
                    throw new Exception(message);
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// Event handler called when Trezor is disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TrezorManagerBroker_TrezorDisconnected(object sender, TrezorManagerConnectionEventArgs<Trezor.Net.Contracts.MessageType> e)
        {
            SetState(KeyDeviceState.Disconnected);
        }

        public void SetPin(string pin)
        {
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
                return _state;
            }
        }

        public string StateMessage
        {
            get
            {
                return _stateMessage;
            }
        }

        private void SetState(KeyDeviceState state, string message = null)
        {
            if (this._state != state)
            {
                this._state = state;
                this._stateMessage = message;
                OnChangeState(this, new KeyDeviceStateEvent(state, message));
            }
        }

        public event EventHandler<KeyDeviceStateEvent> OnChangeState;

        public void Dispose()
        {
            Close();
            _cancellationToken.Dispose();
            _connectionClosed.Dispose();
            _pinEvent.Dispose();
        }

        #endregion Public Methods

        #region Private Fields
        private TrezorManager _trezorManager;
        private static IDeviceFactory _deviceFactory = null;
        private static TrezorManagerBroker _trezorManagerBroker = null;
        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private readonly ManualResetEvent _pinEvent = new ManualResetEvent(false);
        private readonly AutoResetEvent _connectionClosed = new AutoResetEvent(false);
        private string _lastPin = null;
        private KeyDeviceState _state;
        private string _stateMessage;
        private readonly byte[] _salt;
        private ILogger _logger;
        #endregion Private Fields

        #region Private Methods

        private Task<string> GetPin()
        {
            return Task.Run(() =>
            {
                _pinEvent.Reset();
                SetState(KeyDeviceState.WaitPIN);
                int index = WaitHandle.WaitAny(new[] { _pinEvent, _cancellationToken.Token.WaitHandle, _connectionClosed });
                _cancellationToken.Token.ThrowIfCancellationRequested();
                SetState(KeyDeviceState.Processing);
                _pinEvent.Reset();
                return (index == 0) ? _lastPin : null;
            });
        }

        private Task<string> GetPassphrase()
        {
            return Task.FromResult("");
        }

        #endregion Private Methods

        #region ITrezorStateEventReceiver implementation
        public void KeyDeviceEventFired(KeyDeviceStateEvent e)
        {
            SetState(e.State, e.Message);
        }
        #endregion ITrezorStateEventReceiver implementation
    }
}
