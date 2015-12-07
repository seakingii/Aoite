using System.Collections.Generic;
using Aoite.CommandModel;
using Aoite.Data;

namespace System
{
    /// <summary>
    /// 表示 <see cref="ICommandBus"/> 的扩展方法。
    /// </summary>
    public static class CommandBusExtensions
    {
        #region Add

        /// <summary>
        /// 执行一个添加的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="identityKey">指示是否 <typeparamref name="TEntity"/> 是否包含递增列主键。</param>
        /// <returns>当 <typeparamref name="TEntity"/> 包含递增列主键时，返回的递增列值。</returns>
        public static long Add<TEntity>(this ICommandBus bus, TEntity entity, bool identityKey = false)
            => AddAnonymous<TEntity>(bus, entity, identityKey);

        /// <summary>
        /// 执行一个添加的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="identityKey">指示是否 <typeparamref name="TEntity"/> 是否包含递增列主键。</param>
        /// <returns>当 <typeparamref name="TEntity"/> 包含递增列主键时，返回的递增列值。</returns>
        public static long AddAnonymous<TEntity>(this ICommandBus bus, object entity, bool identityKey = false)
            => bus.Execute(new CMD.Add<TEntity>(identityKey) { Entity = entity }).ResultValue;

        #endregion

        #region Modify

        /// <summary>
        /// 执行一个修改的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <returns>受影响的行。</returns>
        public static int Modify<TEntity>(this ICommandBus bus, TEntity entity)
            => ModifyAnonymous<TEntity>(bus, entity);

        /// <summary>
        /// 执行一个修改的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <returns>受影响的行。</returns>
        public static int ModifyAnonymous<TEntity>(this ICommandBus bus, object entity)
            => bus.Execute(new CMD.Modify<TEntity>() { Entity = entity }).ResultValue;

