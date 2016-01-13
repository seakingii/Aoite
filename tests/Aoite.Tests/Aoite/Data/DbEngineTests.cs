using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.Data
{
    public class DbEngineTests
    {
        [Fact]
        public void Create_Provider_Test()
        {
            // "sql","mssql"
            var engine = DbEngine.Create("sql", "1");
            Assert.IsAssignableFrom<SqlEngineProvider>(engine.Provider);
            Assert.Equal("1", engine.Provider.ConnectionString);
            engine = DbEngine.Create("mssql", "1");
            Assert.IsAssignableFrom<SqlEngineProvider>(engine.Provider);

            // "ce", "sqlce", "mssqlce"
            engine = DbEngine.Create("ce", "1");
            Assert.IsAssignableFrom<SqlCeEngineProvider>(engine.Provider);
            Assert.Equal("1", engine.Provider.ConnectionString);
            engine = DbEngine.Create("sqlce", "1");
            Assert.IsAssignableFrom<SqlCeEngineProvider>(engine.Provider);
            engine = DbEngine.Create("mssqlce", "1");
            Assert.IsAssignableFrom<SqlCeEngineProvider>(engine.Provider);
        }
    }
}
