using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Aoite.DataModelGenerator
{
    public static class SmoHelper
    {
        static string[] IgnoreLines = new[] { "SET ANSI_NULLS ON", "SET QUOTED_IDENTIFIER ON" };
        static string[] EmptyLines = new[] {
            "[dbo].",
            " CLUSTERED",
            "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]",
            " ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]",
            " ASC"
        };
        public static string CreateScripts()
        {
            var sqlConn = Db.Engine.Provider.CreateConnection() as SqlConnection;
            var conn = new ServerConnection(sqlConn);
            var server = new Server(conn);
            var database = server.Databases[sqlConn.Database];

            var sb = new StringBuilder();
            var options = new ScriptingOptions();
            options.AnsiPadding = false;
            options.DriPrimaryKey = true;
            options.NoCollation = true;

            sb.AppendLine("/****** Script Date: " + DateTime.Now.ToString() + " ******/").AppendLine();

            foreach(Table table in database.Tables)
            {
                var sc = table.Script(options);
                sb.AppendLine("/*[" + table.Name + "]*/");
                foreach(var s in sc)
                {
                    var line = s;
                    if(IgnoreLines.Contains(line)) continue;
                    foreach(var el in EmptyLines)
                    {
                        line = line.Replace(el, string.Empty);
                    }
                    sb.AppendLine(line);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
