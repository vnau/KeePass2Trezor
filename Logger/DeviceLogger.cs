using System;

namespace TrezorKeyProviderPlugin.Logger
{
    class DeviceLogger : ILogger
    {
        private readonly string filename;
        public DeviceLogger(string filename)
        {
            this.filename = filename;
        }


        public void Log(string message, string region, Exception ex, LogLevel logLevel)
        {
            lock (filename)
            {
                //System.IO.File.AppendAllText(filename, message + "\r\n");
            }
        }

    }
}
