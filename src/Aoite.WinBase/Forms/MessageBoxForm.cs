
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

#if DEVEXPRESS
using DevExpress.XtraEditors;
#endif

namespace System.Windows.Forms
{
#if DEVEXPRESS
    using Label2 = LabelControl;
    using Button2 = SimpleButton;
    using Panel2 = PanelControl;
    using Text2 = TextEdit;
    using Check2 = CheckEdit;
    using Picture2 = PictureEdit;
    using Rich2 = MemoEdit;
    using Control2 = BaseControl;
#else
    using Label2 = Label;
    using Button2 = Button;
    using Panel2 = Panel;
    using Text2 = TextBox;
    using Check2 = CheckBox;
    using Picture2 = PictureBox;
    using Rich2 = RichTextBox;
    using Control2 = Control;
#endif
    internal partial class MessageBoxForm
    {
        public MessageBoxForm(Control owner, MessageBoxParameters paramters)
            : this(owner)
        {
#if DEVEXPRESS
            txt_Content.Properties.ScrollBars = ScrollBars.None;
#else
            txt_Content.ScrollBars = RichTextBoxScrollBars.None;
#endif
            this._parameters = paramters;
            this.InitText();
            this.InitCaption();
            this.InitIcon();
            this.InitButtons();

            this.InitInitCheck();
            this.KeyPreview = true;
        }

        private MessageBoxForm(Control owner)
        {
            InitializeComponent(owner);
        }

