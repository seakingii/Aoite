using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Aoite.DataModelGenerator
{
    static class Program
    {
        /// <summary>
        /// 获取应用程序的图标。
        /// </summary>
        public static readonly Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (new FormLogin().ShowDialog() == DialogResult.OK)
                Application.Run(new FormMain());
        }
    }
}
