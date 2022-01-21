namespace TrezorKeyProviderPlugin.Hardware
{
    public partial class TrezorDevice
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
}