        public bool CheckedResult
        {
            get
            {
                return check_DonotShow.Checked;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                if (this._parameters.EscResult == System.Windows.Forms.DialogResult.None) return;
                this.DialogResult = this._parameters.EscResult;
                this.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // InitDefaultButton
        protected override void OnShown(EventArgs e)
        {
#if DEVEXPRESS
            SimpleButton btn;
#else
            Button btn;
#endif
            switch (this._parameters.DefaultButton)
            {
                case MessageBoxDefaultButton.Button1:
                    btn = ButtonIndex[0];
                    break;
                case MessageBoxDefaultButton.Button2:
                    btn = ButtonIndex[1];
                    break;
                default:
                    btn = ButtonIndex[2];
                    break;
            }
            btn.Focus();
            this.AcceptButton = btn;
        }

        private void CloseByRresult(DialogResult result)
        {
            this.DialogResult = result;
            this.Close();
        }

        private void InitButton(Button2 button, MessageBoxButtonParameters parameters)
        {
            button.DialogResult = parameters.Result;
            button.Visible = parameters.Visible;
            if (button.Visible) this.SetStyle(button, parameters);
            button.Text = parameters.Text;
        }

        private void InitButtons()
        {

            this.simpleButton3.Click += new System.EventHandler(this.simpleButton1_Click);
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton1_Click);
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            switch (this._parameters.Buttons)
            {
                case MessageBoxButtons.YesNoCancel:
                case MessageBoxButtons.AbortRetryIgnore:
                    ButtonIndex[0] = simpleButton3;
                    ButtonIndex[1] = simpleButton2;
                    ButtonIndex[2] = simpleButton1;
                    break;
                case MessageBoxButtons.OKCancel:
                case MessageBoxButtons.RetryCancel:
                case MessageBoxButtons.YesNo:
                    ButtonIndex[0] = simpleButton2;
                    ButtonIndex[1] = simpleButton1;
                    ButtonIndex[2] = simpleButton3;
                    break;
                case MessageBoxButtons.OK:
                default:
                    ButtonIndex[0] = simpleButton1;
                    ButtonIndex[1] = simpleButton2;
                    ButtonIndex[2] = simpleButton3;
                    break;
            }
            this.InitButton(ButtonIndex[0], this._parameters.Button1);
            this.InitButton(ButtonIndex[1], this._parameters.Button2);
            this.InitButton(ButtonIndex[2], this._parameters.Button3);
        }

        private void InitCaption()
        {
            this.SetStyle(lbl_Caption, this._parameters.Caption);
            if (this._parameters.Caption.BackColor.HasValue)
            {
                panel_Caption.BackColor = this._parameters.Caption.BackColor.Value;
            }

            var paramters = this._parameters.Form;
            if (paramters.Font != null) this.Font = paramters.Font;
            if (paramters.ForeColor.HasValue) this.ForeColor = paramters.ForeColor.Value;
            if (paramters.BackColor.HasValue) this.BackColor = paramters.BackColor.Value;
        }


        private void InitIcon()
        {
            var img = this._parameters.Icon.Image;

            if (img == null)
            {
                lbl_Caption.Dock = DockStyle.Fill;
            }
            else
            {
                if (img == null) return;
                img.MakeTransparent(Color.White);
                pic_Icon.Image = img;

                if (this._parameters.Caption.ForeColor.HasValue) return;

                switch (this._parameters.Icon.MessageBoxIcon)
                {
                    case MessageBoxIconEx.Error:
                        lbl_Caption.ForeColor = Color.Red;
                        break;
                    case MessageBoxIconEx.Warning:
                        lbl_Caption.ForeColor = Color.OrangeRed;
                        break;
                    default:
                        break;
                }
            }
        }

        private void InitInitCheck()
        {
            if (this._parameters.Check.Visible)
            {
                this.SetStyle(check_DonotShow, this._parameters.Check);
                if (this._parameters.Check.BackColor.HasValue)
                {
                    panel_Footer.BackColor = this._parameters.Check.BackColor.Value;
                }
                check_DonotShow.Text = this._parameters.Check.Text;
            }
            else
            {
                check_DonotShow.Visible = false;
            }
        }

        private void InitText()
        {
            this.SetStyle(txt_Content, this._parameters.Content);
            if (this._parameters.Content.BackColor.HasValue)
            {
                panel_Content.BackColor = this._parameters.Content.BackColor.Value;
            }

            using (Graphics g = txt_Content.CreateGraphics())
            {
                SizeF sizef = g.MeasureString(txt_Content.Text, txt_Content.Font, txt_Content.Width);
                var height = (int)Math.Ceiling(sizef.Height);
                // var height = size.Height;
                var maxHeight = txt_Content.Font.Height * 5;
#if DEVEXPRESS
                if (height < 30) height = 30;
#else
                if (height < 16) height = 16;
#endif
                if (height >= maxHeight)
                {
                    height = maxHeight;
                    txt_Content.Tag = true;
                }

                this.Height = panel_Caption.Height + panel_Footer.Height
                  + panel_Content.Height + height 
#if DEVEXPRESS
                  - 30
#endif
                  + panel_Content.Padding.Top + panel_Content.Padding.Bottom;//194
            }

        }

        private void SetStyle(Control2 control, MessageBoxTextParameters paramters)
        {
            control.Text = paramters.Text;
            if (paramters.Font != null) control.Font = paramters.Font;
            if (paramters.ForeColor.HasValue) control.ForeColor = paramters.ForeColor.Value;
            if (paramters.BackColor.HasValue) control.BackColor = paramters.BackColor.Value;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.CloseByRresult((sender as Button2).DialogResult);
        }

        private void txt_Content_Enter(object sender, EventArgs e)
        {
            if (txt_Content.Tag != null)
            {
#if DEVEXPRESS
                txt_Content.Properties.ScrollBars = ScrollBars.Vertical;
            }
            txt_Content.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
#else
                txt_Content.ScrollBars = RichTextBoxScrollBars.Both;
            }
            txt_Content.BorderStyle = BorderStyle.Fixed3D;
#endif
        }

        private void txt_Content_Leave(object sender, EventArgs e)
        {
            if (txt_Content.Tag != null)
            {
#if DEVEXPRESS
                txt_Content.Properties.ScrollBars = ScrollBars.None;
            }
            txt_Content.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
#else
                txt_Content.ScrollBars = RichTextBoxScrollBars.None;
            }
            txt_Content.BorderStyle = BorderStyle.None;
#endif
        }

        private void txt_Content_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.AcceptButton.PerformClick();
                e.Handled = true;
            }
        }

    }
    internal partial class MessageBoxForm

#if DEVEXPRESS
 : XtraForm
#else
 : Form
