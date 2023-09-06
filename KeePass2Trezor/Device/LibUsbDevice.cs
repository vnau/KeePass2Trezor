using Device.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KeePass2Trezor.Device
{
    internal class LibUsbDevice : IDevice
    {
        private readonly IDevice device;

        public LibUsbDevice(IDevice device)
        {
            this.device = device;
        }
        public bool IsInitialized
        {
            get { return device.IsInitialized; }
        }

        public string DeviceId
        {
            get { return device.DeviceId; }
        }

        public ConnectedDeviceDefinition ConnectedDeviceDefinition
        {
            get { return device.ConnectedDeviceDefinition; }
        }

        public void Close()
        {
            device.Close();
        }

        public void Dispose()
        {
            device.Dispose();
        }

        public Task Flush(CancellationToken cancellationToken)
        {
            return device.Flush(cancellationToken);
        }

        public Task InitializeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return device.InitializeAsync(cancellationToken);
        }

        public async Task<TransferResult> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            while (true)
            {
                var res = await device.ReadAsync(cancellationToken);
                if (res.BytesTransferred > 0)
                {
                    return res;
                }
                await Task.Delay(100, cancellationToken);
            }
        }

        public async Task<TransferResult> WriteAndReadAsync(byte[] writeBuffer, CancellationToken cancellationToken = default(CancellationToken))
        {

            return await device.WriteAndReadAsync(writeBuffer, cancellationToken);
        }

        public async Task<uint> WriteAsync(byte[] data, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await device.WriteAsync(data, cancellationToken);
        }
    }
}
