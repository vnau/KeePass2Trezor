namespace KeePass2Trezor.Device
{
    internal class KeyDeviceStateEvent
    {
        public KeyDeviceStateEvent(KeyDeviceState state, string message = null)
        {
            State = state;
            Message = message;
        }
        public KeyDeviceState State { get; private set; }

        public string Message { get; private set; }
    }
}
