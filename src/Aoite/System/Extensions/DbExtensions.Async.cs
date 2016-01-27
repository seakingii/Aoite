#if !NET40
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoite.Data;

namespace System
{
    static partial class DbExtensions
    {

        #region Add

        /// <summary>
        /// 异步执行一个插入的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> AddAsync<TEntity>(this IDbEngine engine, TEntity entity, ICommandTunnel tunnel = null)
            => AddAnonymousAsync<TEntity>(engine, entity, tunnel);

        /// <summary>
        /// 异步执行一个插入的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> AddAnonymousAsync<TEntity>(this IDbEngine engine, object entity, ICommandTunnel tunnel = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            if(entity == null) throw new ArgumentNullException(nameof(entity));

            var command = engine.Provider.SqlFactory.CreateInsertCommand(TypeMapper.Instance<TEntity>.Mapper, entity, tunnel);
            return engine.Execute(command).ToNonQueryAsync();
        }

        #endregion

        #region Modify

        /// <summary>
        /// 异步执行一个更新的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> ModifyAsync<TEntity>(this IDbEngine engine, TEntity entity, ICommandTunnel tunnel = null)
            => ModifyAnonymousAsync<TEntity>(engine, entity, tunnel);

        /// <summary>
        /// 异步执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> ModifyAnonymousAsync<TEntity>(this IDbEngine engine, object entity, ICommandTunnel tunnel = null)
            => Filter(engine, GetModifyKeyValues<TEntity>(entity)).ModifyAsync<TEntity>(entity, tunnel);

        #endregion

        #region Remove

        /// <summary>
        /// 异步执行一个删除的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> RemoveAsync<TEntity>(this IDbEngine engine, TEntity entity, ICommandTunnel tunnel = null)
            => RemoveAnonymousAsync<TEntity>(engine, entity, tunnel);

        /// <summary>
        /// 异步执行一个删除的命令，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entityOrPKValues">实体的实例对象，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键，值允许是一个数组，表示删除多条记录），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> RemoveAnonymousAsync<TEntity>(this IDbEngine engine, object entityOrPKValues, ICommandTunnel tunnel = null)
            => Filter(engine, GetRemoveWhere<TEntity>(engine, entityOrPKValues)).RemoveAsync<TEntity>(tunnel);

        #endregion

        #region FineOne

        /// <summary>
        /// 异步获取指定 <paramref name="keyValue"/> 值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        public static Task<TEntity> FindOneAsync<TEntity>(this IDbEngine engine, object keyValue, ICommandTunnel tunnel = null)
            => FindOneAsync<TEntity, TEntity>(engine, keyValue, tunnel);

        /// <summary>
        /// 异步获取指定 <paramref name="keyName"/> 键值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。可以为 null 值。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        public static Task<TEntity> FindOneAsync<TEntity>(this IDbEngine engine, string keyName, object keyValue, ICommandTunnel tunnel = null)
            => FindOneAsync<TEntity, TEntity>(engine, keyName, keyValue, tunnel);

        /// <summary>
        /// 异步获取指定 <paramref name="keyValue"/> 值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        public static Task<TView> FindOneAsync<TEntity, TView>(this IDbEngine engine, object keyValue, ICommandTunnel tunnel = null)
            => FindOneAsync<TEntity, TView>(engine, null, keyValue, tunnel);

        /// <summary>
        /// 异步获取指定 <paramref name="keyName"/> 键值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。可以为 null 值。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        public static Task<TView> FindOneAsync<TEntity, TView>(this IDbEngine engine, string keyName, object keyValue, ICommandTunnel tunnel = null)
            => Filter(engine, GetKeyValues<TEntity>(keyName, keyValue)).FindOneAsync<TEntity, TView>(tunnel);

        #endregion

        #region Exists & RowCount & Select

        /// <summary>
        /// 异步判断指定的主键的列名的值是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>表示数据是否存在。</returns>
        public static Task<bool> ExistsAsync<TEntity>(this IDbEngine engine, object keyValue, ICommandTunnel tunnel = null)
            => ExistsAsync<TEntity>(engine, null, keyValue, tunnel);

        /// <summary>
        /// 异步判断指定的主键的列名的值是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>表示数据是否存在。</returns>
        public static Task<bool> ExistsAsync<TEntity>(this IDbEngine engine, string keyName, object keyValue, ICommandTunnel tunnel = null)
             => Filter(engine, GetKeyValues<TEntity>(keyName, keyValue)).ExistsAsync<TEntity>(tunnel);

        /// <summary>
        /// 异步获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>数据的行数。</returns>
        public static Task<long> RowCountAsync<TEntity>(this IDbEngine engine, ICommandTunnel tunnel = null)
            => Filter(engine).RowCountAsync<TEntity>(tunnel);

        /// <summary>
        /// 异步获取最后递增序列值。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>递增序列值。</returns>
        public static Task<long> GetLastIdentityAsync<TEntity>(this IDbEngine engine, ICommandTunnel tunnel = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            var command = engine.Provider.SqlFactory.CreateLastIdentityCommand(TypeMapper.Instance<TEntity>.Mapper, tunnel);
            return engine.Execute(command).ToScalarAsync<long>();
        }

        #endregion
    }
}
#endif