using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个基于 Microsoft SQL Server 数据源查询与交互引擎的提供程序。
    /// </summary>
    public class SqlEngineProvider : DbEngineProviderBase
    {
        const string IntegratedSecurityFormat = "Data Source={0};Initial Catalog={1};Integrated Security=True;Connect Timeout={2};";
        const string UserPasswordFormat = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Connect Timeout={4};";

        /// <summary>
        /// 获取用于创建提供程序对数据源类的实现的实例。
        /// </summary>
        public override DbProviderFactory DbFactory { get { return SqlClientFactory.Instance; } }

        /// <summary>
        /// 获取一个值，表示当前数据提供程序的名称。
        /// </summary>
        public override string Name { get { return "sql"; } }

        /// <summary>
        /// 指定数据库的连接字符串，初始化 <see cref="SqlEngineProvider"/> 类的新实例。
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

        #region PageProvider

        /// <summary>
        /// 获取分页的字符串格式项。
        /// </summary>
        protected virtual string PageFormat
        {
            get
            {
                return @"SELECT * FROM (SELECT ROW_NUMBER() OVER({4}) AS {1},* FROM ({0}) ____t1____) ____t2____ WHERE {1}>{2} AND {1}<={3}";
            }
        }

        /// <summary>
        /// 获取统计的字符串格式项。
        /// </summary>
        protected virtual string TotalFormat
        {
            get { return @"SELECT COUNT(*) FROM ({0}) ____t____"; }
        }

        static readonly Regex OrderByRegex = new Regex(@"\s*order\s+by\s+[^\s,\)\(]+(?:\s+(?:asc|desc))?(?:\s*,\s*[^\s,\)\(]+(?:\s+(?:asc|desc))?)*", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>
        /// 获取默认页码字段的列名。
        /// </summary>
        public const string DefaultRowNumberName = "_RN_";

        /// <summary>
        /// 获取最后一个匹配的 Order By 结果。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        /// <returns> Order By 结果。</returns>
        protected static Match GetOrderByMatch(string commandText)
        {
            var match = OrderByRegex.Match(commandText);
            while(match.Success)
            {
                if((match.Index + match.Length) == commandText.Length) return match;
                match = match.NextMatch();
            }
            return match;
        }

        /// <summary>
        /// 创建指定查询字符串的统计总行数查询字符串。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        public override string CreatePageTotalCountCommand(string commandText)
        {
            var match = GetOrderByMatch(commandText);
            return string.Format(TotalFormat, match.Success ? commandText.Remove(match.Index) : commandText);
        }

        /// <summary>
        /// 对指定的 <see cref="DbCommand"/> 进行分页处理。
        /// </summary>
        /// <param name="pageNumber">从 1 开始的页码。</param>
        /// <param name="pageSize">页的大小。</param>
        /// <param name="command">数据源查询命令</param>
        public override void PageProcessCommand(int pageNumber, int pageSize, DbCommand command)
        {
            var start = (pageNumber - 1) * pageSize;
            var end = pageNumber * pageSize;
            var match = GetOrderByMatch(command.CommandText);
            var orderBy = "ORDER BY getdate()";
            if(match.Success)
            {
                command.CommandText = command.CommandText.Remove(match.Index);
                orderBy = match.Value.Trim();
            }

            command.CommandText = string.Format(PageFormat
                , command.CommandText
                , DefaultRowNumberName
                , start
                , end
                , orderBy);
        }

        #endregion
    }
}
