namespace TrezorKeyProviderPlugin.Hardware
{
    public enum TrezorState
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
