
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

#if DEVEXPRESS
using DevExpress.XtraEditors;
#endif

namespace System.Windows.Forms
{

#if DEVEXPRESS
    using Panel2 = PanelControl;
#else
    using Panel2 = Panel;
#endif
    internal class MessageLabelForm : NoFocusedForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lbl_ICON;
        private Label lbl_Content;
        private Panel2 panelControl1;

        private int _closeEnd;
        private int _closeIndex = 0;
        private Timer _timer = new Timer();

        internal MessageLabelForm(Control owner, MessageLabelParameters parameters)
        {
            this.InitializeComponent(parameters);

            var size = this.RectangleToScreen(this.DisplayRectangle).Size;
            this.AutoSize = false;
            lbl_Content.AutoSize = false;
            this.Size = size;
            if (parameters.Point.HasValue)
            {
                this.Location = parameters.Point.Value;
            }
            else
            {
                if (owner == null) owner = System.Windows.Forms.Form.ActiveForm;
                if (owner == null)
                {
                    this.StartPosition = FormStartPosition.CenterScreen;
                }
                else
                {
                    Form ownerForm = owner.FindForm();
                    if (ownerForm.ParentForm != null) ownerForm = ownerForm.ParentForm;
                    if (ownerForm.MdiParent != null) ownerForm = ownerForm.MdiParent;

                    this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;

                    if (parameters.IsAlignmentScreen)
                    {
                        this.Location = this.GetPoint(Screen.GetWorkingArea(ownerForm), parameters.Alignment);
                    }
                    else
                    {
                        this.Location = this.GetPoint(ownerForm.RectangleToScreen(ownerForm.ClientRectangle), parameters.Alignment);
                    }
                }
            }
            this._closeEnd = parameters.CloseMillisecond;
            this._timer.Interval = 100;
            this._timer.Tick += new EventHandler(timer_Tick);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_timer.Enabled) _timer.Stop();
            base.OnClosing(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            _timer.Start();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Point GetPoint(Rectangle ownerRect, ContentAlignment alignment)
        {
            int x = ownerRect.X, y = ownerRect.Y;

            var size = this.ClientSize;

            switch (alignment)
            {
                case ContentAlignment.BottomCenter:
                    x += (ownerRect.Width - size.Width) / 2;
                    y += ownerRect.Height - size.Height;
                    break;
                case ContentAlignment.BottomLeft:
                    //x = 0;
                    y += ownerRect.Height - size.Height;
                    break;
                case ContentAlignment.BottomRight:
                    x += ownerRect.Width - size.Width;
                    y += ownerRect.Height - size.Height;
                    break;
                case ContentAlignment.MiddleCenter:
                    x += (ownerRect.Width - size.Width) / 2;
                    y += (ownerRect.Height - size.Height) / 2;
                    break;
                case ContentAlignment.MiddleLeft:
                    //x=0;
                    y += (ownerRect.Height - size.Height) / 2;
                    break;
                case ContentAlignment.MiddleRight:
                    x += ownerRect.Width - size.Width;
                    y += (ownerRect.Height - size.Height) / 2;
                    break;
                case ContentAlignment.TopCenter:
                    x += (ownerRect.Width - size.Width) / 2;
                    //y = 0;
                    break;
                case ContentAlignment.TopRight:
                    x += ownerRect.Width - size.Width;
                    //y = 0;
                    break;
                //case ContentAlignment.TopLeft:
                //default:
                //    return new Point(0, 0);
            }
            return new Point(x, y - 5);
        }

        private void InitializeComponent(MessageLabelParameters parameters)
        {
            this.components = new System.ComponentModel.Container();
            var form = parameters.Form;
            var content = parameters.Content;
            var icon = parameters.Icon;
            var errorIcon = icon.MessageBoxIcon == MessageBoxIconEx.Error;

            this.lbl_Content = new Label();
            this.panelControl1 = new Panel2();
#if DEVEXPRESS
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
#endif
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            //
            // lbl_Content
            //
            this.lbl_Content.AutoEllipsis = true;
            this.lbl_Content.AutoSize = true;
            this.lbl_Content.Dock = System.Windows.Forms.DockStyle.Fill;

            this.lbl_Content.Font = content.Font == null
                ? new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Bold)
                : content.Font;

            this.lbl_Content.ForeColor = content.ForeColor.HasValue
                ? content.ForeColor.Value
                : (errorIcon ? Color.FromArgb(170, 57, 57) : Color.FromArgb(45, 68, 136));

            if (content.BackColor.HasValue)
            {
                this.lbl_Content.BackColor = content.BackColor.Value;
            }

            this.lbl_Content.Location = new System.Drawing.Point(58, 10);
            this.lbl_Content.Name = "lbl_Content";
            this.lbl_Content.Size = new System.Drawing.Size(57, 12);
            this.lbl_Content.TabIndex = 2;
            this.lbl_Content.Text = content.Text;
            if (parameters.ClickClosed)
                this.lbl_Content.Click += new System.EventHandler(this.Close_Click);
            this.lbl_Content.Padding = new Padding(0, 10, 0, 10);
            //
            // panelControl1
            //
            this.panelControl1.AutoSize = true;
            this.panelControl1.Controls.Add(this.lbl_Content);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(8);
            this.panelControl1.Size = new System.Drawing.Size(300, 300);
            this.panelControl1.TabIndex = 3;
#if DEVEXPRESS
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.panelControl1.Appearance.BorderColor = errorIcon ? Color.FromArgb(170, 57, 57) : Color.FromArgb(136, 176, 228);
#else
            this.panelControl1.BorderStyle = BorderStyle.FixedSingle;
#endif
            if (parameters.ClickClosed)
                this.panelControl1.Click += new System.EventHandler(this.Close_Click);
            if (icon.Image != null)
            {
                this.lbl_Content.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                this.lbl_ICON = new System.Windows.Forms.Label();
                //
                // lbl_ICON
                //
                this.lbl_ICON.Dock = System.Windows.Forms.DockStyle.Left;
                this.lbl_ICON.Location = new System.Drawing.Point(10, 10);
                this.lbl_ICON.Name = "lbl_ICON";
                this.lbl_ICON.Size = new System.Drawing.Size(48, 280);
                this.lbl_ICON.TabIndex = 3;
                this.lbl_ICON.Image = icon.Image;
                if (parameters.ClickClosed)
                    this.lbl_ICON.Click += new System.EventHandler(this.Close_Click);
                this.lbl_Content.Padding = new Padding(4, 0, 0, 0);

                this.panelControl1.Controls.Add(this.lbl_ICON);
            }
            else
            {
                this.lbl_Content.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            }
            //
            // MessageLabelForm
            //
            this.Text = form.Text;
            if (form.Font != null) this.Font = form.Font;
            if (form.ForeColor.HasValue) this.ForeColor = form.ForeColor.Value;
            if (form.BackColor.HasValue) this.BackColor = form.BackColor.Value;

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.panelControl1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Opacity = 0d;
            this.Name = "MessageLabelForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;

            this.TopMost = true;
            if (icon.Image != null) this.MinimumSize = new System.Drawing.Size(0, 56);

            this.MaximumSize = new Size(800, 600);
#if DEVEXPRESS
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
#endif
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.IsDisposed)
            {
                _timer.Stop();
                return;
            }
            if (this.Opacity < 1d)
            {
                this.Opacity += 0.25d;
            }
            if (this.Bounds.Contains(Cursor.Position))
            {
                _closeIndex = 0;
                this.Opacity = 1d;
                return;
            }
            else _closeIndex += 100;
            if (_closeEnd > 1999 && _closeEnd - _closeIndex < 1001)
            {
                this.Opacity = (_closeEnd - _closeIndex) / 1000d;
            }

            if (_closeIndex >= _closeEnd)
            {
                this.Close();
            }
        }
    }
}