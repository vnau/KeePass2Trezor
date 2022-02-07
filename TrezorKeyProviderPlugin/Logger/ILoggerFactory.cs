#if TREZORNET4
using Device.Net;

namespace TrezorKeyProviderPlugin.Logger
{
    interface ILoggerFactory
    {
        ILogger CreateLogger(string name);
    }

}
#endif
