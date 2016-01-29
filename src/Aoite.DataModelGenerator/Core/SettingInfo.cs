using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Aoite.DataModelGenerator
{
    public class SettingInfo
    {
        [Ignore]
        public DateTime LastTime { get; set; }
        public string ConnectionString { get; set; }
        public string Namespace { get; set; }
        public string OutputFolder { get; set; }
        public int EngineIndex { get; set; }
        public override string ToString()
        {
            return Shared.DecorateConnectionString(this.ConnectionString);
        }


        public SettingInfo()
        {
            this.Namespace = "Project.Models";
            this.OutputFolder = "D:\\models\\" + DateTime.Now.ToString("yyyyMM");
        }
        public string CreateOutputFolder()
        {
            int index = 1;
            while (true)
            {
                var folder = Path.Combine(this.OutputFolder, index++.ToString());
                if (Directory.Exists(folder)) continue;
                Directory.CreateDirectory(folder);
                return folder;
            }
        }

        public Result WriteCode(string folder,ObjectInfo info, string code)
        {
            try
            {

                File.WriteAllText(Path.Combine(folder, info.Name + ".cs"), code);
                return Result.Successfully;
            }
            catch (Exception ex)
            {
                return ex;
            }
            
        }
    }
}
