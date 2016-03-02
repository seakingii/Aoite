using System.Collections.Generic;
using System.Threading.Tasks;
using Aoite.CommandModel;
using Aoite.Data;

namespace System
{
    /// <summary>
    /// 表示 <see cref="ICommandBus"/> 的扩展方法。
    /// </summary>
    public static partial class CommandBusExtensions
    {
        #region Add

        /// <summary>
        /// 执行一个添加的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static long Add<TEntity>(this ICommandBus bus, TEntity entity, ICommandTunnel tunnel = null)
            => AddAnonymous<TEntity>(bus, entity, tunnel);

        /// <summary>
        /// 执行一个添加的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static long AddAnonymous<TEntity>(this ICommandBus bus, object entity, ICommandTunnel tunnel = null)
            => bus.Call(new CMD.Add<TEntity>() { Entity = entity, Tunnel = tunnel });

        /// <summary>
        /// 执行一个获取递增列值的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>递增列值。</returns>
        public static long GetIdentity<TEntity>(this ICommandBus bus, ICommandTunnel tunnel = null)
            => bus.Call(new CMD.GetIdentity<TEntity>() { Tunnel = tunnel });

        #endregion

        #region Modify

        /// <summary>
        /// 执行一个修改的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static int Modify<TEntity>(this ICommandBus bus, TEntity entity, ICommandTunnel tunnel = null)
            => ModifyAnonymous<TEntity>(bus, entity, tunnel);

        /// <summary>
        /// 执行一个修改的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static int ModifyAnonymous<TEntity>(this ICommandBus bus, object entity, ICommandTunnel tunnel = null)
            => Filter(bus, DbExtensions.GetModifyKeyValues<TEntity>(entity)).Modify<TEntity>(entity, tunnel);

        #endregion

        #region Remove

        /// <summary>
        /// 执行一个移除的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例对象，在删除命令中 <paramref name="entity"/> 可以是主键的值（表只有一个主键，值允许是一个数组，表示删除多条记录），也可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static int Remove<TEntity>(this ICommandBus bus, TEntity entity, ICommandTunnel tunnel = null)
            => RemoveAnonymous<TEntity>(bus, entity, tunnel);

        /// <summary>
        /// 执行一个移除的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entityOrPKValues">实体的实例对象，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键，值允许是一个数组，表示删除多条记录），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static int RemoveAnonymous<TEntity>(this ICommandBus bus, object entityOrPKValues, ICommandTunnel tunnel = null)
        {
            if(bus == null) throw new ArgumentNullException(nameof(bus));

            return Filter(bus, DbExtensions.GetRemoveWhere<TEntity>(bus.GetDbEngine(), entityOrPKValues)).Remove<TEntity>(tunnel);
        }


        #endregion

        #region FindOne

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        public static TEntity FindOne<TEntity>(this ICommandBus bus, object keyValue, ICommandTunnel tunnel = null)
            => FindOne<TEntity, TEntity>(bus, keyValue, tunnel);

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyName">主键的列名。可以为 null 值。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        public static TEntity FindOne<TEntity>(this ICommandBus bus, string keyName, object keyValue, ICommandTunnel tunnel = null)
            => FindOne<TEntity, TEntity>(bus, keyName, keyValue, tunnel);

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        public static TView FindOne<TEntity, TView>(this ICommandBus bus, object keyValue, ICommandTunnel tunnel = null)
            => FindOne<TEntity, TView>(bus, null, keyValue, tunnel);

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyName">主键的列名。可以为 null 值。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        public static TView FindOne<TEntity, TView>(this ICommandBus bus, string keyName, object keyValue, ICommandTunnel tunnel = null)
            => Filter(bus, DbExtensions.GetKeyValues<TEntity>(keyName, keyValue)).FindOne<TEntity, TView>(tunnel);


        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="select">视图选择器。可以为 null 值，表示不采用匿名对象的方式。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        public static TView FindOne<TEntity, TView>(this ICommandBus bus, object keyValue, Func<TEntity, TView> select, ICommandTunnel tunnel = null)
            => FindOne(bus, null, keyValue, select, tunnel);

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyName">主键的列名。可以为 null 值。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="select">视图选择器。可以为 null 值，表示不采用匿名对象的方式。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        public static TView FindOne<TEntity, TView>(this ICommandBus bus, string keyName, object keyValue, Func<TEntity, TView> select, ICommandTunnel tunnel = null)
            => Filter(bus, DbExtensions.GetKeyValues<TEntity>(keyName, keyValue)).FindOne(select, tunnel);

