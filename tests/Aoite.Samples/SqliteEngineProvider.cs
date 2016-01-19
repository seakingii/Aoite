using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Common;
using System.IO;

namespace Aoite.Data
{
    [DbProviders("sqlite")]
    public class SQLiteEngineProvider : SqlEngineProvider
    {
        public override DbProviderFactory DbFactory { get { return SQLiteFactory.Instance; } }
        public override ISqlFactory SqlFactory { get { return SQLiteSqlFactory.Instance; } }
        public override string Name { get { return "sqlite"; } }

        public SQLiteEngineProvider(string connectionString) : base(connectionString) { }

        public SQLiteEngineProvider(string fileName, string password)
            : this("Data Source=" + fileName + ";Version=3;" + (password == null ? string.Empty : "Password" + password))
        { }
    }


    public class SQLiteSqlFactory : Factories.SqlFactory
    {
        public readonly static new SQLiteSqlFactory Instance = new SQLiteSqlFactory();

        protected SQLiteSqlFactory() { }

        public override void PageProcessCommand(int pageNumber, int pageSize, DbCommand command)
        {
            var limit = (pageNumber - 1) * pageSize;
            command.CommandText += "LIMIT " + limit + "," + pageSize;
        }

        public override ExecuteCommand CreateLastIdentityCommand(TypeMapper mapper, ICommandTunnel tunnel = null)
        {
            if(tunnel == null) tunnel = Empty;
            return tunnel.GetCommand(mapper, new ExecuteCommand("SELECT last_insert_rowid()"));
        }
    }
}
