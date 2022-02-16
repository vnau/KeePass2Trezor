using KeePass.UI;
using System;
using System.Windows.Forms;

namespace KeePass2Trezor.Forms
{
    public partial class TrezorConnectForm : Form
    {
        private readonly string _title;
        private readonly string _caption;
        private readonly string _description;
        private readonly string _message;

        public TrezorConnectForm(string title, string caption, string desc, string message)
        {
            _title = title;
            _caption = caption;
            _description = desc;
            _message = message;
            InitializeComponent();
            this.pictureBox1.Image = Utility.LoadImageResource("icon-trezorT.png");
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            this.Text = _title;
            BannerFactory.CreateBannerEx(
                this,
                m_bannerImage,
                Utility.LoadImageResource("trezor48x48.png"),
                _caption,
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
