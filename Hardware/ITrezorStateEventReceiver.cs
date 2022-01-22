namespace TrezorKeyProviderPlugin.Hardware
{
    interface ITrezorStateEventReceiver
    {
        void TrezorEventFired(TrezorStateEvent e);
    }
}