        #endregion

        #region Exists & RowCount

        /// <summary>
        /// 执行一个查询主键是否存在的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>表示数据是否存在。</returns>
        public static bool Exists<TEntity>(this ICommandBus bus, object keyValue, ICommandTunnel tunnel = null)
            => Exists<TEntity>(bus, null, keyValue, tunnel);

        /// <summary>
        /// 执行一个查询主键是否存在的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>表示数据是否存在。</returns>
        public static bool Exists<TEntity>(this ICommandBus bus, string keyName, object keyValue, ICommandTunnel tunnel = null)
            => Filter(bus, DbExtensions.GetKeyValues<TEntity>(keyName, keyValue)).Exists<TEntity>(tunnel);

        /// <summary>
        /// 执行一个一个获取查询条件的数据表行数的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>数据的行数。</returns>
        public static long RowCount<TEntity>(this ICommandBus bus, ICommandTunnel tunnel = null)
            => Filter(bus).RowCount<TEntity>(tunnel);

        #endregion

        #region Filter

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="bus">命令总线。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this ICommandBus bus)
            => Filter(bus, WhereParameters.Empty);

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="bus">命令总线。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this ICommandBus bus, object objectInstance, string binary = "AND")
            => Filter(bus, new ExecuteParameterCollection(objectInstance), binary);

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="bus">命令总线。</param>
        /// <param name="keysAndValues">应当是 <see cref="string"/> / <see cref="object"/> 的字典集合。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this ICommandBus bus, params object[] keysAndValues)
            => Filter(bus, new ExecuteParameterCollection(keysAndValues));

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="bus">命令总线。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this ICommandBus bus, ExecuteParameterCollection ps, string binary = "AND")
            => Filter(bus, DbExtensions.CreateWhere(bus.GetDbEngine(), ps, binary), ps);

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="bus">命令总线。</param>
        /// <param name="whereCallback">一个创建查询条件的回调方法。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this ICommandBus bus, Action<IWhere> whereCallback)
        {
            if(bus == null) throw new ArgumentNullException(nameof(bus));
            var builder = new SqlBuilder(bus.GetDbEngine());
            whereCallback(builder.Where());
            return Filter(bus, builder.WhereText, builder.Parameters);
        }

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this ICommandBus bus, string where, ExecuteParameterCollection ps)
            => Filter(bus, new WhereParameters(where, ps));

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件参数。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this ICommandBus bus, WhereParameters where)
        {
            if(bus == null) throw new ArgumentNullException(nameof(bus));
            if(where == null) throw new ArgumentNullException(nameof(where));

            return new CommandFilterExecutor(bus, where);
        }

        #endregion

        #region Run

        /// <summary>
        /// 调用一个命令模型，并返回命令模型执行的值。
        /// </summary>
        /// <typeparam name="TResult">返回值的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="command">命令模型。</param>
        /// <param name="executing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        /// <returns>命令模型的值。</returns>
        public static TResult Call<TResult>(this ICommandBus bus,ICommand<TResult> command
            , CommandExecutingHandler<ICommand<TResult>> executing = null
            , CommandExecutedHandler<ICommand<TResult>> executed = null)
        {
            return bus.Execute(command, executing, executed).Result;
        }

        /// <summary>
        /// 以异步的方式调用一个命令模型，并返回命令模型执行的值的异步操作。
        /// </summary>
        /// <typeparam name="TResult">返回值的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="command">命令模型。</param>
        /// <param name="executing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        /// <returns>异步操作。</returns>
        public static Task<TResult> CallAsync<TResult>(this ICommandBus bus, ICommand<TResult> command
            , CommandExecutingHandler<ICommand<TResult>> executing = null
            , CommandExecutedHandler<ICommand<TResult>> executed = null)
        {
            return bus.ExecuteAsync(command, executing, executed).ContinueWith(t => t.Result.Result);
        }


        #endregion
    }
}
