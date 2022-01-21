using Device.Net;
using Hardwarewallets.Net.AddressManagement;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trezor.Net;
using Trezor.Net.Contracts.Crypto;
using Usb.Net.Windows;
using TrezorManagerBroker = TrezorKeyProviderPlugin.Trezor.Net.Manager.TrezorManagerBroker;

namespace TrezorKeyProviderPlugin
{
    public enum LogLevel
    {
        Information,
        Error,
    }
    public interface ILogger
    {
        void Log(string message, string region, Exception ex, LogLevel logLevel);
    }

    class DeviceLogger : ILogger
    {
        private readonly string filename;
        public DeviceLogger(string filename)
        {
            this.filename = filename;
        }


        public void Log(string message, string region, Exception ex, LogLevel logLevel)
        {
            lock (filename)
            {
                //System.IO.File.AppendAllText(filename, message + "\r\n");
            }
        }

    }

    public class TrezorDevice : IDisposable
    {
        public enum TrezorState
        {
            Disconnected,
            Connected,
            ButtonRequest,
            Confirmed,
            WaitPin,
            WaitPassfrase,
            Error,
            Processing,
        }

        #region Constructors
        public TrezorDevice(byte[] keyId = null)
        {
            // Source 256-bit message to generate key in Trezor.
            // Consists of 4DEC0DED bytes repeated 8 times.
            this.salt = Enumerable.Repeat(new byte[4] { 0x4D, 0xEC, 0x0D, 0xED }, 8).SelectMany(b => b).ToArray();
            this.keyId = keyId != null
                ? Convert.ToBase64String(keyId)
                : null;
        }
        #endregion Constructor

        #region Public Methods
        public void Log(string message, string region, Exception ex, LogLevel logLevel)
        {
            //if (region == "TrezorManagerBase" && message.Contains(typeof(Trezor.Net.Contracts.Common.ButtonRequest).FullName))
            //{
            //    SetState(TrezorState.ButtonRequest, "Confirm key decryption on your Trezor device");
            //}
            DevLogger.Log(message, region, ex, logLevel);
        }

        public void Close()
        {
            cancellation.Cancel();
            _trezorManager.Device.Close();
            _TrezorManagerBroker.Stop();
        }

        /// <summary>
        /// TODO: This should be made in to a unit test but it's annoying to add the UI for a unit test as the Trezor requires human intervention for the pin
        /// public async Task<byte[]> GetEncryptionKey(Func<int> GetPinHandler, byte[] salt)
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> Encrypt()
        {
            try
            {
                if (usbFactory == null)
                    usbFactory = TrezorManager.DeviceDefinitions.CreateWindowsUsbDeviceFactory();
                Logger.Log("Register devices factory.", null, null, LogLevel.Information);
                //This only needs to be done once.
                //Register the factory for creating Usb devices. Trezor One Firmware 1.7.x / Trezor Model T

                // var hidFactory = TrezorManager.DeviceDefinitions.CreateWindowsHidDeviceFactory();
                //var aggregateFactory = usbFactory.Aggregate(hidFactory/*, _loggerFactory*/);
                var aggregateFactory = usbFactory;

                //WindowsUsbDeviceFactory.Register(UsbLogger, new DebugTracer());

                //Register the factory for creating Hid devices. Trezor One Firmware 1.6.x
                //WindowsHidDeviceFactory.Register(UsbLogger, new DebugTracer());

                Logger.Log("Waiting for Trezor... Please plug it in if it is not connected.", null, null, LogLevel.Information);
                //if (_TrezorManagerBroker == null)
                _TrezorManagerBroker = new TrezorManagerBroker(this.GetPin, this.GetPassphrase, 2000, aggregateFactory);
                _TrezorManagerBroker.Start();
                using (_TrezorManagerBroker)
                {
                    //_TrezorManagerBroker.TrezorInitialized += (object sender, TrezorManagerConnectionEventArgs<Trezor.Net.Contracts.MessageType> e) =>
                    //{
                    //    //Logger.Log("Trezor connected.", null, null, LogLevel.Information);
                    //};

                    //_TrezorManagerBroker.TrezorDisconnected += (object sender, TrezorManagerConnectionEventArgs<Trezor.Net.Contracts.MessageType> e) =>
                    //{
                    //    if (_trezorManager == e.TrezorManager)
                    //    {
                    //        Logger.Log("Trezor disconnected.", null, null, LogLevel.Information);
                    //        _connectionClosed.Set();
                    //        SetState(TrezorState.Disconnected);
                    //    }
                    //};

                    _trezorManager = await _TrezorManagerBroker.WaitForFirstTrezorAsync(cancellation.Token).ConfigureAwait(false);

                    //_trezorManager.Logger = this;

                    //private void _TrezorManagerBroker_TrezorDisconnected(object sender, TrezorManagerConnectionEventArgs<Trezor.Net.Contracts.MessageType> e)

                    //using (_trezorManager)
                    {
                        SetState(TrezorState.Connected, string.Format("{0} Model {1} connection recognized", _trezorManager.Features.Label, _trezorManager.Features.Model));
                        Logger.Log("Trezor connection recognized", null, null, LogLevel.Information);

                        var cipherKeyValue = new CipherKeyValue()
                        {
                            Key = "Unlock encrypted storage?",// "KeePass" + (keyId != null ? (" " + keyId) : ""),
                            AskOnDecrypt = true,
                            AskOnEncrypt = true,
                            Encrypt = true,
                            Value = salt,
                            AddressNs = AddressPathBase.Parse<BIP44AddressPath>("m/10016'/0"/*"m/1'/2'/3'"*/).ToArray()
                        };
                        var res = await _trezorManager.SendMessageAsync<CipheredKeyValue, CipherKeyValue>(cipherKeyValue);
                        SetState(TrezorState.Confirmed, "Operation confirmed");
                        return res.Value;
                        Logger.Log("All good", null, null, LogLevel.Information);
                    }
                    return null;
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
                    SetState(TrezorState.Error, ex.Message);
                    Logger.Log(ex.ToString(), null, ex, LogLevel.Error);
                }
                throw;
            }
            finally
            {

            }
        }

