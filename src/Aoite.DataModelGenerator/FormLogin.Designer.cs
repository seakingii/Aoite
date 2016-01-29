namespace Aoite.DataModelGenerator
{
    partial class FormLogin
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
            this.components = new System.ComponentModel.Container();
            this.lbl_ConnectionString = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_Exit = new System.Windows.Forms.Button();
            this.lbl_DisplayHistory = new System.Windows.Forms.Label();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.tbox_OutputFolder = new System.Windows.Forms.TextBox();
            this.tbox_Namespace = new System.Windows.Forms.TextBox();
            this.lbl_OutputFolder = new System.Windows.Forms.Label();
            this.lbl_Namespace = new System.Windows.Forms.Label();
            this.lbl_Engines = new System.Windows.Forms.Label();
            this.cbox_Engines = new System.Windows.Forms.ComboBox();
            this.rtbox_ConnectionString = new System.Windows.Forms.RichTextBox();
            this.llbl_Template = new System.Windows.Forms.LinkLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_ConnectionString
            // 
            this.lbl_ConnectionString.AutoSize = true;
            this.lbl_ConnectionString.Location = new System.Drawing.Point(12, 214);
            this.lbl_ConnectionString.Name = "lbl_ConnectionString";
            this.lbl_ConnectionString.Size = new System.Drawing.Size(65, 12);
            this.lbl_ConnectionString.TabIndex = 7;
            this.lbl_ConnectionString.Text = "连接字符串";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(83, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(341, 78);
            this.pictureBox1.TabIndex = 33;
            this.pictureBox1.TabStop = false;
            // 
            // btn_Exit
            // 
            this.btn_Exit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.btn_Exit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Exit.ForeColor = System.Drawing.Color.White;
            this.btn_Exit.Location = new System.Drawing.Point(395, 340);
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Size = new System.Drawing.Size(95, 33);
            this.btn_Exit.TabIndex = 10;
            this.btn_Exit.Text = "取消(&E)";
            this.btn_Exit.UseVisualStyleBackColor = false;
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // lbl_DisplayHistory
            // 
            this.lbl_DisplayHistory.AutoSize = true;
            this.lbl_DisplayHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            this.lbl_DisplayHistory.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lbl_DisplayHistory.Location = new System.Drawing.Point(12, 350);
            this.lbl_DisplayHistory.Name = "lbl_DisplayHistory";
            this.lbl_DisplayHistory.Padding = new System.Windows.Forms.Padding(3);
            this.lbl_DisplayHistory.Size = new System.Drawing.Size(77, 18);
            this.lbl_DisplayHistory.TabIndex = 11;
            this.lbl_DisplayHistory.Text = "显示历史(0)";
            // 
            // btn_Connect
            // 
            this.btn_Connect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(81)))), ((int)(((byte)(34)))));
            this.btn_Connect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Connect.ForeColor = System.Drawing.Color.White;
            this.btn_Connect.Location = new System.Drawing.Point(283, 340);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(95, 33);
            this.btn_Connect.TabIndex = 9;
            this.btn_Connect.Text = "连接(&C)";
            this.btn_Connect.UseVisualStyleBackColor = false;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // tbox_OutputFolder
            // 
            this.tbox_OutputFolder.Location = new System.Drawing.Point(83, 141);
            this.tbox_OutputFolder.Name = "tbox_OutputFolder";
            this.tbox_OutputFolder.Size = new System.Drawing.Size(407, 21);
            this.tbox_OutputFolder.TabIndex = 3;
            // 
            // tbox_Namespace
            // 
            this.tbox_Namespace.Location = new System.Drawing.Point(83, 107);
            this.tbox_Namespace.Name = "tbox_Namespace";
            this.tbox_Namespace.Size = new System.Drawing.Size(407, 21);
            this.tbox_Namespace.TabIndex = 1;
            // 
            // lbl_OutputFolder
            // 
            this.lbl_OutputFolder.AutoSize = true;
            this.lbl_OutputFolder.Location = new System.Drawing.Point(24, 144);
            this.lbl_OutputFolder.Name = "lbl_OutputFolder";
            this.lbl_OutputFolder.Size = new System.Drawing.Size(53, 12);
            this.lbl_OutputFolder.TabIndex = 2;
            this.lbl_OutputFolder.Text = "输出目录";
            // 
            // lbl_Namespace
            // 
            this.lbl_Namespace.AutoSize = true;
            this.lbl_Namespace.Location = new System.Drawing.Point(24, 110);
            this.lbl_Namespace.Name = "lbl_Namespace";
            this.lbl_Namespace.Size = new System.Drawing.Size(53, 12);
            this.lbl_Namespace.TabIndex = 0;
            this.lbl_Namespace.Text = "命名空间";
            // 
            // lbl_Engines
            // 
            this.lbl_Engines.AutoSize = true;
            this.lbl_Engines.Location = new System.Drawing.Point(12, 178);
            this.lbl_Engines.Name = "lbl_Engines";
            this.lbl_Engines.Size = new System.Drawing.Size(65, 12);
            this.lbl_Engines.TabIndex = 4;
            this.lbl_Engines.Text = "数据库引擎";
            // 
            // cbox_Engines
            // 
            this.cbox_Engines.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_Engines.FormattingEnabled = true;
            this.cbox_Engines.Location = new System.Drawing.Point(83, 175);
            this.cbox_Engines.Name = "cbox_Engines";
            this.cbox_Engines.Size = new System.Drawing.Size(348, 20);
            this.cbox_Engines.TabIndex = 5;
            // 
            // rtbox_ConnectionString
            // 
            this.rtbox_ConnectionString.Location = new System.Drawing.Point(83, 211);
            this.rtbox_ConnectionString.Name = "rtbox_ConnectionString";
            this.rtbox_ConnectionString.Size = new System.Drawing.Size(407, 123);
            this.rtbox_ConnectionString.TabIndex = 8;
            this.rtbox_ConnectionString.Text = "";
            // 
            // llbl_Template
            // 
            this.llbl_Template.AutoSize = true;
            this.llbl_Template.Location = new System.Drawing.Point(437, 178);
            this.llbl_Template.Name = "llbl_Template";
            this.llbl_Template.Size = new System.Drawing.Size(53, 12);
            this.llbl_Template.TabIndex = 6;
            this.llbl_Template.TabStop = true;
            this.llbl_Template.Text = "连接模板";
            this.llbl_Template.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbl_Template_LinkClicked);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // FormLogin
            // 
            this.AcceptButton = this.btn_Connect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.CancelButton = this.btn_Exit;
            this.ClientSize = new System.Drawing.Size(502, 385);
            this.Controls.Add(this.llbl_Template);
            this.Controls.Add(this.rtbox_ConnectionString);
            this.Controls.Add(this.lbl_ConnectionString);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btn_Exit);
            this.Controls.Add(this.lbl_DisplayHistory);
            this.Controls.Add(this.btn_Connect);
            this.Controls.Add(this.tbox_OutputFolder);
            this.Controls.Add(this.tbox_Namespace);
            this.Controls.Add(this.lbl_OutputFolder);
            this.Controls.Add(this.lbl_Namespace);
            this.Controls.Add(this.lbl_Engines);
            this.Controls.Add(this.cbox_Engines);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(152)))), ((int)(((byte)(255)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "连接到服务器";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_ConnectionString;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btn_Exit;
        private System.Windows.Forms.Label lbl_DisplayHistory;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.TextBox tbox_OutputFolder;
        private System.Windows.Forms.TextBox tbox_Namespace;
        private System.Windows.Forms.Label lbl_OutputFolder;
        private System.Windows.Forms.Label lbl_Namespace;
        private System.Windows.Forms.Label lbl_Engines;
        private System.Windows.Forms.ComboBox cbox_Engines;
        private System.Windows.Forms.RichTextBox rtbox_ConnectionString;
        private System.Windows.Forms.LinkLabel llbl_Template;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;


    }
}