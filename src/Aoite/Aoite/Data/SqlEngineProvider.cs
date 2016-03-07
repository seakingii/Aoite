using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个基于 Microsoft SQL Server 数据源查询与交互引擎的提供程序。
    /// </summary>
    [DbProviders("sql", "mssql")]
    public class SqlEngineProvider : DbEngineProviderBase
    {
        const string IntegratedSecurityFormat = "Data Source={0};Initial Catalog={1};Integrated Security=True;Connect Timeout={2};";
        const string UserPasswordFormat = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Connect Timeout={4};";

        /// <summary>
        /// 获取用于创建提供程序对数据源类的实现的实例。
        /// </summary>
        public override DbProviderFactory DbFactory { get { return SqlClientFactory.Instance; } }

        /// <summary>
        /// 获取用生成查询命令的实现实例。
        /// </summary>
        public override ISqlFactory SqlFactory { get { return Factories.SqlFactory.Instance; } }

        /// <summary>
        /// 获取一个值，表示当前数据提供程序的名称。
        /// </summary>
        public override string Name { get { return "sql"; } }

        /// <summary>
        /// 指定数据库的连接字符串，初始化一个 <see cref="SqlEngineProvider"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">数据源的连接字符串。</param>
        public SqlEngineProvider(string connectionString) : base(connectionString) { }

        /// <summary>
        /// 提供数据库连接信息，初始化一个 <see cref="SqlEngineProvider"/> 类的新实例。
        /// </summary>
        /// <param name="dataSource">数据源。</param>
        /// <param name="initialCatalog">数据源。</param>
        public SqlEngineProvider(string dataSource, string initialCatalog) : this(dataSource, initialCatalog, 15) { }

        /// <summary>
        /// 提供数据库连接信息，初始化一个 <see cref="SqlEngineProvider"/> 类的新实例。
        /// </summary>
        /// <param name="dataSource">数据源。</param>
        /// <param name="initialCatalog">数据源。</param>
        /// <param name="connectTimeout">指示连接超时时限。</param>
        public SqlEngineProvider(string dataSource, string initialCatalog, int connectTimeout)
            : this(string.Format(IntegratedSecurityFormat, dataSource, initialCatalog, connectTimeout))
        { }

        /// <summary>
        /// 提供数据库连接信息，初始化一个 <see cref="SqlEngineProvider"/> 类的新实例。
        /// </summary>
        /// <param name="dataSource">数据源。</param>
        /// <param name="initialCatalog">数据源。</param>
        /// <param name="userId">登录账户。</param>
        /// <param name="passwrod">登录密码。</param>
        public SqlEngineProvider(string dataSource, string initialCatalog, string userId, string passwrod)
            : this(dataSource, initialCatalog, userId, passwrod, 15)
        { }

        /// <summary>
        /// 提供数据库连接信息，初始化一个 <see cref="SqlEngineProvider"/> 类的新实例。
        /// </summary>
        /// <param name="dataSource">数据源。</param>
        /// <param name="initialCatalog">数据源。</param>
        /// <param name="userId">登录账户。</param>
        /// <param name="passwrod">登录密码。</param>
        /// <param name="connectTimeout">指示连接超时时限。</param>
        public SqlEngineProvider(string dataSource, string initialCatalog, string userId, string passwrod, int connectTimeout)
            : this(string.Format(UserPasswordFormat, dataSource, initialCatalog, userId, passwrod, connectTimeout))
        { }
    }
}
