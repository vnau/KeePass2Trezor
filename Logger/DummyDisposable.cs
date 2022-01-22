using System;

namespace TrezorKeyProviderPlugin.Logger
{
    class DummyDisposable : IDisposable
    {
        public void Dispose() { }
    }
}
