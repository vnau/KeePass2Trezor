using System;
using System.Threading.Tasks;

namespace KeePass2Trezor.Device
{
    internal interface IKeyProviderDevice : IDisposable
    {
        KeyDeviceState State { get; }
        string StateMessage { get; }

        event EventHandler<KeyDeviceStateEvent> OnChangeState;

        void Close();

        Task<byte[]> GetKeyByRequest(string request);

        void SetPin(string pin);
    }
}