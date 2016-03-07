namespace Aoite.DataModelGenerator
{
    partial class FormMain
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.textEditorControl1 = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbl_Refresh = new Aoite.DataModelGenerator.HoverLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_Run = new Aoite.DataModelGenerator.HoverLabel();
            this.lbl_Connect = new Aoite.DataModelGenerator.HoverLabel();
            this.lbl_Generate = new Aoite.DataModelGenerator.HoverLabel();
            this.lbl_UnitTestScripts = new Aoite.DataModelGenerator.HoverLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(213)))), ((int)(((byte)(255)))));
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 43);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.textEditorControl1);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.splitContainer1.Size = new System.Drawing.Size(683, 390);
            this.splitContainer1.SplitterDistance = 313;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(313, 387);
            this.treeView1.TabIndex = 0;
            // 
            // textEditorControl1
            // 
            this.textEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditorControl1.Location = new System.Drawing.Point(0, 3);
            this.textEditorControl1.Name = "textEditorControl1";
            this.textEditorControl1.Size = new System.Drawing.Size(366, 387);
            this.textEditorControl1.TabIndex = 0;
            this.textEditorControl1.Text = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbl_Refresh);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lbl_UnitTestScripts);
            this.panel1.Controls.Add(this.lbl_Run);
            this.panel1.Controls.Add(this.lbl_Connect);
            this.panel1.Controls.Add(this.lbl_Generate);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(683, 43);
            this.panel1.TabIndex = 0;
            // 
            // lbl_Refresh
            // 
            this.lbl_Refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_Refresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lbl_Refresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_Refresh.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_Refresh.ForeColor = System.Drawing.Color.White;
            this.lbl_Refresh.HoverColor = System.Drawing.Color.Purple;
            this.lbl_Refresh.Location = new System.Drawing.Point(578, 5);
            this.lbl_Refresh.Name = "lbl_Refresh";
            this.lbl_Refresh.Size = new System.Drawing.Size(100, 31);
            this.lbl_Refresh.TabIndex = 3;
            this.lbl_Refresh.Text = "刷新";
            this.lbl_Refresh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Refresh.Click += new System.EventHandler(this.lbl_Refresh_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(153)))), ((int)(((byte)(0)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(683, 3);
            this.label1.TabIndex = 4;
            // 
            // lbl_Run
            // 
            this.lbl_Run.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(81)))), ((int)(((byte)(34)))));
            this.lbl_Run.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_Run.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_Run.ForeColor = System.Drawing.Color.White;
            this.lbl_Run.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lbl_Run.Location = new System.Drawing.Point(217, 5);
            this.lbl_Run.Name = "lbl_Run";
            this.lbl_Run.Size = new System.Drawing.Size(100, 31);
            this.lbl_Run.TabIndex = 2;
            this.lbl_Run.Text = "运行";
            this.lbl_Run.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Run.Click += new System.EventHandler(this.lbl_Run_Click);
            // 
            // lbl_Connect
            // 
            this.lbl_Connect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(163)))), ((int)(((byte)(0)))));
            this.lbl_Connect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_Connect.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_Connect.ForeColor = System.Drawing.Color.White;
            this.lbl_Connect.HoverColor = System.Drawing.Color.Green;
            this.lbl_Connect.Location = new System.Drawing.Point(5, 5);
            this.lbl_Connect.Name = "lbl_Connect";
            this.lbl_Connect.Size = new System.Drawing.Size(100, 31);
            this.lbl_Connect.TabIndex = 0;
            this.lbl_Connect.Text = "连接";
            this.lbl_Connect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Connect.Click += new System.EventHandler(this.lbl_Connect_Click);
            // 
            // lbl_Generate
            // 
            this.lbl_Generate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lbl_Generate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_Generate.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_Generate.ForeColor = System.Drawing.Color.White;
            this.lbl_Generate.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lbl_Generate.Location = new System.Drawing.Point(111, 5);
            this.lbl_Generate.Name = "lbl_Generate";
            this.lbl_Generate.Size = new System.Drawing.Size(100, 31);
            this.lbl_Generate.TabIndex = 1;
            this.lbl_Generate.Text = "生成";
            this.lbl_Generate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Generate.Click += new System.EventHandler(this.lbl_Generate_Click);
            // 
            // lbl_UnitTestScripts
            // 
            this.lbl_UnitTestScripts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_UnitTestScripts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(81)))), ((int)(((byte)(34)))));
            this.lbl_UnitTestScripts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_UnitTestScripts.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_UnitTestScripts.ForeColor = System.Drawing.Color.White;
            this.lbl_UnitTestScripts.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lbl_UnitTestScripts.Location = new System.Drawing.Point(472, 5);
            this.lbl_UnitTestScripts.Name = "lbl_UnitTestScripts";
            this.lbl_UnitTestScripts.Size = new System.Drawing.Size(100, 31);
            this.lbl_UnitTestScripts.TabIndex = 2;
            this.lbl_UnitTestScripts.Text = "生成单元测试";
            this.lbl_UnitTestScripts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_UnitTestScripts.Click += new System.EventHandler(this.lbl_UnitTestScripts_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(683, 433);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "FormMain";
            this.Text = "数据库实体生成器";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.RichTextBox textEditorControl1;
        private System.Windows.Forms.Panel panel1;
        private HoverLabel lbl_Generate;
        private HoverLabel lbl_Connect;
        private HoverLabel lbl_Run;
        private HoverLabel lbl_Refresh;
        private System.Windows.Forms.Label label1;
        private HoverLabel lbl_UnitTestScripts;
    }
}