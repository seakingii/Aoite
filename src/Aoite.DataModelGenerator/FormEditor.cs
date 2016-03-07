using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Aoite.DataModelGenerator
{
    public partial class FormEditor : Form
    {
        public FormEditor()
        {
            InitializeComponent();
            this.Icon = Program.AppIcon;
        }
        public FormEditor(string title, string code)
            : this()
        {
            this.Text = title + " - 查看代码";
            textEditorControl1.Text = code;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Show();
        }
    }
}
