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
        #region Fields
        private static readonly string[] _Addresses = new string[50];
        private static ILogger Logger = new DeviceLogger(@"r:\trezor.log");
        private static ILogger DevLogger = new DeviceLogger(@"r:\trezor_dev.log");
        private static ILogger UsbLogger = new DeviceLogger(@"r:\trezor_usb.log");
        private byte[] salt;
        private string keyId;
        //private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => _ = builder.AddDebug().SetMinimumLevel(LogLevel.Trace));
        #endregion

        public TrezorDevice(byte[] keyId = null)
        {
            // Source 256-bit message to generate key in Trezor.
            // Consists of 4DEC0DED bytes repeated 8 times.
            this.salt = Enumerable.Repeat(new byte[4] { 0x4D, 0xEC, 0x0D, 0xED }, 8).SelectMany(b => b).ToArray();
            this.keyId = keyId != null
                ? Convert.ToBase64String(keyId)
                : null;
        }

        public void Log(string message, string region, Exception ex, LogLevel logLevel)
        {
            //if (region == "TrezorManagerBase" && message.Contains(typeof(Trezor.Net.Contracts.Common.ButtonRequest).FullName))
            //{
            //    SetState(TrezorState.ButtonRequest, "Confirm key decryption on your Trezor device");
            //}
            DevLogger.Log(message, region, ex, logLevel);
        }

        #region Private  Methods

        private TrezorManager _trezorManager;

        public void Close()
        {
            cancellation.Cancel();
            _trezorManager?.Device?.Close();
            _TrezorManagerBroker?.Stop();
        }

        static IDeviceFactory usbFactory = null;
        static TrezorManagerBroker _TrezorManagerBroker = null;

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
                            Key = "KeePass" + (keyId != null ? (" " + keyId) : ""),
                            AskOnDecrypt = true,
                            AskOnEncrypt = false,
                            Encrypt = false,
                            Value = salt,
                            AddressNs = AddressPathBase.Parse<BIP44AddressPath>("m/1'/2'/3'").ToArray()
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

        CancellationTokenSource cancellation = new CancellationTokenSource();
        ManualResetEvent _pinEvent = new ManualResetEvent(false);
        AutoResetEvent _connectionClosed = new AutoResetEvent(false);
        private string _lastPin = null;

        public void SetPin(string pin)
        {
            Logger.Log("Setting pin...", null, null, LogLevel.Information);
            _lastPin = pin ?? throw new ArgumentNullException(nameof(pin));
            if (State == TrezorState.WaitPin)
                SetState(TrezorState.Processing);
            _pinEvent.Set();
        }

        private Task<string> GetPin()
        => Task.Run(() =>
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

        private Task<string> GetPassphrase() => GetPin();

        public void Initialize()
        {
            SetState(TrezorState.Disconnected);
        }

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

        private TrezorState state;

        public TrezorState State
        {
            get => state;
        }

        private string stateMessage;

        public string StateMessage
        {
            get => stateMessage;
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

        public event EventHandler<int> OnGetPin;

        public void Dispose()
        {

        }

        #endregion
    }
}
