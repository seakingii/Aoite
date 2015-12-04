using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Aoite.Dbx
{
    public class SqlEngineProvider : IEngineProvider
    {
        public DbProviderFactory DbFactory { get { return SqlClientFactory.Instance; } }

        public string Name { get { return "sql"; } }

        public string EscapeName(string name, NamePoint point)
        {
            switch(point)
            {
                case NamePoint.Field:
                case NamePoint.Table:
                    return string.Concat("[", name, "]");
                case NamePoint.CommandParameter:
                case NamePoint.DeclareParameter:
                    return string.Concat("@", name);
            }
            return name;
        }
    }
}
