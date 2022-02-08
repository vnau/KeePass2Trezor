using KeePass.UI;
using KeePassLib.Keys;
using System;
using System.Windows.Forms;

namespace KeePass2Trezor.Forms
{
    public partial class TrezorConnectForm : Form
    {
        private readonly string _title = "Connect Trezor";
        private readonly string _description = "Connect your Trezor device";
        private readonly string _message = "Connect your Trezor device";

        public TrezorConnectForm(string title, string desc, string message)
        {
            _title = title;
            _description = desc;
            _message = message;
            InitializeComponent();
            this.pictureBox1.Image = Utility.LoadImageResource("icon-trezorT.png");
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            //if (m_trezorInfo == null) { Debug.Assert(false); throw new InvalidOperationException(); }

            GlobalWindowManager.AddWindow(this);

            this.Text = _title;
            BannerFactory.CreateBannerEx(
                this,
                m_bannerImage,
                Utility.LoadImageResource("trezor48x48.png"),
                _title,
                _description
            );
            this.labelMessage.Text = _message;
            KeePass2TrezorExt.ConfigureHelpButton(m_btnHelp);
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
        }
    }
}