#endif
    {
        private IContainer components = null;
        private MessageBoxParameters _parameters;
        private Check2 check_DonotShow;
        private Label2 lbl_Caption;
        private Panel2 panel_Caption;
        private Panel2 panel_Content;
        private Panel2 panel_Footer;
        private Picture2 pic_Icon;
        private Button2 simpleButton1;
        private Button2 simpleButton2;
        private Button2 simpleButton3;
        private Rich2 txt_Content;
        private Button2[] ButtonIndex = new Button2[3];

        private void InitializeComponent(Control owner)
        {
            this.lbl_Caption = new Label2();
            this.pic_Icon = new Picture2();
            this.panel_Footer = new Panel2();
            this.check_DonotShow = new Check2();
            this.simpleButton3 = new Button2();
            this.simpleButton2 = new Button2();
            this.simpleButton1 = new Button2();
            this.panel_Content = new Panel2();
            this.txt_Content = new Rich2();
            this.panel_Caption = new Panel2();
#if DEVEXPRESS
            ((System.ComponentModel.ISupportInitialize)(this.panel_Caption)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Icon.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel_Footer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.check_DonotShow.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel_Content)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Content.Properties)).BeginInit();
#endif
            this.panel_Caption.SuspendLayout();
            this.panel_Footer.SuspendLayout();
            this.panel_Content.SuspendLayout();
            this.check_DonotShow.SuspendLayout();
            this.SuspendLayout();
#if DEVEXPRESS
            var noBorder = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
#else
            var noBorder = BorderStyle.None;
#endif
            //
            // panel_Caption
            //
            this.panel_Caption.BorderStyle = noBorder;
            this.panel_Caption.Controls.Add(this.lbl_Caption);
            this.panel_Caption.Controls.Add(this.pic_Icon);
            this.panel_Caption.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Caption.Location = new System.Drawing.Point(8, 8);
            this.panel_Caption.Name = "panel_Caption";
            this.panel_Caption.Size = new System.Drawing.Size(478, 64);
            this.panel_Caption.TabIndex = 0;
            //
            // lbl_Caption
            //
#if DEVEXPRESS
            this.lbl_Caption.AllowHtmlString = true;
            this.lbl_Caption.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lbl_Caption.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lbl_Caption.Appearance.Options.UseFont = true;
            this.lbl_Caption.Appearance.Options.UseForeColor = true;
            this.lbl_Caption.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
#else
            this.lbl_Caption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lbl_Caption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lbl_Caption.TextAlign = ContentAlignment.MiddleLeft;
            this.lbl_Caption.AutoSize = false;
#endif
            this.lbl_Caption.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbl_Caption.Location = new System.Drawing.Point(64, 0);
            this.lbl_Caption.Name = "lbl_Caption";
            this.lbl_Caption.Size = new System.Drawing.Size(414, 64);
            this.lbl_Caption.TabIndex = 1;
            this.lbl_Caption.Text = "标题";
            //
            // pic_Icon
            //
            this.pic_Icon.Location = new System.Drawing.Point(14, 14);
            this.pic_Icon.Name = "pic_Icon";
#if DEVEXPRESS
            this.pic_Icon.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pic_Icon.Properties.Appearance.Options.UseBackColor = true;
            this.pic_Icon.Properties.BorderStyle = noBorder;
            this.pic_Icon.Properties.ShowMenu = false;
#else
            this.pic_Icon.BackColor = System.Drawing.Color.Transparent;
            this.pic_Icon.BorderStyle = noBorder;
#endif
            this.pic_Icon.Size = new System.Drawing.Size(36, 36);
            this.pic_Icon.TabIndex = 0;
            //
            // panel_Footer
            //
            this.panel_Footer.BorderStyle = noBorder;
            this.panel_Footer.Controls.Add(this.check_DonotShow);
            this.panel_Footer.Controls.Add(this.simpleButton3);
            this.panel_Footer.Controls.Add(this.simpleButton2);
            this.panel_Footer.Controls.Add(this.simpleButton1);
            this.panel_Footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_Footer.Location = new System.Drawing.Point(8, 143);
            this.panel_Footer.Name = "panel_Footer";
            this.panel_Footer.Size = new System.Drawing.Size(478, 35);
            this.panel_Footer.TabIndex = 1;
            //
            // check_DonotShow
            //
            this.check_DonotShow.Location = new System.Drawing.Point(3, 7);
            this.check_DonotShow.Name = "check_DonotShow";
