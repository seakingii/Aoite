using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
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
#else
    using Label2 = Label;
    using Button2 = Button;
    using Panel2 = Panel;
    using Text2 = TextBox;
#endif

    /// <summary>
    /// 一个输入框的窗体。
    /// </summary>
    public partial class InputBoxForm
    {
        private Label2 lbl_Info;
        private TableLayoutPanel tableLayoutPanel1;
        private InputBoxParameters _paramters;

        /// <summary>
        /// 初始化 <see cref="InputBoxForm"/> 的新实例。
        /// </summary>
        /// <param name="owner">附属控件。</param>
        /// <param name="paramters">输入框参数。</param>
        public InputBoxForm(Control owner, InputBoxParameters paramters)
        {
            this._paramters = paramters;
            paramters._InputBoxForm = this;
            this.InitializeComponent(owner, paramters);
        }

        /// <summary>
        /// 提交表单。
        /// </summary>
        public void Submit()
        {
            CancelEventArgs ea = new CancelEventArgs();
            this._paramters.OnSubmitting(ea);
            if (ea.Cancel) return;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._paramters.OnSubmit();
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Submit_Click(object sender, EventArgs e)
        {
            this.Submit();
        }

        /// <summary>
        /// 引发 <see cref="System.Windows.Forms.Form.Closing"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="System.ComponentModel.CancelEventArgs"/>。</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            CancelEventArgs ea = new CancelEventArgs();
            this._paramters.OnCanceling(ea);
            if (ea.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                base.OnClosing(e);
                this._paramters.OnCancel();
            }
        }
    }

    public partial class InputBoxForm :
#if DEVEXPRESS
 XtraForm
#else
 Form
#endif
    {

        private void InitializeComponent(Control owner, InputBoxParameters parameters)
        {
                var noBorder = 
#if DEVEXPRESS
 DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
#else
 BorderStyle.None
#endif
;
            this.lbl_Info = new Label2();
            var panelControl1 = new Panel2();
            var panelControl2 = new Panel2();
            var btn_Submit = new Button2();
            var btn_Cancel = new Button2();

            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            panelControl1.SuspendLayout();
            panelControl2.SuspendLayout();
            this.SuspendLayout();

            //
            // lbl_Info
            //
            this.lbl_Info.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_Info.Location = new System.Drawing.Point(15, 15);
            this.lbl_Info.Name = "lbl_Info";
            this.lbl_Info.Size = new System.Drawing.Size(48, 15);
            this.lbl_Info.TabIndex = 0;
            this.lbl_Info.Text = string.IsNullOrEmpty(parameters.DisplayInfo) ? "请输入..." : parameters.DisplayInfo;
            //
            // tableLayoutPanel1
            //
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 520F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 30);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";

            if (parameters.Editors.Count == 0)
                parameters.Editors.Add( new Text2());

            var editors = parameters.Editors;

            this.tableLayoutPanel1.RowCount = editors.Count;

            Control editor;
            for (int i = 0; i < editors.Count; i++)
            {
                editor = editors[i];
                float height = editor.Height;
                Panel panel = new Panel();
                panel.Dock = DockStyle.Fill;
                editor.Dock = DockStyle.Fill;

                panel.Controls.Add(editor);
                if (editor.Tag != null)
                {
                    panel.Controls.Add(new Label2()
                    {
                        Text = editor.Tag.ToString(),
                        Dock = DockStyle.Left,
                        Padding = new Padding(3, 3, 10, 0)
                    });
                }
                panel.TabIndex = i;
#if DEVEXPRESS
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, height < 30 ? 30F : height));
#else
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, height < 35 ? 35F : height));
#endif
                tableLayoutPanel1.Controls.Add(panel);
            }

            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tableLayoutPanel1.TabIndex = 3;
            //
            // panelControl1
            //
            panelControl1.AutoSize = true;
            panelControl1.BorderStyle = noBorder;
            panelControl1.Controls.Add(panelControl2);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl1.Location = new System.Drawing.Point(15, 327);
            panelControl1.Name = "panelControl1";
            panelControl1.Padding = new System.Windows.Forms.Padding(5, 0, 5, 5);
            panelControl1.Size = new System.Drawing.Size(520, 48);
            panelControl1.TabIndex = 4;
            //
            // panelControl2
            //
            panelControl2.BorderStyle = noBorder;
            panelControl2.Controls.Add(btn_Submit);
            panelControl2.Controls.Add(btn_Cancel);
            panelControl2.Location = new System.Drawing.Point(5, 5);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(510, 35);
            panelControl2.TabIndex = 1;
            //
            // btn_Submit
            //
            btn_Submit.Location = new System.Drawing.Point(351, 1);
            btn_Submit.Name = "btn_Submit";
            btn_Submit.Size = new System.Drawing.Size(75, 32);
            btn_Submit.TabIndex = int.MaxValue - 1;
            btn_Submit.Text = "确定";
            btn_Submit.Click += new System.EventHandler(btn_Submit_Click);
            //
            // btn_Cancel
            //
            btn_Cancel.Location = new System.Drawing.Point(432, 1);
            btn_Cancel.Name = "btn_Cancel";
            btn_Cancel.Size = new System.Drawing.Size(75, 32);
            btn_Cancel.TabIndex = int.MaxValue;
            btn_Cancel.Text = "取消";
            btn_Cancel.Click += new System.EventHandler(btn_Cancel_Click);
            //
            // InputBoxForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.StartPosition = owner == null ? FormStartPosition.CenterScreen : FormStartPosition.CenterParent;
            this.ClientSize = new System.Drawing.Size(550, 380);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(panelControl1);
            this.Controls.Add(this.lbl_Info);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputBoxForm";
            this.Padding = new System.Windows.Forms.Padding(15, 15, 15, 5);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.AcceptButton = btn_Submit;
            this.CancelButton = btn_Cancel;
            this.Text = "请根据提示输入...";
            this.tableLayoutPanel1.ResumeLayout(false);
            panelControl1.ResumeLayout(false);
            panelControl2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

    }
}