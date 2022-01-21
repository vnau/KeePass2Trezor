namespace TrezorKeyProviderPlugin.Hardware
{
    public class TrezorStateEvent
    {
        public TrezorStateEvent(TrezorState state, string message = null)
        {
            State = state;
            Message = message;
        }
        public TrezorState State { get; private set; }

        public string Message { get; private set; }
    }
}
