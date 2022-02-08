namespace KeePass2Trezor.Device
{
    internal enum KeyDeviceState
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
