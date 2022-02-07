namespace TrezorKeyProviderPlugin.Device
{
    internal interface IDeviceStateEventReceiver
    {
        void KeyDeviceEventFired(KeyDeviceStateEvent e);
    }
}
