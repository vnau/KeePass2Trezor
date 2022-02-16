using KeePass.UI;
using System;
using System.Windows.Forms;

namespace KeePass2Trezor.Forms
{
    public partial class TrezorPinPromptForm : Form
    {
        private readonly string _title;

        public string Pin { get; set; }

        public TrezorPinPromptForm(string title)
        {
            _title = title;
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            string strTitle = "Enter PIN";
            string strDesc = "Unlock Trezor using PIN.";

            this.Text = _title;
            BannerFactory.CreateBannerEx(
                this,
                m_bannerImage,
                Utility.LoadImageResource("trezor48x48.png"),
                strTitle,
                strDesc
           );

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
            Pin = pinTextBox.Text;
            this.DialogResult = DialogResult.OK;
        }
        private void OnBtnHelp(object sender, EventArgs e)
        {
            //KeePass2TrezorExt.ShowHelp(m_kpContext);
        }

        private void BtnKey_Click(object sender, EventArgs e)
        {
            pinTextBox.Text += (sender as Button).Tag.ToString();
        }

        private void BtnBackspace_Click(object sender, EventArgs e)
        {
            if (pinTextBox.Text.Length > 0)
            {
                pinTextBox.Text = pinTextBox.Text.Substring(0, pinTextBox.Text.Length - 1);
            }
        }

        private void TrezorPinPromptForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                m_btnBackspace.PerformClick();
            }
            if (e.KeyCode >= Keys.NumPad1 && e.KeyCode <= Keys.NumPad9)
            {
                pinTextBox.Text += (e.KeyCode - Keys.NumPad1 + 1).ToString();
            }
        }
    }
}
