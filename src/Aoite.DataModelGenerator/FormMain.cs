using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aoite.DataModelGenerator
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            this.Icon = Program.AppIcon;
        }

        private void RefreshDB()
        {
            treeView1.Nodes.Clear();
            treeView1.BeginUpdate();
            var dbNode = treeView1.Nodes.Add("数据库");
            try
            {
                using(var fw = this.BeginRun("正在获取表内容..."))
                {
                    var tableNode = dbNode.Nodes.Add("表");
                    tableNode.Tag = ObjectType.Table;
                    Db.Engine.Execute("SELECT a.Name,b.Value FROM sysobjects a LEFT JOIN  sys.extended_properties b ON a.ID=b.Major_ID AND b.Minor_ID=0 WHERE a.XType='U' AND a.Name <>'sysdiagrams' ORDER BY a.Name").ToReader(reader =>
                    {
                        while(reader.Read())
                        {
                            ObjectInfo info = new ObjectInfo()
                            {
                                Type = ObjectType.Table,
                                Name = reader.GetString(0),
                                Comments = Convert.ToString(reader.GetValue(1))
                            };
                            tableNode.Nodes.Add(info.ToString()).Tag = info;
                        }
                    });

                    fw.Text = "正在获取视图内容...";
                    var viewNode = dbNode.Nodes.Add("视图");
                    viewNode.Tag = ObjectType.View;
                    Db.Engine.Execute("SELECT a.Name FROM sysobjects a WHERE a.XType='V' ORDER BY a.Name").ToReader(reader =>
                    {
                        while(reader.Read())
                        {
                            ObjectInfo info = new ObjectInfo()
                            {
                                Type = ObjectType.View,
                                Name = reader.GetString(0),
                            };
                            info.ClassName = info.Name;
                            if(info.ClassName.StartsWith("v_", StringComparison.CurrentCultureIgnoreCase))
                            {
                                info.ClassName = info.ClassName.Remove(0, 2);
                            }
                            viewNode.Nodes.Add(info.ToString()).Tag = info;
                        }
                    }); ;

                }
            }
            finally
            {
                treeView1.EndUpdate();
                dbNode.Expand();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.RefreshDB();
            treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
        }

        private void DisplayObjectInfo(ObjectInfo info)
        {
            var code = info.GenerateCode(Db.Engine, Shared.Setting);
            new FormEditor(info.ToString(), code).ShowDialog();
        }

        #region Button Events

        void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(treeView1.SelectedNode != e.Node || e.Node.Tag == null || e.Node.Tag is ObjectType) return;
            this.DisplayObjectInfo(e.Node.Tag as ObjectInfo);
        }

        private void lbl_Connect_Click(object sender, EventArgs e)
        {
            FormLogin login = new FormLogin();
            login.Tag = true;
            if(login.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.RefreshDB();
        }

        private bool CheckResult(Form fw, Result result)
        {
            return result.IsSucceed
                || fw.ShowYesNo("当前生成出现了错误，是否继续生成文件？\r\n" + result.Exception);

        }

        private bool WriteCodeFile(string folder, TreeNode node, Form fw)
        {
            var info = node.Tag as ObjectInfo;
            if(info != null)
            {
                fw.Text = "正在生成“" + info.Name + "”...";
                var code = info.GenerateCode(Db.Engine, Shared.Setting);
                if(!CheckResult(fw, Shared.Setting.WriteCode(folder, info, code)))
                    return false;
            }
            else if(!WriteCodeFiles(folder, node, fw)) return false;
            return true;
        }

        private bool WriteCodeFiles(string folder, TreeNode node, Form fw)
        {
            foreach(TreeNode child in node.Nodes)
            {
                if(!this.WriteCodeFile(folder, child, fw)) return false;
            }
            return true;
        }

        private void lbl_Generate_Click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            if(node == null) return;
            Func<string, TreeNode, Form, bool> func = this.WriteCodeFiles;
            if(node.Tag == null)
            {
                if(!this.ShowYesNo("是否生成所有对象的实体代码文件？")) return;
            }
            else if(node.Tag is ObjectType)
            {
                if(!this.ShowYesNo("是否生成所有"
                    + (((ObjectType)node.Tag) == ObjectType.Table ? "数据库表" : "视图")
                    + "的代码文件？")) return;
            }
            else func = this.WriteCodeFile;

            using(var fw = this.BeginRun())
            {
                var folder = Shared.Setting.CreateOutputFolder();
                if(func(folder, node, fw))
                {
                    System.Diagnostics.Process.Start("explorer.exe", folder);
                }
            }
        }

        private void lbl_Run_Click(object sender, EventArgs e)
        {
            var sql = textEditorControl1.SelectedText;
            if(string.IsNullOrEmpty(sql)) sql = textEditorControl1.Text;

            if(string.IsNullOrEmpty(sql))
            {
                this.ShowLabel("空的查询语句。");
                return;
            }
            var r = this.InputBox("请输入实体的名称：", "类名");
            string className = null;
            if(!r.ShowDialog()
                || string.IsNullOrEmpty(className = r.GetText(0))) return;

            ObjectInfo info = new ObjectInfo() { Name = className };
            var code = info.GenerateCode(Db.Engine, Shared.Setting, sql);
            new FormEditor("自定义 SQL 语句", code).ShowDialog();
        }

        private void lbl_Refresh_Click(object sender, EventArgs e)
        {
            this.RefreshDB();
        }

        #endregion
    }
}
