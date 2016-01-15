using System.Data.Common;
using System.Data.SqlServerCe;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个基于 Microsoft SQL Server Compact 数据源查询与交互引擎的提供程序。
    /// </summary>
    [DbProviders("sqlce", "ce", "mssqlce")]
    public class SqlCeEngineProvider : SqlEngineProvider
    {
        /// <summary>
        /// 获取用于创建提供程序对数据源类的实现的实例。
        /// </summary>
        public override DbProviderFactory DbFactory { get { return SqlCeProviderFactory.Instance; } }

        /// <summary>
        /// 获取用生成查询命令的实现实例。
        /// </summary>
        public override ISqlFactory SqlFactory { get { return Factories.SqlCeFactory.Instance; } }

        /// <summary>
        /// 获取一个值，表示当前数据提供程序的名称。
        /// </summary>
        public override string Name { get { return "sqlce"; } }

        /// <summary>
        /// 指定数据库的连接字符串，初始化 <see cref="SqlCeEngineProvider"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">数据源的连接字符串。</param>
        public SqlCeEngineProvider(string connectionString) : base(connectionString) { }

        /// <summary>
        /// 提供数据源和密码，初始化一个 <see cref="SqlCeEngineProvider"/> 类的新实例。
        /// </summary>
        /// <param name="datasource">SQL Server Compact 数据源的文件路径和名称。</param>
        /// <param name="password">数据源密码，最多包含 40 个字符。</param>
        public SqlCeEngineProvider(string datasource, string password) : this(string.Format("Persist Security Info=False;Data Source='{0}';password='{1}'", datasource, password)) { }

        /// <summary>
        /// 创建新数据源。
        /// </summary>
        public void CreateDatabase()
        {
            using(SqlCeEngine engine = new SqlCeEngine(this.ConnectionString))
            {
                engine.CreateDatabase();
            }
        }
    }
}
