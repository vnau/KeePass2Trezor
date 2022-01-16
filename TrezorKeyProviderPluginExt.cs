using KeePass.Forms;
using KeePass.Plugins;
using KeePass.Util;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using KeePassLib.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TrezorKeyProviderPlugin
{
    public sealed class TrezorKeyProviderPluginExt : Plugin
    {
        private static IPluginHost m_host = null;
        private static TrezorKeyProvider m_prov = null;

        private const string HelpFileName = "TrezorKeyProvider_ReadMe.html";

        private delegate void IocTrezorDelegate(IOConnectionInfo ioc);

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
            if (m_host != null)
            {
                Debug.Assert(false);
                Terminate();
            }
            if (host == null)
                return false;

            m_host = host;
            m_prov = new TrezorKeyProvider();
            m_host.KeyProviderPool.Add(m_prov);

            return true;
        }

        public override void Terminate()
        {
            if (m_host != null)
            {
                m_host.KeyProviderPool.Remove(m_prov);
                m_prov = null;
                m_host = null;
            }
        }

        private static bool IsHelpPresent()
        {
            return !string.IsNullOrEmpty(TrezorKeyProviderPluginExt.HelpFile);
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

            try { if (IsHelpPresent()) Process.Start(TrezorKeyProviderPluginExt.HelpFile); }
            catch (Exception ex) { MessageService.ShowWarning(ex.Message); }
        }
    }
}
