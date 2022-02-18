using KeePass.Forms;
using KeePass.Plugins;
using KeePass.Util;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using KeePassLib.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KeePass2Trezor
{
    /// <summary>
    /// Trezor Key Provider plugin class.
    /// </summary>
    public sealed class KeePass2TrezorExt : Plugin
    {
        private static IPluginHost m_host = null;
        private static TrezorKeyProvider m_prov = null;

        private const string HelpFileName = "TrezorKeyProvider_ReadMe.html";

        private delegate void IocTrezorDelegate(IOConnectionInfo ioc);

        public override Image SmallIcon
        {
            get { return Utility.LoadImageResource("trezor16x16.png"); }
        }

        public override string UpdateUrl
        {
            get { return "https://github.com/vnau/KeePass2Trezor/raw/master/keepass.version"; }
        }

        public static IPluginHost Host
        {
            get { return m_host; }
        }

        private static string g_strHelpFile = null;
        public static string HelpFile
        {
            get
            {
                if (g_strHelpFile != null) return g_strHelpFile;

                try
                {
                    string strRoot = UrlUtil.GetFileDirectory(WinUtil.GetExecutable(),
                        false, true);
                    List<string> l = UrlUtil.GetFilePaths(strRoot, HelpFileName,
                        SearchOption.AllDirectories);
                    if ((l != null) && (l.Count > 0))
                    {
                        g_strHelpFile = l[0];
                        return l[0];
                    }
                }
                catch (Exception) { Debug.Assert(false); }

                g_strHelpFile = string.Empty;
                return string.Empty;
            }
        }

        public override bool Initialize(IPluginHost host)
        {
            Debug.Assert(m_host == null);
            if (m_host != null)
            {
                Terminate();
            }

            if (host == null)
                return false;

            m_host = host;
            m_prov = new TrezorKeyProvider();

            // Event handler to store Trezor key ID in database before saving.
            m_host.MainWindow.FileSavingPre += MainWindow_FileSavingPre;

            // Add Trezor Key Provider to the Provider Pool
            m_host.KeyProviderPool.Add(m_prov);

            return true;
        }

        public override void Terminate()
        {
            if (m_host != null)
            {
                m_host.KeyProviderPool.Remove(m_prov);
                m_host.MainWindow.FileSavingPre -= MainWindow_FileSavingPre;
                m_prov = null;
                m_host = null;
            }
        }

        private void MainWindow_FileSavingPre(object sender, FileSavingEventArgs e)
        {
            bool trezorKeyProvider = e.Database.MasterKey.UserKeys.Any(k => (k is KcpCustomKey) && (k as KcpCustomKey).Name == TrezorKeyProvider.ProviderName);
            if (trezorKeyProvider)
            {
                // Add key ID to Public custom data (unencrypted) of the database.
                var keyId = TrezorKeysCache.Instance.Get(e.Database.IOConnectionInfo);
                if (keyId != null)
                {
                    // Update trezor keyID if cached a new value for the connection
                    e.Database.PublicCustomData.SetByteArray(TrezorKeysCache.TrezorPropertyKey, keyId);
                }
            }
            else
            {
                // Remove a key ID from the Public custom data (unencrypted) of the database
                // if Trezor Key Provider is not used anymore.
                e.Database.PublicCustomData.Remove(TrezorKeysCache.TrezorPropertyKey);
            }
        }

        private static bool IsHelpPresent()
        {
            return !string.IsNullOrEmpty(KeePass2TrezorExt.HelpFile);
        }

        internal static void ConfigureHelpButton(Button btn)
        {
            if (!IsHelpPresent()) btn.Enabled = false;
        }

        internal static void ConfigureHelpMenuItem(ToolStripMenuItem tsmi)
        {
            if (!IsHelpPresent()) tsmi.Enabled = false;
        }

        internal static void ShowHelp(KeyProviderQueryContext ctx)
        {
            if ((ctx != null) && ctx.IsOnSecureDesktop)
            {
                MessageService.ShowWarning("The help file cannot be displayed on the secure desktop.",
                    @"Please open the file '" + HelpFileName + @"' manually by double-clicking it in Windows Explorer.");
                return;
            }

            try { if (IsHelpPresent()) Process.Start(KeePass2TrezorExt.HelpFile); }
            catch (Exception ex) { MessageService.ShowWarning(ex.Message); }
        }
    }
}