#if DEVEXPRESS
            this.check_DonotShow.Properties.Caption = "不再显示";
#else
            this.check_DonotShow.Text = "不再显示";
#endif
            this.check_DonotShow.AutoSize = true;
            this.check_DonotShow.Size = new System.Drawing.Size(75, 19);
            this.check_DonotShow.TabIndex = 3;
            //
            // simpleButton3
            //
            this.simpleButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton3.Location = new System.Drawing.Point(234, 6);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Size = new System.Drawing.Size(75, 25);
            this.simpleButton3.TabIndex = 2;
            this.simpleButton3.Text = "Three";
            //
            // simpleButton2
            //
            this.simpleButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton2.Location = new System.Drawing.Point(315, 6);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 25);
            this.simpleButton2.TabIndex = 1;
            this.simpleButton2.Text = "Tow";
            //
            // simpleButton1
            //
            this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton1.Location = new System.Drawing.Point(396, 6);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 25);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "One";
            //
            // panel_Content
            //
#if DEVEXPRESS
            this.panel_Content.Appearance.BackColor = System.Drawing.Color.White;
            this.panel_Content.Appearance.Options.UseBackColor = true;
#else
            this.panel_Content.BackColor = System.Drawing.Color.White;
#endif
            this.panel_Content.BorderStyle = noBorder;
            this.panel_Content.Controls.Add(this.txt_Content);
            this.panel_Content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Content.Location = new System.Drawing.Point(8, 72);
            this.panel_Content.Name = "panel_Content";
            this.panel_Content.Padding = new System.Windows.Forms.Padding(32, 24, 32, 24);
            this.panel_Content.Size = new System.Drawing.Size(478, 71);
            this.panel_Content.TabIndex = 2;
            //
            // txt_Content
            //
            this.txt_Content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_Content.Location = new System.Drawing.Point(32, 24);
            this.txt_Content.Name = "txt_Content";
#if DEVEXPRESS
            this.txt_Content.EditValue = "";
            this.txt_Content.Properties.Appearance.BackColor = System.Drawing.SystemColors.Window;
            this.txt_Content.Properties.Appearance.Font = new System.Drawing.Font("新宋体", 9F);
            this.txt_Content.Properties.Appearance.Options.UseBackColor = true;
            this.txt_Content.Properties.Appearance.Options.UseFont = true;
            this.txt_Content.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.txt_Content.Properties.ReadOnly = true;
#else
            this.txt_Content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_Content.Text = "";
            this.txt_Content.Location = new System.Drawing.Point(32, 24);
            this.txt_Content.Name = "txt_Content";
            this.txt_Content.BackColor = System.Drawing.SystemColors.Window;
            this.txt_Content.Font = new System.Drawing.Font("新宋体", 9F);
            this.txt_Content.BorderStyle = BorderStyle.None;
            this.txt_Content.ReadOnly = true;
#endif
            this.txt_Content.Size = new System.Drawing.Size(414, 23);
            this.txt_Content.TabIndex = 2;
            this.txt_Content.Enter += new System.EventHandler(this.txt_Content_Enter);
            this.txt_Content.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Content_KeyDown);
            this.txt_Content.Leave += new System.EventHandler(this.txt_Content_Leave);
            //
            // MessageBoxForm
            //
            this.AcceptButton = this.simpleButton1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 186);
            this.Controls.Add(this.panel_Content);
            this.Controls.Add(this.panel_Footer);
            this.Controls.Add(this.panel_Caption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageBoxForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = (owner == null) ? System.Windows.Forms.FormStartPosition.CenterScreen : FormStartPosition.CenterParent;
#if DEVEXPRESS
            this.LookAndFeel.SkinName = "Money Twins";
            ((System.ComponentModel.ISupportInitialize)(this.panel_Caption)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Icon.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel_Footer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.check_DonotShow.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel_Content)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Content.Properties)).EndInit();
#endif
            this.panel_Caption.ResumeLayout(false);
            this.panel_Footer.ResumeLayout(false);
            this.check_DonotShow.ResumeLayout(false);
            this.panel_Content.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}