﻿namespace KeePass2Trezor.Forms
{
    partial class TrezorPinPromptForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_bannerImage = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_btnKey1 = new System.Windows.Forms.Button();
            this.m_btnKey2 = new System.Windows.Forms.Button();
            this.m_btnKey3 = new System.Windows.Forms.Button();
            this.m_btnKey7 = new System.Windows.Forms.Button();
            this.m_btnKey8 = new System.Windows.Forms.Button();
            this.m_btnKey9 = new System.Windows.Forms.Button();
            this.m_btnKey4 = new System.Windows.Forms.Button();
            this.m_btnKey5 = new System.Windows.Forms.Button();
            this.m_btnKey6 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_btnBackspace = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pinTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(289, 60);
            this.m_bannerImage.TabIndex = 2;
            this.m_bannerImage.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.m_btnKey1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_btnKey2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_btnKey3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_btnKey7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_btnKey8, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_btnKey9, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_btnKey4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_btnKey5, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_btnKey6, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 120);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(8);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(289, 206);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // m_btnKey1
            // 
            this.m_btnKey1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnKey1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.m_btnKey1.Location = new System.Drawing.Point(11, 137);
            this.m_btnKey1.Name = "m_btnKey1";
            this.m_btnKey1.Size = new System.Drawing.Size(85, 58);
            this.m_btnKey1.TabIndex = 10;
            this.m_btnKey1.TabStop = false;
            this.m_btnKey1.Tag = "1";
            this.m_btnKey1.Text = "●";
            this.m_btnKey1.UseVisualStyleBackColor = true;
            this.m_btnKey1.Click += new System.EventHandler(this.BtnKey_Click);
            // 
            // m_btnKey2
            // 
            this.m_btnKey2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnKey2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.m_btnKey2.Location = new System.Drawing.Point(102, 137);
            this.m_btnKey2.Name = "m_btnKey2";
            this.m_btnKey2.Size = new System.Drawing.Size(85, 58);
            this.m_btnKey2.TabIndex = 11;
            this.m_btnKey2.TabStop = false;
            this.m_btnKey2.Tag = "2";
            this.m_btnKey2.Text = "●";
            this.m_btnKey2.UseVisualStyleBackColor = true;
            this.m_btnKey2.Click += new System.EventHandler(this.BtnKey_Click);
            // 
            // m_btnKey3
            // 
            this.m_btnKey3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnKey3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.m_btnKey3.Location = new System.Drawing.Point(193, 137);
            this.m_btnKey3.Name = "m_btnKey3";
            this.m_btnKey3.Size = new System.Drawing.Size(85, 58);
            this.m_btnKey3.TabIndex = 12;
            this.m_btnKey3.TabStop = false;
            this.m_btnKey3.Tag = "3";
            this.m_btnKey3.Text = "●";
            this.m_btnKey3.UseVisualStyleBackColor = true;
            this.m_btnKey3.Click += new System.EventHandler(this.BtnKey_Click);
            // 
            // m_btnKey7
            // 
            this.m_btnKey7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnKey7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.m_btnKey7.Location = new System.Drawing.Point(11, 11);
            this.m_btnKey7.Name = "m_btnKey7";
            this.m_btnKey7.Size = new System.Drawing.Size(85, 57);
            this.m_btnKey7.TabIndex = 4;
            this.m_btnKey7.TabStop = false;
            this.m_btnKey7.Tag = "7";
            this.m_btnKey7.Text = "●";
            this.m_btnKey7.UseVisualStyleBackColor = true;
            this.m_btnKey7.Click += new System.EventHandler(this.BtnKey_Click);
            // 
            // m_btnKey8
            // 
            this.m_btnKey8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnKey8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.m_btnKey8.Location = new System.Drawing.Point(102, 11);
            this.m_btnKey8.Name = "m_btnKey8";
            this.m_btnKey8.Size = new System.Drawing.Size(85, 57);
            this.m_btnKey8.TabIndex = 5;
            this.m_btnKey8.TabStop = false;
            this.m_btnKey8.Tag = "8";
            this.m_btnKey8.Text = "●";
            this.m_btnKey8.UseVisualStyleBackColor = true;
            this.m_btnKey8.Click += new System.EventHandler(this.BtnKey_Click);
            // 
            // m_btnKey9
            // 
            this.m_btnKey9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnKey9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.m_btnKey9.Location = new System.Drawing.Point(193, 11);
            this.m_btnKey9.Name = "m_btnKey9";
            this.m_btnKey9.Size = new System.Drawing.Size(85, 57);
            this.m_btnKey9.TabIndex = 6;
            this.m_btnKey9.TabStop = false;
            this.m_btnKey9.Tag = "9";
            this.m_btnKey9.Text = "●";
            this.m_btnKey9.UseVisualStyleBackColor = true;
            this.m_btnKey9.Click += new System.EventHandler(this.BtnKey_Click);
            // 
            // m_btnKey4
            // 
            this.m_btnKey4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnKey4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.m_btnKey4.Location = new System.Drawing.Point(11, 74);
            this.m_btnKey4.Name = "m_btnKey4";
            this.m_btnKey4.Size = new System.Drawing.Size(85, 57);
            this.m_btnKey4.TabIndex = 7;
            this.m_btnKey4.TabStop = false;
            this.m_btnKey4.Tag = "4";
            this.m_btnKey4.Text = "●";
            this.m_btnKey4.UseVisualStyleBackColor = true;
            this.m_btnKey4.Click += new System.EventHandler(this.BtnKey_Click);
            // 
            // m_btnKey5
            // 
            this.m_btnKey5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnKey5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.m_btnKey5.Location = new System.Drawing.Point(102, 74);
            this.m_btnKey5.Name = "m_btnKey5";
            this.m_btnKey5.Size = new System.Drawing.Size(85, 57);
            this.m_btnKey5.TabIndex = 8;
            this.m_btnKey5.TabStop = false;
            this.m_btnKey5.Tag = "5";
            this.m_btnKey5.Text = "●";
            this.m_btnKey5.UseVisualStyleBackColor = true;
            this.m_btnKey5.Click += new System.EventHandler(this.BtnKey_Click);
            // 
            // m_btnKey6
            // 
            this.m_btnKey6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnKey6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.m_btnKey6.Location = new System.Drawing.Point(193, 74);
            this.m_btnKey6.Name = "m_btnKey6";
            this.m_btnKey6.Size = new System.Drawing.Size(85, 57);
            this.m_btnKey6.TabIndex = 9;
            this.m_btnKey6.TabStop = false;
            this.m_btnKey6.Tag = "6";
            this.m_btnKey6.Text = "●";
            this.m_btnKey6.UseVisualStyleBackColor = true;
            this.m_btnKey6.Click += new System.EventHandler(this.BtnKey_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 326);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(289, 55);
            this.panel1.TabIndex = 16;


            // 
            // m_btnOK
            // 
            this.m_btnOK.AutoSize = true;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_btnOK.Location = new System.Drawing.Point(10, 0);
            this.m_btnOK.Margin = new System.Windows.Forms.Padding(3, 3, 3, 30);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(269, 45);
            this.m_btnOK.TabIndex = 15;
            this.m_btnOK.Text = "&Enter PIN";
            this.m_btnOK.UseVisualStyleBackColor = true;
            this.m_btnOK.Click += new System.EventHandler(this.OnBtnOK);
            
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.m_btnHelp);
            this.panel2.Controls.Add(this.m_btnBackspace);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.pinTextBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 60);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(10);
            this.panel2.Size = new System.Drawing.Size(289, 60);
            this.panel2.TabIndex = 17;
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Location = new System.Drawing.Point(127, 23);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 19;
            this.m_btnHelp.Text = "&Help";
            this.m_btnHelp.UseVisualStyleBackColor = true;
            this.m_btnHelp.Visible = false;
            // 
            // m_btnBackspace
            // 
            this.m_btnBackspace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnBackspace.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_btnBackspace.Location = new System.Drawing.Point(246, 25);
            this.m_btnBackspace.Name = "m_btnBackspace";
            this.m_btnBackspace.Size = new System.Drawing.Size(33, 32);
            this.m_btnBackspace.TabIndex = 18;
            this.m_btnBackspace.TabStop = false;
            this.m_btnBackspace.Text = "←";
            this.m_btnBackspace.UseVisualStyleBackColor = true;
            this.m_btnBackspace.Click += new System.EventHandler(this.BtnBackspace_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(234, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Check your Trezor screen for the keypad layout.";
            // 
            // pinTextBox
            // 
            this.pinTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pinTextBox.Enabled = false;
            this.pinTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pinTextBox.Location = new System.Drawing.Point(10, 25);
            this.pinTextBox.Name = "pinTextBox";
            this.pinTextBox.ReadOnly = true;
            this.pinTextBox.Size = new System.Drawing.Size(231, 32);
            this.pinTextBox.TabIndex = 17;
            this.pinTextBox.TabStop = false;
            this.pinTextBox.UseSystemPasswordChar = true;
            this.pinTextBox.WordWrap = false;
            // 
            // TrezorPinPromptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(289, 381);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.m_bannerImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrezorPinPromptForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TrezorPinPromptForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.TextBox pinTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button m_btnKey8;
        private System.Windows.Forms.Button m_btnKey9;
        private System.Windows.Forms.Button m_btnKey4;
        private System.Windows.Forms.Button m_btnKey7;
        private System.Windows.Forms.Button m_btnKey1;
        private System.Windows.Forms.Button m_btnKey2;
        private System.Windows.Forms.Button m_btnKey3;
        private System.Windows.Forms.Button m_btnKey5;
        private System.Windows.Forms.Button m_btnKey6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button m_btnBackspace;
        private System.Windows.Forms.Button m_btnHelp;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}