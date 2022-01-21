namespace TrezorKeyProviderPlugin.Hardware
{
    public enum TrezorState
        {
            Disconnected,
            Connected,
            ButtonRequest,
            Confirmed,
            WaitPin,
            WaitPassfrase,
            Error,
            Processing,
        }
}
