using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

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
        /// 获取用于当前数据源的连接字符串。
        /// </summary>
        string ConnectionString { get; }
        /// <summary>
        /// 获取一个值，表示当前数据提供程序的名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 转义指定位置的名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="point">名称的位置。</param>
        /// <returns>转义后的名称。</returns>
        string EscapeName(string name, NamePoint point);
        /// <summary>
        /// 创建并返回一个到数据源的连接。
        /// </summary>
        /// <returns>一个到数据源的连接。</returns>
        DbConnection CreateConnection();

        /// <summary>
        /// 指定类型映射器创建一个获取最后递增序列值的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateLastIdentityCommand(TypeMapper mapper);
        /// <summary>
        /// 指定类型映射器和实体创建一个插入的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateInsertCommand(TypeMapper mapper, object entity, string tableName = null);
        /// <summary>
        /// 指定类型映射器和实体创建一个更新的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateUpdateCommand(TypeMapper mapper, object entity, string tableName = null);
        /// <summary>
        /// 指定类型映射器和实体创建一个删除的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entityOrPKValue">实体的实例对象（引用类型）或一个主键的值（值类型）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateDeleteCommand(TypeMapper mapper, object entityOrPKValue, string tableName = null);

        /// <summary>
        /// 创建指定查询字符串的统计总行数查询字符串。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        string CreatePageTotalCountCommand(string commandText);
        /// <summary>
        /// 对指定的 <see cref="DbCommand"/> 进行分页处理。
        /// </summary>
        /// <param name="pageNumber">从 1 开始的页码。</param>
        /// <param name="pageSize">页的大小。</param>
        /// <param name="command">数据源查询命令</param>
        void PageProcessCommand(int pageNumber, int pageSize, DbCommand command);
    }
}
