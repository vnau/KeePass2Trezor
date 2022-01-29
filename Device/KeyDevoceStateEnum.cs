namespace TrezorKeyProviderPlugin.Device
{
    public enum KeyDeviceState
    {
        Disconnected,
        Connected,
        Confirmed,
        Processing,
        WaitPIN,
        WaitPassphrase,
        WaitConfirmation,
        Error,
    }
}
