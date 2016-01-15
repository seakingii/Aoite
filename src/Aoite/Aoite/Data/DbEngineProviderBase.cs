using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源查询与交互引擎的提供程序基类。
    /// </summary>
    public abstract class DbEngineProviderBase : IDbEngineProvider
    {
        /// <summary>
        /// 获取用于创建提供程序对数据源类的实现的实例。
        /// </summary>
        public abstract DbProviderFactory DbFactory { get; }
        /// <summary>
        /// 获取用生成查询命令的实现实例。
        /// </summary>
        public abstract ISqlFactory SqlFactory { get; }

        /// <summary>
        /// 获取一个值，表示当前数据提供程序的名称。
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 获取用于当前数据源的连接字符串。
        /// </summary>
        public virtual string ConnectionString { get; }

        /// <summary>
        /// 指定数据库的连接字符串，初始化一个 <see cref="DbEngineProviderBase"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">数据源的连接字符串。</param>
        public DbEngineProviderBase(string connectionString)
        {
            if(string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// 创建并返回一个到数据源的连接。
        /// </summary>
        /// <returns>一个到数据源的连接。</returns>
        public virtual DbConnection CreateConnection()
        {
            var conn = this.DbFactory.CreateConnection();
            conn.ConnectionString = this.ConnectionString;
            return conn;
        }
    }
}
