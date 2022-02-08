#if TREZORNET4
using Device.Net;

namespace KeePass2Trezor.Logger
{
    interface ILoggerFactory
    {
        ILogger CreateLogger(string name);
    }

}
#endif
