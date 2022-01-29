namespace TrezorKeyProviderPlugin.Device
{
    interface IDeviceStateEventReceiver
    {
        void KeyDeviceEventFired(KeyDeviceStateEvent e);
    }
}
