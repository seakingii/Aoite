#if !NET40
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoite.CommandModel;
using Aoite.Data;

namespace System
{
    static partial class CommandBusExtensions
    {
        #region Add

        /// <summary>
        /// 异步执行一个添加的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> AddAsync<TEntity>(this ICommandBus bus, TEntity entity, ICommandTunnel tunnel = null)
            => AddAnonymousAsync<TEntity>(bus, entity, tunnel);

        /// <summary>
        /// 异步执行一个添加的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> AddAnonymousAsync<TEntity>(this ICommandBus bus, object entity, ICommandTunnel tunnel = null)
            => bus.ExecuteAsync(new CMD.Add<TEntity>() { Entity = entity, Tunnel = tunnel }).ContinueWith(t => t.Result.Result);

        /// <summary>
        /// 异步执行一个获取递增列值的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>递增列值。</returns>
        public static Task<long> GetIdentityAsync<TEntity>(this ICommandBus bus, ICommandTunnel tunnel = null)
            => bus.ExecuteAsync(new CMD.GetIdentity<TEntity>() { Tunnel = tunnel }).ContinueWith(t => t.Result.Result);

        #endregion

        #region Modify

        /// <summary>
        /// 异步执行一个修改的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> ModifyAsync<TEntity>(this ICommandBus bus, TEntity entity, ICommandTunnel tunnel = null)
            => ModifyAnonymousAsync<TEntity>(bus, entity, tunnel);

        /// <summary>
        /// 异步执行一个修改的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> ModifyAnonymousAsync<TEntity>(this ICommandBus bus, object entity, ICommandTunnel tunnel = null)
            => Filter(bus, DbExtensions.GetModifyKeyValues<TEntity>(entity)).ModifyAsync<TEntity>(entity, tunnel);

        #endregion

        #region Remove

        /// <summary>
        /// 异步执行一个移除的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例对象，在删除命令中 <paramref name="entity"/> 可以是主键的值（表只有一个主键，值允许是一个数组，表示删除多条记录），也可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> RemoveAsync<TEntity>(this ICommandBus bus, TEntity entity, ICommandTunnel tunnel = null)
            => RemoveAnonymousAsync<TEntity>(bus, entity, tunnel);

        /// <summary>
        /// 异步执行一个移除的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entityOrPKValues">实体的实例对象，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键，值允许是一个数组，表示删除多条记录），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static Task<int> RemoveAnonymousAsync<TEntity>(this ICommandBus bus, object entityOrPKValues, ICommandTunnel tunnel = null)
        {
            if(bus == null) throw new ArgumentNullException(nameof(bus));

            return Filter(bus, DbExtensions.GetRemoveWhere<TEntity>(bus.GetDbEngine(), entityOrPKValues)).RemoveAsync<TEntity>(tunnel);
        }


        #endregion

        #region FindOne

        /// <summary>
        /// 异步执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个实体。</returns>
        public static Task<TEntity> FindOneAsync<TEntity>(this ICommandBus bus, object keyValue, ICommandTunnel tunnel = null)
            => FindOneAsync<TEntity, TEntity>(bus, keyValue, tunnel);

        /// <summary>
        /// 异步执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyName">主键的列名。可以为 null 值。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个实体。</returns>
        public static Task<TEntity> FindOneAsync<TEntity>(this ICommandBus bus, string keyName, object keyValue, ICommandTunnel tunnel = null)
            => FindOneAsync<TEntity, TEntity>(bus, keyName, keyValue, tunnel);

        /// <summary>
        /// 异步执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个实体。</returns>
        public static Task<TView> FindOneAsync<TEntity, TView>(this ICommandBus bus, object keyValue, ICommandTunnel tunnel = null)
            => FindOneAsync<TEntity, TView>(bus, null, keyValue, tunnel);

        /// <summary>
        /// 异步执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyName">主键的列名。可以为 null 值。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个实体。</returns>
        public static Task<TView> FindOneAsync<TEntity, TView>(this ICommandBus bus, string keyName, object keyValue, ICommandTunnel tunnel = null)
            => Filter(bus, DbExtensions.GetKeyValues<TEntity>(keyName, keyValue)).FindOneAsync<TEntity, TView>(tunnel);

        #endregion

        #region Exists & RowCount

        /// <summary>
        /// 异步执行一个查询主键是否存在的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        public static Task<bool> ExistsAsync<TEntity>(this ICommandBus bus, object keyValue, ICommandTunnel tunnel = null)
            => ExistsAsync<TEntity>(bus, null, keyValue, tunnel);

        /// <summary>
        /// 异步执行一个查询主键是否存在的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        public static Task<bool> ExistsAsync<TEntity>(this ICommandBus bus, string keyName, object keyValue, ICommandTunnel tunnel = null)
            => Filter(bus, DbExtensions.GetKeyValues<TEntity>(keyName, keyValue)).ExistsAsync<TEntity>(tunnel);

        /// <summary>
        /// 异步执行一个获取查询条件的数据表行数的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>数据的行数。</returns>
        public static Task<long> RowCountAsync<TEntity>(this ICommandBus bus, ICommandTunnel tunnel = null)
            => Filter(bus).RowCountAsync<TEntity>(tunnel);

        #endregion
    }
}
#endif