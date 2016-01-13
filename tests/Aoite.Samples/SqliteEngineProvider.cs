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
    public class SqliteEngineProvider : SqlEngineProvider
    {
        public override DbProviderFactory DbFactory { get { return SQLiteFactory.Instance; } }

        public SqliteEngineProvider(string connectionString) : base(connectionString) { }

        public SqliteEngineProvider(string fileName, string password)
            : this("Data Source=" + fileName + ";Version=3;" + (password == null ? string.Empty : "Password" + password))
        { }
        public override void PageProcessCommand(int pageNumber, int pageSize, DbCommand command)
        {
            var limit = (pageNumber - 1) * pageSize;
            command.CommandText += "LIMIT " + limit + "," + pageSize;
        }

        public override ExecuteCommand CreateLastIdentityCommand(TypeMapper mapper)
        {
            return new ExecuteCommand("SELECT last_insert_rowid()");
        }
    }
}