        public void SetPin(string pin)
        {
            Logger.Log("Setting pin...", null, null, LogLevel.Information);
            if (pin == null)
                throw new ArgumentNullException("pin");
            _lastPin = pin;
            if (State == TrezorState.WaitPin)
                SetState(TrezorState.Processing);
            _pinEvent.Set();
        }
        public void Initialize()
        {
            SetState(TrezorState.Disconnected);
        }

        public TrezorState State
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

        public void SetState(TrezorState state, string message = null)
        {
            if (this.state != state)
            {
                this.state = state;
                this.stateMessage = message;
                OnChangeState(this, new TrezorStateEvent(state, message));
            }
        }

        public event EventHandler<TrezorStateEvent> OnChangeState;

        public void Dispose()
        {
        }

        #endregion Public Methods

        #region Private Fields
        private TrezorManager _trezorManager;
        private static IDeviceFactory usbFactory = null;
        private static TrezorManagerBroker _TrezorManagerBroker = null;
        private CancellationTokenSource cancellation = new CancellationTokenSource();
        private ManualResetEvent _pinEvent = new ManualResetEvent(false);
        private AutoResetEvent _connectionClosed = new AutoResetEvent(false);
        private string _lastPin = null;
        private TrezorState state;
        private string stateMessage;
        private static readonly string[] _Addresses = new string[50];
        private static ILogger Logger = new DeviceLogger(@"r:\trezor.log");
        private static ILogger DevLogger = new DeviceLogger(@"r:\trezor_dev.log");
        private static ILogger UsbLogger = new DeviceLogger(@"r:\trezor_usb.log");
        private byte[] salt;
        private string keyId;
        //private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => _ = builder.AddDebug().SetMinimumLevel(LogLevel.Trace));
        #endregion Private Fields

        #region Private Methods

        private Task<string> GetPin()
        {
            return Task.Run(() =>
                    {
                        Logger.Log("Waiting for the pin", null, null, LogLevel.Information);
                        _pinEvent.Reset();
                        SetState(TrezorState.WaitPin);
                        int index = WaitHandle.WaitAny(new[] { _pinEvent, cancellation.Token.WaitHandle, _connectionClosed });
                        cancellation.Token.ThrowIfCancellationRequested();
                        SetState(TrezorState.Processing);
                        _pinEvent.Reset();
                        if (index == 0)
                        {
                            Logger.Log("Pin applied", null, null, LogLevel.Information);
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
    }
}
