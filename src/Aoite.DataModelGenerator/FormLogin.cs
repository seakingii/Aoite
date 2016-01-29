using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aoite.Data;
using System.Text.RegularExpressions;
using System.IO;

namespace Aoite.DataModelGenerator
{
    public partial class FormLogin : Form
    {

        public FormLogin()
        {
            InitializeComponent();
            cbox_Engines.SelectedIndexChanged += cbox_Engines_SelectedIndexChanged;
            this.InitialData();
            this.Icon = Program.AppIcon;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SettingInfo firstSetting = null;
            foreach (var setting in Shared.LoadSettings())
            {
                if (firstSetting == null) firstSetting = setting;
                contextMenuStrip1.Items.Add(new ToolStripButton(setting.ToString()) { Tag = setting });
                contextMenuStrip1.Items.Add(new ToolStripSeparator());
            }
            if (this.Tag != null) this.BindingSetting(Shared.Setting);
            else this.BindingSetting(firstSetting ?? Shared.Setting);

            lbl_DisplayHistory.Text = "显示历史(" + contextMenuStrip1.Items.Count / 2 + ")";
            lbl_DisplayHistory.MouseClick += lbl_DisplayHistory_MouseClick;
            contextMenuStrip1.ItemClicked += contextMenuStrip1_ItemClicked;
        }

        private void BindingSetting(SettingInfo setting)
        {
            tbox_Namespace.Text = setting.Namespace;
            tbox_OutputFolder.Text = setting.OutputFolder;
            rtbox_ConnectionString.Text = setting.ConnectionString;
            cbox_Engines.SelectedIndex = setting.EngineIndex;
        }
        private void InitialData()
        {
            this.AddEngineItem("Microsoft SQL Server"
                , "sql"
                ,
@"Data Source       = 
;Initial Catalog  = 
;User ID          = 
;Password         = ");
            //            this.AddEngineItem("Oracle", QueryEngineProvider.Oracle,
            //@"HOST       = 
            //;PORT      = 
            //;SID       = 
            //;USER ID   = 
            //;PASSWORD  = ");
            rtbox_ConnectionString.AcceptsTab = true;
            rtbox_ConnectionString.KeyDown += rtbox_ConnectionString_KeyDown;
        }

        public void AddEngineItem(string name, string provider, string connectionStringTemplate)
        {
            var item = new EngineDisplayItem()
            {
                Name = name,
                Provider = provider,
                ConnectionStringTemplate = connectionStringTemplate
            };
            cbox_Engines.Items.Add(item);
        }

        private bool ValidateData()
        {
            return tbox_Namespace.DoCheck("命名空间不能为空！")
                && tbox_OutputFolder.DoCheck("输出目录不能为空！")
                && rtbox_ConnectionString.DoCheck("连接字符串不能为空！");
            //    if(string.IsNullOrEmpty(""))
        }

        private void rtbox_ConnectionString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                var line = rtbox_ConnectionString.GetLineFromCharIndex(rtbox_ConnectionString.SelectionStart) + 1;
                if (line < rtbox_ConnectionString.Lines.Length)
                {
                    rtbox_ConnectionString.SelectionStart = rtbox_ConnectionString.SelectionStart + rtbox_ConnectionString.Lines[line].Length + 1;
                }
                else
                {
                    this.GetNextControl(rtbox_ConnectionString, true).Focus();
                }
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var setting = e.ClickedItem.Tag as SettingInfo;
            if (setting != null)
                this.BindingSetting(setting);
        }

        private void lbl_DisplayHistory_MouseClick(object sender, MouseEventArgs e)
        {
            if (contextMenuStrip1.Items.Count == 0)
            {
                this.ShowLabel("您没有保存任何历史配置...");
                return;
            }
            contextMenuStrip1.Show(MousePosition);
        }

        private void cbox_Engines_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbox_Engines.SelectedIndex < 0
                || rtbox_ConnectionString.TextLength > 0) return;

            llbl_Template_LinkClicked(null, null);
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            if (this.ValidateData())
            {
                var item = cbox_Engines.SelectedItem as EngineDisplayItem;
                var connString = Shared.DecorateConnectionString(rtbox_ConnectionString.Text);
                var engine = DbEngine.Create(item.Provider, connString);
                if (!this.IsSuccess(engine.TestConnection(), "连接到服务器失败！"))
                    return;
                Shared.Setting.Namespace = tbox_Namespace.Text;
                Shared.Setting.OutputFolder = tbox_OutputFolder.Text;
                Shared.Setting.ConnectionString = rtbox_ConnectionString.Text;
                Shared.Setting.EngineIndex = cbox_Engines.SelectedIndex;
                Shared.Setting.Save();
                Db.SetEngine(engine);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }

        }

        private void llbl_Template_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (cbox_Engines.SelectedIndex < 0) return;
            rtbox_ConnectionString.Text = (cbox_Engines.SelectedItem as EngineDisplayItem).ConnectionStringTemplate;
            //rtbox_ConnectionString.DataBindings["Text"].WriteValue();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (this.DialogResult == System.Windows.Forms.DialogResult.OK) return;
            e.Cancel = this.Tag == null && !this.ShowYesNo("是否退出？");
        }
    }
}
