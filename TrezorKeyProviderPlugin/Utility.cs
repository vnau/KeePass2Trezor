using System.Drawing;
using System.Reflection;

namespace TrezorKeyProviderPlugin
{
    class Utility
    {
        internal static Image LoadImageResource(string path)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (var ms = assembly.GetManifestResourceStream("TrezorKeyProviderPlugin.Images." + path))
            {
                return Image.FromStream(ms, true);
            }
        }
    }
}
