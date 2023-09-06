using Device.Net;
using Device.Net.LibUsb;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trezor.Net;

namespace KeePass2Trezor.Device
{
    internal class LibUsbDeviceFactory : IDeviceFactory
    {
        private readonly IDeviceFactory deviceFactory;

        public LibUsbDeviceFactory(ILoggerFactory loggerFactory)
        {
            this.deviceFactory = TrezorManager.DeviceDefinitions.CreateLibUsbDeviceFactory(loggerFactory);
        }

        public async Task<IEnumerable<ConnectedDeviceDefinition>> GetConnectedDeviceDefinitionsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await deviceFactory.GetConnectedDeviceDefinitionsAsync(cancellationToken);
        }

        public async Task<IDevice> GetDeviceAsync(ConnectedDeviceDefinition connectedDeviceDefinition, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new LibUsbDevice(await deviceFactory.GetDeviceAsync(connectedDeviceDefinition, cancellationToken));
        }

        public async Task<bool> SupportsDeviceAsync(ConnectedDeviceDefinition connectedDeviceDefinition, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await deviceFactory.SupportsDeviceAsync(connectedDeviceDefinition, cancellationToken);
        }
    }
}
