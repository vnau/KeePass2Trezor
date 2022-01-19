namespace TrezorKeyProviderPlugin
{
    public class TrezorStateEvent
    {
        public TrezorStateEvent(TrezorDevice.TrezorState state, string message = null)
        {
            State = state;
            Message = message;
        }
        public TrezorDevice.TrezorState State { get; }

        public string Message { get; }
    }
}
