using System;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 定义数据源查询与交互引擎的提供程序。
    /// </summary>
    public interface IDbEngineProvider
    {
        /// <summary>
        /// 获取用于创建提供程序对数据源类的实现的实例。
        /// </summary>
        DbProviderFactory DbFactory { get; }
        /// <summary>
        /// 获取用生成查询命令的实现实例。
        /// </summary>
        ISqlFactory SqlFactory { get; }
        /// <summary>
        /// 获取用于当前数据源的连接字符串。
        /// </summary>
        string ConnectionString { get; }
        /// <summary>
        /// 获取一个值，表示当前数据提供程序的名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 创建并返回一个到数据源的连接。
        /// </summary>
        /// <returns>一个到数据源的连接。</returns>
        DbConnection CreateConnection();
    }
}
