using System;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个数据库命令生成工厂。
    /// </summary>
    public interface ISqlFactory
    {
        /// <summary>
        /// 指定类型映射器和条件参数创建一个删除的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="where">条件参数。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateDeleteCommand(TypeMapper mapper, WhereParameters where, string tableName = null);
        /// <summary>
        /// 指定类型映射器和实体创建一个删除的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entityOrPKValue">实体的实例对象（引用类型）或一个主键的值（值类型）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateDeleteCommand(TypeMapper mapper, object entityOrPKValue, string tableName = null);
        /// <summary>
        /// 指定类型映射器和条件参数创建一个行是否存在的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="where">条件参数。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateExistsCommand(TypeMapper mapper, WhereParameters where, string tableName = null);
        /// <summary>
        /// 创建指定视图类型的字段列表。
        /// </summary>
        /// <param name="entityMapper">实体的类型映射器。</param>
        /// <param name="viewMapper">视图的类型映射器。</param>
        /// <returns>包含在 <paramref name="entityMapper"/> 的 <paramref name="viewMapper"/> 属性集合，并对每个属性进行转义。</returns>
        string CreateFields(TypeMapper entityMapper, TypeMapper viewMapper);
        /// <summary>
        /// 指定类型映射器和实体创建一个插入的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateInsertCommand(TypeMapper mapper, object entity, string tableName = null);
        /// <summary>
        /// 指定类型映射器创建一个获取最后递增序列值的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateLastIdentityCommand(TypeMapper mapper, string tableName = null);
        /// <summary>
        /// 创建指定查询字符串的统计总行数查询字符串。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        string CreatePageTotalCountCommand(string commandText);
        /// <summary>
        /// 指定实体类型映射器、视图映射器和条件创建一个查询的命令。
        /// </summary>
        /// <param name="entityMapper">实体的类型映射器。</param>
        /// <param name="viewMapper">视图的类型映射器。</param>
        /// <param name="where">条件参数。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <param name="top">指定 TOP 数量，小于 1 则忽略作用。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateQueryCommand(TypeMapper entityMapper, TypeMapper viewMapper, WhereParameters where, string tableName = null, int top = 0);
        /// <summary>
        /// 指定类型映射器和条件参数创建一个表总行数的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="where">条件参数。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateRowCountCommand(TypeMapper mapper, WhereParameters where, string tableName = null);
        /// <summary>
        /// 指定类型映射器和实体创建一个更新的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateUpdateCommand(TypeMapper mapper, object entity, string tableName = null);
        /// <summary>
        /// 指定类型映射器和实体创建一个更新的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="where">条件参数。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        ExecuteCommand CreateUpdateCommand(TypeMapper mapper, object entity, WhereParameters where, string tableName = null);
        /// <summary>
        /// 转义指定位置的名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="point">名称的位置。</param>
        /// <returns>转义后的名称。</returns>
        string EscapeName(string name, NamePoint point);
        /// <summary>
        /// 对指定的 <see cref="DbCommand"/> 进行分页处理。
        /// </summary>
        /// <param name="pageNumber">从 1 开始的页码。</param>
        /// <param name="pageSize">页的大小。</param>
        /// <param name="command">数据源查询命令</param>
        void PageProcessCommand(int pageNumber, int pageSize, DbCommand command);
    }
}