using Device.Net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trezor.Net;
using Trezor.Net.Manager;

namespace KeePass2Trezor.Override.Trezor.Net.Manager
{
    //TODO: Add logging (Inject the logger factory)

    internal abstract class TrezorManagerBrokerBase<T, TMessageType> where T : TrezorManagerBase<TMessageType>, IDisposable
    {
        protected ILoggerFactory LoggerFactory { get; private set; }

        #region Fields
        private bool _disposed;
        private readonly DeviceListener _DeviceListener;
        private readonly SemaphoreSlim _Lock = new SemaphoreSlim(1, 1);
        private readonly TaskCompletionSource<T> _FirstTrezorTaskCompletionSource = new TaskCompletionSource<T>();
        #endregion

        #region Events
        /// <summary>
        /// Occurs after the TrezorManagerBroker notices that a device hasbeen connected, and initialized
        /// </summary>
        public event EventHandler<TrezorManagerConnectionEventArgs<TMessageType>> TrezorInitialized;

        /// <summary>
        /// Occurs after the TrezorManagerBroker notices that the device has been disconnected, but before the TrezorManager is disposed
        /// </summary>
        public event EventHandler<TrezorManagerConnectionEventArgs<TMessageType>> TrezorDisconnected;
        #endregion

        #region Public Properties
        public ReadOnlyCollection<T> TrezorManagers { get; private set; }
        public EnterPinArgs EnterPinArgs { get; private set; }
        public EnterPinArgs EnterPassphraseArgs { get; private set; }
        public ICoinUtility CoinUtility { get; private set; }
        public int? PollInterval { get; private set; }
        #endregion

        #region Constructor
        protected TrezorManagerBrokerBase(
            EnterPinArgs enterPinArgs,
            EnterPinArgs enterPassphraseArgs,
            int? pollInterval,
            IDeviceFactory deviceFactory,
            ICoinUtility coinUtility = null,
            ILoggerFactory loggerFactory = null
            )
        {
            TrezorManagers = new ReadOnlyCollection<T>(new List<T>());
            EnterPinArgs = enterPinArgs;
            EnterPassphraseArgs = enterPassphraseArgs;
            CoinUtility = coinUtility ?? new DefaultCoinUtility();
            PollInterval = pollInterval;
            LoggerFactory = loggerFactory;

#if TREZORNET4
            _DeviceListener = new DeviceListener(deviceFactory, PollInterval);
#else
            _DeviceListener = new DeviceListener(deviceFactory, PollInterval, loggerFactory);          
#endif
            _DeviceListener.DeviceDisconnected += DevicePoller_DeviceDisconnected;
            _DeviceListener.DeviceInitialized += DevicePoller_DeviceInitialized;
        }
        #endregion

        #region Protected Abstract Methods
        protected abstract T CreateTrezorManager(IDevice device);
        #endregion

        #region Event Handlers
        private async void DevicePoller_DeviceInitialized(object sender, DeviceEventArgs e)
        {
            try
            {
                await _Lock.WaitAsync().ConfigureAwait(false);

                var trezorManager = TrezorManagers.FirstOrDefault(t => ReferenceEquals(t.Device, e.Device));

                if (trezorManager != null) return;

                trezorManager = CreateTrezorManager(e.Device);

                var tempList = new List<T>(TrezorManagers)
                {
                    trezorManager
                };

                TrezorManagers = new ReadOnlyCollection<T>(tempList);

                await trezorManager.InitializeAsync().ConfigureAwait(false);

                if (_FirstTrezorTaskCompletionSource.Task.Status == TaskStatus.WaitingForActivation)
                    _FirstTrezorTaskCompletionSource.SetResult(trezorManager);

                if (TrezorInitialized != null)
                    TrezorInitialized.Invoke(this, new TrezorManagerConnectionEventArgs<TMessageType>(trezorManager));
            }
            finally
            {
                _Lock.Release();
            }
        }

        private async void DevicePoller_DeviceDisconnected(object sender, DeviceEventArgs e)
        {
            try
            {
                await _Lock.WaitAsync().ConfigureAwait(false);

                var trezorManager = TrezorManagers.FirstOrDefault(t => ReferenceEquals(t.Device, e.Device));

                if (trezorManager == null) return;

                if (TrezorDisconnected != null)
                    TrezorDisconnected.Invoke(this, new TrezorManagerConnectionEventArgs<TMessageType>(trezorManager));

                trezorManager.Dispose();

                var tempList = new List<T>(TrezorManagers);

                tempList.Remove(trezorManager);

                TrezorManagers = new ReadOnlyCollection<T>(tempList);
            }
            finally
            {
                _Lock.Release();
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize listening.
        /// </summary>
        public void Start()
        {
            _DeviceListener.Start();

            //TODO: Call Start on the DeviceListener when it is implemented...
        }

        public void Stop()
        {
            if (_DeviceListener != null)
                _DeviceListener.Stop();

            if (_FirstTrezorTaskCompletionSource.Task.Status == TaskStatus.WaitingForActivation)
                _FirstTrezorTaskCompletionSource.TrySetCanceled();
        }

        /// <summary>
        /// Check to see if there are any devices connected
        /// </summary>
        public async Task CheckForDevicesAsync()
        {
            try
            {
                await _DeviceListener.CheckForDevicesAsync().ConfigureAwait(false);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Starts the device listener and waits for the first connected Trezor to be initialized
        /// </summary>
        /// <returns></returns>
        public async Task<T> WaitForFirstTrezorAsync(CancellationToken cancellation = new CancellationToken())
        {
            if (_DeviceListener == null) Start();
            await _DeviceListener.CheckForDevicesAsync(cancellation).ConfigureAwait(false);
            return await _FirstTrezorTaskCompletionSource.Task.ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _Lock.Dispose();
            _DeviceListener.Stop();
            _DeviceListener.Dispose();

            foreach (var trezorManager in TrezorManagers)
            {
                trezorManager.Dispose();
            }

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}