        /// <summary>
        /// 执行一个条件的修改的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>受影响的行。</returns>
        public static int ModifyWhere<TEntity>(this ICommandBus bus, object entity, string where, object objectInstance)
            => ModifyWhere<TEntity>(bus, entity, where, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 执行一个条件的修改的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>受影响的行。</returns>
        public static int ModifyWhere<TEntity>(this ICommandBus bus, object entity, string where, ExecuteParameterCollection ps = null)
            => ModifyWhere<TEntity>(bus, entity, new WhereParameters(where, ps));

        /// <summary>
        /// 执行一个条件的修改的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>受影响的行。</returns>
        public static int ModifyWhere<TEntity>(this ICommandBus bus, object entity, object objectInstance, string binary = "AND")
            => ModifyWhere<TEntity>(bus, entity, WhereParameters.Parse(bus.GetDbEngine(), objectInstance, binary));

        /// <summary>
        /// 执行一个条件的修改的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="ps">参数集合。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>受影响的行。</returns>
        public static int ModifyWhere<TEntity>(this ICommandBus bus, object entity, ExecuteParameterCollection ps, string binary = "AND")
            => ModifyWhere<TEntity>(bus, entity, WhereParameters.Parse(bus.GetDbEngine(), ps, binary));

        /// <summary>
        /// 执行一个条件的修改的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例。</param>
        /// <param name="whereParameters">一个 WHERE 的条件参数。</param>
        /// <returns>受影响的行。</returns>
        public static int ModifyWhere<TEntity>(this ICommandBus bus, object entity, WhereParameters whereParameters)
            => bus.Execute(new CMD.ModifyWhere<TEntity>() { Entity = entity, WhereParameters = whereParameters }).ResultValue;

        #endregion

        #region Remove

        /// <summary>
        /// 执行一个移除的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entity">实体的实例对象，在删除命令中 <paramref name="entity"/> 可以是主键的值（表只有一个主键，值允许是一个数组，表示删除多条记录），也可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <returns>受影响的行。</returns>
        public static int Remove<TEntity>(this ICommandBus bus, TEntity entity)
            => RemoveAnonymous<TEntity>(bus, entity);

        /// <summary>
        /// 执行一个移除的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="entityOrPKValues">实体的实例对象，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键，值允许是一个数组，表示删除多条记录），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <returns>受影响的行。</returns>
        public static int RemoveAnonymous<TEntity>(this ICommandBus bus, object entityOrPKValues)
            => bus.Execute(new CMD.Remove<TEntity>() { Entity = entityOrPKValues }).ResultValue;

        /// <summary>
        /// 执行一个移除的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>受影响的行。</returns>
        public static int RemoveWhere<TEntity>(this ICommandBus bus, string where, object objectInstance)
            => RemoveWhere<TEntity>(bus, where, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 执行一个移除的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>受影响的行。</returns>
        public static int RemoveWhere<TEntity>(this ICommandBus bus, string where, ExecuteParameterCollection ps = null)
            => RemoveWhere<TEntity>(bus, new WhereParameters(where, ps));

        /// <summary>
        /// 执行一个移除的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>受影响的行。</returns>
        public static int RemoveWhere<TEntity>(this ICommandBus bus, object objectInstance, string binary = "AND")
            => RemoveWhere<TEntity>(bus, WhereParameters.Parse(bus.GetDbEngine(), objectInstance, binary));

        /// <summary>
        /// 执行一个移除的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>受影响的行。</returns>
        public static int RemoveWhere<TEntity>(this ICommandBus bus, ExecuteParameterCollection ps, string binary = "AND")
            => RemoveWhere<TEntity>(bus, WhereParameters.Parse(bus.GetDbEngine(), ps, binary));

        /// <summary>
        /// 执行一个移除的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="whereParameters">一个 WHERE 的条件参数。</param>
        /// <returns>受影响的行。</returns>
        public static int RemoveWhere<TEntity>(this ICommandBus bus, WhereParameters whereParameters)
            => bus.Execute(new CMD.RemoveWhere<TEntity>() { WhereParameters = whereParameters }).ResultValue;

        #endregion

        #region FindOne

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <returns>一个实体。</returns>
        public static TEntity FindOne<TEntity>(this ICommandBus bus, object keyValue)
            => FindOne<TEntity>(bus, null, keyValue);

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <returns>一个实体。</returns>
        public static TEntity FindOne<TEntity>(this ICommandBus bus, string keyName, object keyValue)
            => bus.Execute(new CMD.FindOne<TEntity>(keyValue) { KeyName = keyName }).ResultValue;

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <returns>一个实体。</returns>
        public static TView FindOne<TEntity, TView>(this ICommandBus bus, object keyValue)
            => FindOne<TEntity, TView>(bus, null, keyValue);

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <returns>一个实体。</returns>
        public static TView FindOne<TEntity, TView>(this ICommandBus bus, string keyName, object keyValue)
            => bus.Execute(new CMD.FindOne<TEntity, TView>(keyValue) { KeyName = keyName }).ResultValue;

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>一个实体。</returns>
        public static TEntity FindOneWhere<TEntity>(this ICommandBus bus, string where, object objectInstance)
            => FindOneWhere<TEntity>(bus, where, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>一个实体。</returns>
        public static TEntity FindOneWhere<TEntity>(this ICommandBus bus, string where, ExecuteParameterCollection ps = null)
            => FindOneWhere<TEntity>(bus, new WhereParameters(where, ps));

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体。</returns>
        public static TEntity FindOneWhere<TEntity>(this ICommandBus bus, object objectInstance, string binary = "AND")
            => FindOneWhere<TEntity>(bus, WhereParameters.Parse(bus.GetDbEngine(), objectInstance, binary));

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体。</returns>
        public static TEntity FindOneWhere<TEntity>(this ICommandBus bus, ExecuteParameterCollection ps, string binary = "AND")
            => FindOneWhere<TEntity>(bus, WhereParameters.Parse(bus.GetDbEngine(), ps, binary));
        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="whereParameters">一个 WHERE 的条件参数。</param>
        /// <returns>一个实体。</returns>
        public static TEntity FindOneWhere<TEntity>(this ICommandBus bus, WhereParameters whereParameters)
            => bus.Execute(new CMD.FindOneWhere<TEntity>() { WhereParameters = whereParameters }).ResultValue;

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>一个实体。</returns>
        public static TView FindOneWhere<TEntity, TView>(this ICommandBus bus, string where, object objectInstance)
            => FindOneWhere<TEntity, TView>(bus, where, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>一个实体。</returns>
        public static TView FindOneWhere<TEntity, TView>(this ICommandBus bus, string where, ExecuteParameterCollection ps = null)
            => FindOneWhere<TEntity, TView>(bus, new WhereParameters(where, ps));

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体。</returns>
        public static TView FindOneWhere<TEntity, TView>(this ICommandBus bus, object objectInstance, string binary = "AND")
            => FindOneWhere<TEntity, TView>(bus, WhereParameters.Parse(bus.GetDbEngine(), objectInstance, binary));

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体。</returns>
        public static TView FindOneWhere<TEntity, TView>(this ICommandBus bus, ExecuteParameterCollection ps, string binary = "AND")
            => FindOneWhere<TEntity, TView>(bus, WhereParameters.Parse(bus.GetDbEngine(), ps, binary));

        /// <summary>
        /// 执行一个查找单项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="whereParameters">一个 WHERE 的条件参数。</param>
        /// <returns>一个实体。</returns>
        public static TView FindOneWhere<TEntity, TView>(this ICommandBus bus, WhereParameters whereParameters)
            => bus.Execute(new CMD.FindOneWhere<TEntity, TView>() { WhereParameters = whereParameters }).ResultValue;

        #endregion

        #region FindAllWhere

        /// <summary>
        /// 执行一个查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>一个实体的集合。</returns>
        public static List<TEntity> FindAllWhere<TEntity>(this ICommandBus bus, string where, object objectInstance)
            => FindAllWhere<TEntity>(bus, where, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 执行一个查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>一个实体的集合。</returns>
        public static List<TEntity> FindAllWhere<TEntity>(this ICommandBus bus, string where, ExecuteParameterCollection ps = null)
            => FindAllWhere<TEntity>(bus, new WhereParameters(where, ps));

        /// <summary>
        /// 执行一个查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体的集合。</returns>
        public static List<TEntity> FindAllWhere<TEntity>(this ICommandBus bus, object objectInstance, string binary = "AND")
            => FindAllWhere<TEntity>(bus, WhereParameters.Parse(bus.GetDbEngine(), objectInstance, binary));

        /// <summary>
        /// 执行一个查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体的集合。</returns>
        public static List<TEntity> FindAllWhere<TEntity>(this ICommandBus bus, ExecuteParameterCollection ps, string binary = "AND")
            => FindAllWhere<TEntity>(bus, WhereParameters.Parse(bus.GetDbEngine(), ps, binary));

        /// <summary>
        /// 执行一个查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="whereParameters">一个 WHERE 的条件参数。</param>
        /// <returns>一个实体的集合。</returns>
        public static List<TEntity> FindAllWhere<TEntity>(this ICommandBus bus, WhereParameters whereParameters)
            => bus.Execute(new CMD.FindAllWhere<TEntity>() { WhereParameters = whereParameters }).ResultValue;

        /// <summary>
        /// 执行一个查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>一个实体的集合。</returns>
        public static List<TView> FindAllWhere<TEntity, TView>(this ICommandBus bus, string where, object objectInstance)
            => FindAllWhere<TEntity, TView>(bus, where, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 执行一个查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>一个实体的集合。</returns>
        public static List<TView> FindAllWhere<TEntity, TView>(this ICommandBus bus, string where, ExecuteParameterCollection ps = null)
            => FindAllWhere<TEntity, TView>(bus, new WhereParameters(where, ps));

        /// <summary>
        /// 执行一个查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体的集合。</returns>
        public static List<TView> FindAllWhere<TEntity, TView>(this ICommandBus bus, object objectInstance, string binary = "AND")
            => FindAllWhere<TEntity, TView>(bus, WhereParameters.Parse(bus.GetDbEngine(), objectInstance, binary));

        /// <summary>
        /// 执行一个查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体的集合。</returns>
        public static List<TView> FindAllWhere<TEntity, TView>(this ICommandBus bus, ExecuteParameterCollection ps, string binary = "AND")
            => FindAllWhere<TEntity, TView>(bus, WhereParameters.Parse(bus.GetDbEngine(), ps, binary));

        /// <summary>
        /// 执行一个查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="whereParameters">一个 WHERE 的条件参数。</param>
        /// <returns>一个实体的集合。</returns>
        public static List<TView> FindAllWhere<TEntity, TView>(this ICommandBus bus, WhereParameters whereParameters)
            => bus.Execute(new CMD.FindAllWhere<TEntity, TView>() { WhereParameters = whereParameters }).ResultValue;

        #endregion

        #region FindAllPage

        /// <summary>
        /// 执行一个以分页方式查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="page">分页的数据。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>一个实体的分页集合。</returns>
        public static PageData<TEntity> FindAllPage<TEntity>(this ICommandBus bus, IPagination page, string where, object objectInstance)
            => FindAllPage<TEntity>(bus, page, where, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 执行一个以分页方式查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="page">分页的数据。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>一个实体的分页集合。</returns>
        public static PageData<TEntity> FindAllPage<TEntity>(this ICommandBus bus, IPagination page, string where, ExecuteParameterCollection ps = null)
            => FindAllPage<TEntity>(bus, page, new WhereParameters(where, ps));

        /// <summary>
        /// 执行一个以分页方式查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="page">分页的数据。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体的分页集合。</returns>
        public static PageData<TEntity> FindAllPage<TEntity>(this ICommandBus bus, IPagination page, object objectInstance, string binary = "AND")
            => FindAllPage<TEntity>(bus, page, WhereParameters.Parse(bus.GetDbEngine(), objectInstance, binary));

        /// <summary>
        /// 执行一个以分页方式查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="page">分页的数据。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体的分页集合。</returns>
        public static PageData<TEntity> FindAllPage<TEntity>(this ICommandBus bus, IPagination page, ExecuteParameterCollection ps, string binary = "AND")
            => FindAllPage<TEntity>(bus, page, WhereParameters.Parse(bus.GetDbEngine(), ps, binary));

        /// <summary>
        /// 执行一个以分页方式查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="page">分页的数据。</param>
        /// <param name="whereParameters">一个 WHERE 的条件参数。</param>
        /// <returns>一个实体的分页集合。</returns>
        public static PageData<TEntity> FindAllPage<TEntity>(this ICommandBus bus, IPagination page, WhereParameters whereParameters)
            => bus.Execute(new CMD.FindAllPage<TEntity>() { Page = page, WhereParameters = whereParameters }).ResultValue;

        /// <summary>
        /// 执行一个以分页方式查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="page">分页的数据。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>一个实体的分页集合。</returns>
        public static PageData<TView> FindAllPage<TEntity, TView>(this ICommandBus bus, IPagination page, string where, object objectInstance)
            => FindAllPage<TEntity, TView>(bus, page, where, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 执行一个以分页方式查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="page">分页的数据。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>一个实体的分页集合。</returns>
        public static PageData<TView> FindAllPage<TEntity, TView>(this ICommandBus bus, IPagination page, string where, ExecuteParameterCollection ps = null)
            => FindAllPage<TEntity, TView>(bus, page, new WhereParameters(where, ps));

        /// <summary>
        /// 执行一个以分页方式查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="page">分页的数据。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体的分页集合。</returns>
        public static PageData<TView> FindAllPage<TEntity, TView>(this ICommandBus bus, IPagination page, object objectInstance, string binary = "AND")
            => FindAllPage<TEntity, TView>(bus, page, WhereParameters.Parse(bus.GetDbEngine(), objectInstance, binary));

        /// <summary>
        /// 执行一个以分页方式查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="page">分页的数据。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个实体的分页集合。</returns>
        public static PageData<TView> FindAllPage<TEntity, TView>(this ICommandBus bus, IPagination page, ExecuteParameterCollection ps, string binary = "AND")
            => FindAllPage<TEntity, TView>(bus, page, WhereParameters.Parse(bus.GetDbEngine(), ps, binary));

        /// <summary>
        /// 执行一个以分页方式查找多项的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="page">分页的数据。</param>
        /// <param name="whereParameters">一个 WHERE 的条件参数。</param>
        /// <returns>一个实体的分页集合。</returns>
        public static PageData<TView> FindAllPage<TEntity, TView>(this ICommandBus bus, IPagination page, WhereParameters whereParameters)
            => bus.Execute(new CMD.FindAllPage<TEntity, TView>() { Page = page, WhereParameters = whereParameters }).ResultValue;

        #endregion

        #region Exists

        /// <summary>
        /// 执行一个查询主键是否存在的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        public static bool Exists<TEntity>(this ICommandBus bus, object keyValue)
            => Exists<TEntity>(bus, null, keyValue);

        /// <summary>
        /// 执行一个查询主键是否存在的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        public static bool Exists<TEntity>(this ICommandBus bus, string keyName, object keyValue)
            => bus.Execute(new CMD.Exists<TEntity>(keyValue) { KeyName = keyName }).ResultValue;

        /// <summary>
        /// 执行一个查询条件是否存在的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        public static bool ExistsWhere<TEntity>(this ICommandBus bus, string where, object objectInstance)
            => ExistsWhere<TEntity>(bus, where, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 执行一个查询条件是否存在的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        public static bool ExistsWhere<TEntity>(this ICommandBus bus, string where, ExecuteParameterCollection ps = null)
            => ExistsWhere<TEntity>(bus, new WhereParameters(where, ps));

        /// <summary>
        /// 执行一个查询条件是否存在的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        public static bool ExistsWhere<TEntity>(this ICommandBus bus, object objectInstance, string binary = "AND")
            => ExistsWhere<TEntity>(bus, WhereParameters.Parse(bus.GetDbEngine(), objectInstance, binary));

        /// <summary>
        /// 执行一个查询条件是否存在的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        public static bool ExistsWhere<TEntity>(this ICommandBus bus, ExecuteParameterCollection ps, string binary = "AND")
            => ExistsWhere<TEntity>(bus, WhereParameters.Parse(bus.GetDbEngine(), ps, binary));

        /// <summary>
        /// 执行一个查询条件是否存在的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="whereParameters">一个 WHERE 的条件参数。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        public static bool ExistsWhere<TEntity>(this ICommandBus bus, WhereParameters whereParameters)
            => bus.Execute(new CMD.ExistsWhere<TEntity>() { WhereParameters = whereParameters }).ResultValue;

        #endregion

        #region RowCount

        /// <summary>
        /// 执行一个一个获取查询条件的数据表行数的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>数据的行数。</returns>
        public static long RowCount<TEntity>(this ICommandBus bus, string where, object objectInstance)
            => RowCount<TEntity>(bus, where, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 执行一个一个获取查询条件的数据表行数的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>数据的行数。</returns>
        public static long RowCount<TEntity>(this ICommandBus bus, string where, ExecuteParameterCollection ps = null)
            => RowCount<TEntity>(bus, new WhereParameters(where, ps));

        /// <summary>
        /// 执行一个一个获取查询条件的数据表行数的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>数据的行数。</returns>
        public static long RowCount<TEntity>(this ICommandBus bus, object objectInstance, string binary = "AND")
            => RowCount<TEntity>(bus, WhereParameters.Parse(bus.GetDbEngine(), objectInstance, binary));

        /// <summary>
        /// 执行一个一个获取查询条件的数据表行数的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>数据的行数。</returns>
        public static long RowCount<TEntity>(this ICommandBus bus, ExecuteParameterCollection ps, string binary = "AND")
            => RowCount<TEntity>(bus, WhereParameters.Parse(bus.GetDbEngine(), ps, binary));

        /// <summary>
        /// 执行一个一个获取查询条件的数据表行数的命令模型。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="bus">命令总线。</param>
        /// <param name="whereParameters">一个 WHERE 的条件参数。</param>
        /// <returns>数据的行数。</returns>
        public static long RowCount<TEntity>(this ICommandBus bus, WhereParameters whereParameters)
            => bus.Execute(new CMD.RowCount<TEntity>() { WhereParameters = whereParameters }).ResultValue;

        #endregion
    }
}
