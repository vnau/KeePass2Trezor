using KeePass.UI;
using KeePassLib.Keys;
using System;
using System.Windows.Forms;

namespace TrezorKeyProviderPlugin.Forms
{
    public partial class TrezorConnectForm : Form
    {
        string strTitle = "Connect Trezor";
        string strDesc = "Connect your Trezor device";
        string strMessage = "Connect your Trezor device";

        public void InitEx(KeyProviderQueryContext ctx)
        {
        }

        public TrezorConnectForm(string title, string desc, string message)
        {
            this.strTitle = title;
            this.strDesc = desc;
            this.strMessage = message;
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            //if (m_trezorInfo == null) { Debug.Assert(false); throw new InvalidOperationException(); }

            GlobalWindowManager.AddWindow(this);

            this.Text = strTitle;
            BannerFactory.CreateBannerEx(this, m_bannerImage,
                global::TrezorKeyProviderPlugin.Resources.trezor48x48, strTitle, strDesc);
            this.labelMessage.Text = strMessage;
            TrezorKeyProviderPluginExt.ConfigureHelpButton(m_btnHelp);
        }

        private void OnFormShown(object sender, EventArgs e)
        {
        }


        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }

        private void OnBtnOK(object sender, EventArgs e)
        {
        }

        private void OnBtnHelp(object sender, EventArgs e)
        {
            //TrezorKeyProviderPluginExt.ShowHelp(m_kpContext);
        }
    }
}
