using System;
using System.Collections.Generic;
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
    /// <summary> 
    /// 文本编辑框的窗体。
    /// </summary>
    public partial class TextEditorForm
    {
        /// <summary> 
        /// 设置或获取文本编辑框的说明。
        /// </summary>
        public string Description
        {
            get
            {
                return lbl_Tip.Text;
            }
            set
            {
                lbl_Tip.Text = value;
            }
        }

        /// <summary> 
        /// 初始化 <see cref="System.Windows.Forms.TextEditorForm"/> 的新实例。
        /// </summary>
        protected TextEditorForm() : this(null, true) { }

        /// <summary>
        /// 指定默认值，初始化 <see cref="System.Windows.Forms.TextEditorForm"/> 的新实例。
        /// </summary>
        /// <param name="value">编辑框的默认值。</param>
        /// <param name="showBars">指示是否显示滚动条。</param>
        public TextEditorForm(string value, bool showBars = true)
        {
            InitializeComponent(value, showBars);
        }

        /// <summary>
        /// 将当前文本框设置为只读模式。
        /// </summary>
        public void ReadOnlyContent()
        {
#if DEVEXPRESS
            edit_Text.Properties.ReadOnly = true;
#else
            edit_Text.ReadOnly = true;
#endif
            edit_Text.BackColor = System.Drawing.Color.White;
        }

        /// <summary> 
        /// 获取当前文本框的内容。
        /// </summary>
        public virtual string Content
        {
            get { return this.edit_Text.Text; }
        }

        /// <summary>
        /// 设置或获取提示标签的文本。
        /// </summary>
        public virtual string LabelText
        {
            get
            {
                return this.lbl_Tip.Text;
            }
            set
            {
                this.lbl_Tip.Text = value;
            }
        }
        /// <summary>
        /// 表示在提交时发生的方法。
        /// </summary>
        /// <returns>如果返回 true 则表示允许提交，否则拒绝提交。</returns>
        protected virtual bool OnAccepting()
        {
            return true;
        }

        private void btn_Accept_Click(object sender, EventArgs e)
        {
            if (this.OnAccepting())
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public partial class TextEditorForm :
#if DEVEXPRESS
 XtraForm
#else
 Form
#endif
    {

        private System.ComponentModel.IContainer components = null;

        private Rich2 edit_Text;
        private Button2 btn_Accept;
        private Button2 btn_Cancel;
        /// <summary>
        /// 一个标签。
        /// </summary>
        protected Label2 lbl_Tip;
        /// <summary> 
        /// 释放时发生。
        /// </summary>
        /// <param name="disposing">指示是否释放组件。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent(string value, bool showBars)
        {
            this.lbl_Tip = new Label2();
            this.edit_Text = new Rich2();
            this.btn_Accept = new Button2();
            this.btn_Cancel = new Button2();
            System.Windows.Forms.Panel editPanel = new System.Windows.Forms.Panel();
#if DEVEXPRESS
            ((System.ComponentModel.ISupportInitialize)(this.edit_Text.Properties)).BeginInit();
#endif
            this.SuspendLayout();
            // 
            // lbl_Tip
            // 
            this.lbl_Tip.Location = new System.Drawing.Point(26, 13);
            this.lbl_Tip.Name = "lbl_Tip";
            this.lbl_Tip.Size = new System.Drawing.Size(108, 14);
            this.lbl_Tip.TabIndex = 0;
            this.lbl_Tip.Text = "请输入内容字符串：";
            // 
            // edit_Text
            // 
            this.edit_Text.Name = "edit_Text";
            this.edit_Text.TabIndex = 2;
            this.edit_Text.Font = new System.Drawing.Font("新宋体", 10);
            this.edit_Text.Dock = System.Windows.Forms.DockStyle.Fill;
#if DEVEXPRESS
            this.edit_Text.EditValue = value;
            this.edit_Text.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            if (showBars) this.edit_Text.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
#else
            this.edit_Text.Text = value;
            this.edit_Text.BorderStyle = BorderStyle.None;
            if(showBars) this.edit_Text.ScrollBars = RichTextBoxScrollBars.Both;
#endif
            //
            // editPanel
            //
            editPanel.Location = new System.Drawing.Point(22, 33);
            editPanel.Name = "editPanel";
            editPanel.Size = new System.Drawing.Size(520, 280);
            editPanel.TabIndex = 1;
            editPanel.BackColor = System.Drawing.Color.White;
            editPanel.Padding = new System.Windows.Forms.Padding(10);
            editPanel.Controls.Add(this.edit_Text);
            editPanel.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
            // 
            // btn_Accept
            // 
            this.btn_Accept.Location = new System.Drawing.Point(351, 319);
            this.btn_Accept.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_Accept.Name = "btn_Accept";
            this.btn_Accept.Size = new System.Drawing.Size(84, 27);
            this.btn_Accept.TabIndex = 3;
            this.btn_Accept.Text = "确定";
            this.btn_Accept.Click += new System.EventHandler(this.btn_Accept_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_Cancel.Location = new System.Drawing.Point(458, 319);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(84, 27);
            this.btn_Cancel.TabIndex = 4;
            this.btn_Cancel.Text = "取消";
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // TextEditorForm
            // 
            this.AcceptButton = this.btn_Accept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(564, 359);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Accept);
            this.Controls.Add(editPanel);
            this.Controls.Add(this.lbl_Tip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "TextEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "文本编辑器";
#if DEVEXPRESS
            ((System.ComponentModel.ISupportInitialize)(this.edit_Text.Properties)).EndInit();
#endif
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}