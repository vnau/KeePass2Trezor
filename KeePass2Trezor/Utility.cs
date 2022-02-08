using System.Drawing;
using System.Reflection;

namespace KeePass2Trezor
{
    class Utility
    {
        internal static Image LoadImageResource(string path)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (var ms = assembly.GetManifestResourceStream("KeePass2Trezor.Images." + path))
            {
                return Image.FromStream(ms, true);
            }
        }
    }
}
