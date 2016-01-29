using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Aoite.DataModelGenerator
{
    public static class Shared
    {
        public static SettingInfo Setting = new SettingInfo();

        static readonly string Folder = GA.FullPath("Settings");

        public static IEnumerable<SettingInfo> LoadSettings()
        {
            var files = GA.IO.CreateDirectory(Folder).GetFiles("*.setting");
            var settings = new List<SettingInfo>(files.Length);
            foreach (var file in files)
            {
                var content = File.ReadAllText(file.FullName);
                var info = Serializer.Json.FastRead<SettingInfo>(content);
                info.LastTime = file.LastWriteTime;
                settings.Add(info);
            }
            return settings.OrderByDescending(info => info.LastTime);
        }

        public static void Save(this SettingInfo setting)
        {
            File.WriteAllText(Path.Combine(Folder, setting.ConnectionString.GetHashCode().ToString() + ".setting"), Serializer.Json.FastWrite(setting));
        }

        public static string DecorateConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return string.Empty;

            StringBuilder builder = new StringBuilder();
            using (StringReader reader = new StringReader(connectionString))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    builder.Append(Regex.Replace(line, " {2,}", " "));

                }
                return builder.ToString();
            }
        }
    }
}
