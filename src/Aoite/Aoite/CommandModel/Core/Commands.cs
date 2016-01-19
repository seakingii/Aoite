using System;
using System.Collections.Generic;
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
        /// 设置或获取用于个性化表名和命令的暗道，可以为 null 值。
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
        /// 设置或获取一个 WHERE 的条件参数。
        /// </summary>
        public WhereParameters Where { get { return this._Where; } set { this._Where = value ?? new WhereParameters(); } }
    }

    /// <summary>
    /// 表示一个添加的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class Add<TEntity> : CMDBase, ICommand<long>
    {
        /// <summary>
        /// 获取或设置要添加的实体，可以是匿名对象的部分成员（<see cref="Entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        public object Entity { get; set; }

        /// <summary>
        /// 设置或获取一个值，指示是否 <typeparamref name="TEntity"/> 是否包含递增列主键。
        /// </summary>
        public bool IsIdentityKey { get; set; }

        /// <summary>
        /// 设置或获取一个值，若 <see cref="IsIdentityKey"/> 为 true 时返回的递增列值，否则返回受影响的行。
        /// </summary>
        public long ResultValue { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.Add{TEntity}"/> 类的新实例。
        /// </summary>
        public Add() { }

        class Executor : IExecutor<Add<TEntity>>
        {
            public void Execute(IContext context, Add<TEntity> command)
            {
                command.ResultValue = context.Engine.AddAnonymous<TEntity>(command.Entity, command.Tunnel);
                if(command.IsIdentityKey) command.ResultValue = context.Engine.GetLastIdentity<TEntity>(command.Tunnel);
            }
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
        /// 设置或获取受影响的行。
        /// </summary>
        public int ResultValue { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.Modify{TEntity}"/> 类的新实例。
        /// </summary>
        public Modify() { }

        class Executor : IExecutor<Modify<TEntity>>
        {
            public void Execute(IContext context, Modify<TEntity> command)
            {
                command.ResultValue = context.Engine.Filter(command.Where).Modify<TEntity>(command.Entity, command.Tunnel);
            }
        }
    }

    /// <summary>
    /// 表示一个移除的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class Remove<TEntity> : CMDWhereBase, ICommand<int>
    {
        /// <summary>
        /// 设置或获取受影响的行。
        /// </summary>
        public int ResultValue { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.Remove{TEntity}"/> 类的新实例。
        /// </summary>
        public Remove() { }

        class Executor : IExecutor<Remove<TEntity>>
        {
            public void Execute(IContext context, Remove<TEntity> command)
            {
                command.ResultValue = context.Engine.Filter(command.Where).Remove<TEntity>(command.Tunnel);
            }
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
        /// 设置或获取一个实体。
        /// </summary>
        public TView ResultValue { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.FindOne{TEntity, TView}"/> 类的新实例。
        /// </summary>
        public FindOne() { }

        class Executor : IExecutor<FindOne<TEntity, TView>>
        {
            public void Execute(IContext context, FindOne<TEntity, TView> command)
            {
                command.ResultValue = context.Engine.Filter(command.Where).FindOne<TEntity, TView>(command.Tunnel);
            }
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
        /// 设置或获取一个实体的集合。
        /// </summary>
        public List<TView> ResultValue { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.FindAll{TEntity, TView}"/> 类的新实例。
        /// </summary>
        public FindAll() { }

        class Executor : IExecutor<FindAll<TEntity, TView>>
        {
            public void Execute(IContext context, FindAll<TEntity, TView> command)
            {
                command.ResultValue = context.Engine.Filter(command.Where).FindAll<TEntity, TView>(command.Tunnel);
            }
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
        /// 设置或获取分页的数据。
        /// </summary>
        public IPagination Page { get; set; }

        /// <summary>
        /// 设置或获取一个实体的分页集合。
        /// </summary>
        public PageData<TView> ResultValue { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.FindAllPage{TEntity, TView}"/> 类的新实例。
        /// </summary>
        public FindAllPage() { }

        class Executor : IExecutor<FindAllPage<TEntity, TView>>
        {
            public void Execute(IContext context, FindAllPage<TEntity, TView> command)
            {
                command.ResultValue = context.Engine.Filter(command.Where).FindAll<TEntity, TView>(command.Page, command.Tunnel);
            }
        }

    }

    /// <summary>
    /// 表示一个查询条件是否存在的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class Exists<TEntity> : CMDWhereBase, ICommand<bool>
    {
        /// <summary>
        /// 设置或获取一个值，指示数据是否存在。
        /// </summary>
        public bool ResultValue { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.Exists{TEntity}"/> 类的新实例。
        /// </summary>
        public Exists() { }

        class Executor : IExecutor<Exists<TEntity>>
        {
            public void Execute(IContext context, Exists<TEntity> command)
            {
                command.ResultValue = context.Engine.Filter(command.Where).Exists<TEntity>(command.Tunnel);
            }
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
        public long ResultValue { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.RowCount{TEntity}"/> 类的新实例。
        /// </summary>
        public RowCount() { }

        class Executor : IExecutor<RowCount<TEntity>>
        {
            public void Execute(IContext context, RowCount<TEntity> command)
            {
                command.ResultValue = context.Engine.Filter(command.Where).RowCount<TEntity>(command.Tunnel);
            }
        }
    }
}

