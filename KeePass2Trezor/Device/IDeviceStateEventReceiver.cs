namespace KeePass2Trezor.Device
{
    internal interface IDeviceStateEventReceiver
    {
        void KeyDeviceEventFired(KeyDeviceStateEvent e);
    }
}
