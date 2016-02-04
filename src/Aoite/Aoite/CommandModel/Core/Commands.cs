using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aoite.CommandModel;
using Aoite.Data;

namespace CMD
{
    /// <summary>
    /// 表示一个实体命令模型的基类。
    /// </summary>
    public abstract class CMDBase
    {
        /// <summary>
        /// 获取或设置用于个性化表名和命令的暗道，可以为 null 值。
        /// </summary>
        public ICommandTunnel Tunnel { get; set; }
    }

    /// <summary>
    /// 表示一个条件命令模型的基类。
    /// </summary>
    public abstract class CMDWhereBase : CMDBase
    {
        private WhereParameters _Where = new WhereParameters();
        /// <summary>
        /// 获取或设置一个 WHERE 的条件参数。
        /// </summary>
        public WhereParameters Where { get { return this._Where; } set { this._Where = value ?? new WhereParameters(); } }
    }

    /// <summary>
    /// 表示一个添加的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class Add<TEntity> : CMDBase, ICommand<int>
    {
        /// <summary>
        /// 获取或设置要添加的实体，可以是匿名对象的部分成员（<see cref="Entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        public object Entity { get; set; }

        /// <summary>
        /// 获取或设置受影响的行。
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.Add{TEntity}"/> 类的新实例。
        /// </summary>
        public Add() { }

        class Executor
#if !NET40
            : ExecutorAsyncBase<Add<TEntity>, int>
        {
            protected override Task<int> ExecuteResultAsync(IContext context, Add<TEntity> command)
                => context.Engine.AddAnonymousAsync<TEntity>(command.Entity, command.Tunnel);
#else
            : ExecutorBase<Add<TEntity>, int>
        {
#endif
            protected override int ExecuteResult(IContext context, Add<TEntity> command)
                => context.Engine.AddAnonymous<TEntity>(command.Entity, command.Tunnel);
        }
    }

    /// <summary>
    /// 表示一个获取当前上下文递增列值的命令。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class GetIdentity<TEntity> : CMDBase, ICommand<long>
    {
        /// <summary>
        /// 获取或设置一个值，表示递增列值。
        /// </summary>
        public long Result { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.GetIdentity{TEntity}"/> 类的新实例。
        /// </summary>
        public GetIdentity() { }

        class Executor
#if !NET40
            : ExecutorAsyncBase<GetIdentity<TEntity>, long>
        {
            protected override Task<long> ExecuteResultAsync(IContext context, GetIdentity<TEntity> command)
                => context.Engine.GetLastIdentityAsync<TEntity>(command.Tunnel);
#else
            : ExecutorBase<GetIdentity<TEntity>, long>
        {
#endif
            protected override long ExecuteResult(IContext context, GetIdentity<TEntity> command)
                => context.Engine.GetLastIdentity<TEntity>(command.Tunnel);
        }
    }

    /// <summary>
    /// 表示一个修改的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class Modify<TEntity> : CMDWhereBase, ICommand<int>
    {
        /// <summary>
        /// 获取或设置要修改的实体，可以是匿名对象的部分成员（<see cref="Entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        public object Entity { get; set; }

        /// <summary>
        /// 获取或设置受影响的行。
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.Modify{TEntity}"/> 类的新实例。
        /// </summary>
        public Modify() { }

        class Executor
#if !NET40
            : ExecutorAsyncBase<Modify<TEntity>, int>
        {
            protected override Task<int> ExecuteResultAsync(IContext context, Modify<TEntity> command)
                => context.Engine.Filter(command.Where).ModifyAsync<TEntity>(command.Entity, command.Tunnel);
#else
            : ExecutorBase<Modify<TEntity>, int>
        {
#endif
            protected override int ExecuteResult(IContext context, Modify<TEntity> command)
                => context.Engine.Filter(command.Where).Modify<TEntity>(command.Entity, command.Tunnel);
        }
    }

    /// <summary>
    /// 表示一个移除的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class Remove<TEntity> : CMDWhereBase, ICommand<int>
    {
        /// <summary>
        /// 获取或设置受影响的行。
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.Remove{TEntity}"/> 类的新实例。
        /// </summary>
        public Remove() { }

        class Executor
#if !NET40
            : ExecutorAsyncBase<Remove<TEntity>, int>
        {
            protected override Task<int> ExecuteResultAsync(IContext context, Remove<TEntity> command)
                => context.Engine.Filter(command.Where).RemoveAsync<TEntity>(command.Tunnel);
#else
            : ExecutorBase<Remove<TEntity>, int>
        {
#endif
            protected override int ExecuteResult(IContext context, Remove<TEntity> command)
                => context.Engine.Filter(command.Where).Remove<TEntity>(command.Tunnel);

        }
    }

    /// <summary>
    /// 表示一个查找单项、返回结果为视图类型的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    /// <typeparam name="TView">实体的视图类型。</typeparam>
    public class FindOne<TEntity, TView> : CMDWhereBase, ICommand<TView>
    {
        /// <summary>
        /// 获取或设置一个实体。
        /// </summary>
        public TView Result { get; set; }
        /// <summary>
        /// 获取或设置视图选择器。可以为 null 值，表示不采用匿名对象的方式。
        /// </summary>
        public Func<TEntity, TView> Select { get; set; }
        /// <summary>
        /// 初始化一个 <see cref="CMD.FindOne{TEntity, TView}"/> 类的新实例。
        /// </summary>
        public FindOne() { }

        class Executor
#if !NET40
            : ExecutorAsyncBase<FindOne<TEntity, TView>, TView>
        {
            protected override Task<TView> ExecuteResultAsync(IContext context, FindOne<TEntity, TView> command)
                => context.Engine.Filter(command.Where).FindOneAsync(command.Select, command.Tunnel);
#else
            : ExecutorBase<FindOne<TEntity, TView>, TView>
        {
#endif
            protected override TView ExecuteResult(IContext context, FindOne<TEntity, TView> command)
                => context.Engine.Filter(command.Where).FindOne(command.Select, command.Tunnel);

        }
    }

    /// <summary>
    /// 表示一个查找多项、返回结果为视图类型的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    /// <typeparam name="TView">实体的视图类型。</typeparam>
    public class FindAll<TEntity, TView> : CMDWhereBase, ICommand<List<TView>>
    {
        /// <summary>
        /// 获取或设置一个实体的集合。
        /// </summary>
        public List<TView> Result { get; set; }
        /// <summary>
        /// 获取或设置视图选择器。可以为 null 值，表示不采用匿名对象的方式。
        /// </summary>
        public Func<TEntity, TView> Select { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.FindAll{TEntity, TView}"/> 类的新实例。
        /// </summary>
        public FindAll() { }

        class Executor
#if !NET40
            : ExecutorAsyncBase<FindAll<TEntity, TView>, List<TView>>
        {
            protected override Task<List<TView>> ExecuteResultAsync(IContext context, FindAll<TEntity, TView> command)
                => context.Engine.Filter(command.Where).FindAllAsync(command.Select, command.Tunnel);
#else
            : ExecutorBase<FindAll<TEntity, TView>, List<TView>>
        {
#endif
            protected override List<TView> ExecuteResult(IContext context, FindAll<TEntity, TView> command)
               => context.Engine.Filter(command.Where).FindAll(command.Select, command.Tunnel);

        }
    }

    /// <summary>
    /// 表示一个以分页方式查找多项、返回结果为视图类型的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    /// <typeparam name="TView">实体的视图类型。</typeparam>
    public class FindAllPage<TEntity, TView> : CMDWhereBase, ICommand<PageData<TView>>
    {
        /// <summary>
        /// 获取或设置分页的数据。
        /// </summary>
        public IPagination Page { get; set; }
        /// <summary>
        /// 获取或设置视图选择器。可以为 null 值，表示不采用匿名对象的方式。
        /// </summary>
        public Func<TEntity, TView> Select { get; set; }

        /// <summary>
        /// 获取或设置一个实体的分页集合。
        /// </summary>
        public PageData<TView> Result { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.FindAllPage{TEntity, TView}"/> 类的新实例。
        /// </summary>
        public FindAllPage() { }

        class Executor
#if !NET40
            : ExecutorAsyncBase<FindAllPage<TEntity, TView>, PageData<TView>>
        {
            protected override Task<PageData<TView>> ExecuteResultAsync(IContext context, FindAllPage<TEntity, TView> command)
                => context.Engine.Filter(command.Where).FindAllAsync(command.Select, command.Page, command.Tunnel);
#else
            : ExecutorBase<FindAllPage<TEntity, TView>, PageData<TView>>
        {
#endif
            protected override PageData<TView> ExecuteResult(IContext context, FindAllPage<TEntity, TView> command)
                => context.Engine.Filter(command.Where).FindAll(command.Select, command.Page, command.Tunnel);

        }

    }

    /// <summary>
    /// 表示一个查询条件是否存在的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class Exists<TEntity> : CMDWhereBase, ICommand<bool>
    {
        /// <summary>
        /// 获取或设置一个值，指示数据是否存在。
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.Exists{TEntity}"/> 类的新实例。
        /// </summary>
        public Exists() { }

        class Executor
#if !NET40
            : ExecutorAsyncBase<Exists<TEntity>, bool>
        {
            protected override Task<bool> ExecuteResultAsync(IContext context, Exists<TEntity> command)
                => context.Engine.Filter(command.Where).ExistsAsync<TEntity>(command.Tunnel);
#else
            : ExecutorBase<Exists<TEntity>, bool>
        {
#endif
            protected override bool ExecuteResult(IContext context, Exists<TEntity> command)
                => context.Engine.Filter(command.Where).Exists<TEntity>(command.Tunnel);

        }
    }

    /// <summary>
    /// 表示一个获取查询条件的数据表行数的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class RowCount<TEntity> : CMDWhereBase, ICommand<long>
    {
        /// <summary>
        /// 获取或设置数据的行数。
        /// </summary>
        public long Result { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.RowCount{TEntity}"/> 类的新实例。
        /// </summary>
        public RowCount() { }

        class Executor
#if !NET40
            : ExecutorAsyncBase<RowCount<TEntity>, long>
        {
            protected override Task<long> ExecuteResultAsync(IContext context, RowCount<TEntity> command)
                => context.Engine.Filter(command.Where).RowCountAsync<TEntity>(command.Tunnel);
#else
            : ExecutorBase<RowCount<TEntity>, long>
        {
#endif
            protected override long ExecuteResult(IContext context, RowCount<TEntity> command)
                => context.Engine.Filter(command.Where).RowCount<TEntity>(command.Tunnel);

        }
    }
}

