using System;
using System.Collections.Generic;
using Aoite.CommandModel;
using Aoite.Data;

namespace CMD
{
    /// <summary>
    /// 表示一个实体命令模型的基类。
    /// </summary>
    public abstract class EntityCommandBase
    {
        /// <summary>
        /// 获取或设置实体的实例。
        /// </summary>
        public object Entity { get; set; }
    }

    /// <summary>
    /// 表示一个条件命令模型的基类。
    /// </summary>
    public abstract class WhereCommandBase
    {
        private WhereParameters _WhereParameters = new WhereParameters();
        /// <summary>
        /// 设置一个 WHERE 的条件参数。
        /// </summary>
        public WhereParameters WhereParameters { get { return this._WhereParameters; } set { this._WhereParameters = value ?? new WhereParameters(); } }
    }


    #region Add

    /// <summary>
    /// 表示一个添加的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class Add<TEntity> : EntityCommandBase, ICommand<long>
    {
        /// <summary>
        /// 设置或获取一个值，指示是否 <typeparamref name="TEntity"/> 是否包含递增列主键。
        /// </summary>
        public bool IdentityKey { get; set; }
        /// <summary>
        /// 设置或获取当 <typeparamref name="TEntity"/> 包含递增列主键时，返回的递增列值。
        /// </summary>
        public long ResultValue { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.Add{TEntity}"/> 类的新实例。
        /// </summary>
        public Add() { }
        /// <summary>
        /// 指示是否包含递增列主键，初始化一个 <see cref="CMD.Add{TEntity}"/> 类的新实例。
        /// </summary>
        /// <param name="identityKey">指示是否 <typeparamref name="TEntity"/> 是否包含递增列主键。</param>
        public Add(bool identityKey) { this.IdentityKey = identityKey; }

        class Executor : IExecutor<Add<TEntity>>
        {
            public void Execute(IContext context, Add<TEntity> command)
            {
                context.Engine.AddAnonymous<TEntity>(command.Entity);
                if(command.IdentityKey)
                    command.ResultValue = context.Engine.GetLastIdentity<TEntity>().UnsafeValue;
            }
        }

    }

    #endregion

    #region Modify

    /// <summary>
    /// 表示一个修改的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class Modify<TEntity> : EntityCommandBase, ICommand<int>
    {
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
                command.ResultValue = context.Engine.ModifyAnonymous<TEntity>(command.Entity);
            }
        }
    }

    #endregion

    #region ModifyWhere

    /// <summary>
    /// 表示一个条件的修改的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class ModifyWhere<TEntity> : WhereCommandBase, ICommand<int>
    {
        /// <summary>
        /// 获取或设置实体的实例。
        /// </summary>
        public object Entity { get; set; }

        /// <summary>
        /// 设置或获取受影响的行。
        /// </summary>
        public int ResultValue { get; set; }
        /// <summary>
        /// 初始化一个 <see cref="CMD.ModifyWhere{TEntity}"/> 类的新实例。
        /// </summary>
        public ModifyWhere() { }
        class Executor : IExecutor<ModifyWhere<TEntity>>
        {
            public void Execute(IContext context, ModifyWhere<TEntity> command)
            {
                command.ResultValue = context.Engine.ModifyWhere<TEntity>(command.Entity
                    , command.WhereParameters.Where
                    , command.WhereParameters.Parameters);
            }
        }
    }

    #endregion

    #region Remove

    /// <summary>
    /// 表示一个移除的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class Remove<TEntity> : EntityCommandBase, ICommand<int>
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
                command.ResultValue = context.Engine.RemoveAnonymous<TEntity>(command.Entity);
            }
        }
    }

    #endregion

    #region RemoveWhere

    /// <summary>
    /// 表示一个条件的修改的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class RemoveWhere<TEntity> : WhereCommandBase, ICommand<int>
    {
        /// <summary>
        /// 设置或获取受影响的行。
        /// </summary>
        public int ResultValue { get; set; }
        /// <summary>
        /// 初始化一个 <see cref="CMD.RemoveWhere{TEntity}"/> 类的新实例。
        /// </summary>
        public RemoveWhere() { }
        class Executor : IExecutor<RemoveWhere<TEntity>>
        {
            public void Execute(IContext context, RemoveWhere<TEntity> command)
            {
                command.ResultValue = context.Engine.RemoveWhere<TEntity>(command.WhereParameters.Where
                    , command.WhereParameters.Parameters);
            }
        }
    }

    #endregion

    #region FindOne

    abstract class FindOneExecutorBase<TEntity, TView, TCommand> : IExecutor<TCommand>
      where TCommand : FindOne<TEntity, TView>
    {
        public void Execute(IContext context, TCommand command)
        {
            command.ResultValue = context.Engine.FindOne<TEntity, TView>(command.KeyName, command.KeyValue);
        }
    }

    /// <summary>
    /// 表示一个查找单项的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class FindOne<TEntity> : FindOne<TEntity, TEntity>
    {
        /// <summary>
        /// 初始化一个 <see cref="CMD.FindOne{TEntity}"/> 类的新实例。
        /// </summary>
        public FindOne() { }
        /// <summary>
        /// 提供实体主键的列值，初始化一个 <see cref="CMD.FindOne{TEntity}"/> 类的新实例。
        /// </summary>
        /// <param name="keyValue">主键的列值。</param>
        public FindOne(object keyValue) : base(keyValue) { }

        class Executor : FindOneExecutorBase<TEntity, TEntity, FindOne<TEntity>> { }
    }

    /// <summary>
    /// 表示一个查找单项的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    /// <typeparam name="TView">实体的视图类型。</typeparam>
    public class FindOne<TEntity, TView> : ICommand<TView>
    {
        /// <summary>
        /// 设置或获取主键的列名。
        /// </summary>
        public string KeyName { get; set; }
        /// <summary>
        /// 设置或获取主键的列值。
        /// </summary>
        public object KeyValue { get; set; }
        /// <summary>
        /// 设置或获取一个实体。
        /// </summary>
        public TView ResultValue { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.FindOne{TEntity, TView}"/> 类的新实例。
        /// </summary>
        public FindOne() { }
        /// <summary>
        /// 提供实体主键的列值，初始化一个 <see cref="CMD.FindOne{TEntity, TView}"/> 类的新实例。
        /// </summary>
        /// <param name="keyValue">主键的列值。</param>
        public FindOne(object keyValue) { this.KeyValue = keyValue; }

        class Executor : FindOneExecutorBase<TEntity, TView, FindOne<TEntity, TView>> { }

    }

    #endregion

    #region FindOneWhere

    abstract class FindOneWhereExecutorBase<TEntity, TView, TCommand> : IExecutor<TCommand>
        where TCommand : FindOneWhere<TEntity, TView>
    {
        public void Execute(IContext context, TCommand command)
        {
            command.ResultValue = context.Engine.FindOneWhere<TEntity, TView>(command.WhereParameters.Where,
                command.WhereParameters.Parameters);
        }
    }

    /// <summary>
    /// 表示一个查找单项的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class FindOneWhere<TEntity> : FindOneWhere<TEntity, TEntity>
    {
        /// <summary>
        /// 初始化一个 <see cref="CMD.FindOneWhere{TEntity}"/> 类的新实例。
        /// </summary>
        public FindOneWhere() { }

        class Executor : FindOneWhereExecutorBase<TEntity, TEntity, FindOneWhere<TEntity>> { }
    }

    /// <summary>
    /// 表示一个查找单项、返回结果为视图类型的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    /// <typeparam name="TView">实体的视图类型。</typeparam>
    public class FindOneWhere<TEntity, TView> : WhereCommandBase, ICommand<TView>
    {
        /// <summary>
        /// 设置或获取一个实体。
        /// </summary>
        public TView ResultValue { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CMD.FindOneWhere{TEntity, TView}"/> 类的新实例。
        /// </summary>
        public FindOneWhere() { }
        class Executor : FindOneWhereExecutorBase<TEntity, TView, FindOneWhere<TEntity, TView>> { }
    }

    #endregion

    #region FindAllWhere

    abstract class FindAllWhereExecutorBase<TEntity, TView, TCommand> : IExecutor<TCommand>
        where TCommand : FindAllWhere<TEntity, TView>
    {
        public void Execute(IContext context, TCommand command)
        {
            command.ResultValue = context.Engine.FindAllWhere<TEntity, TView>(command.WhereParameters.Where,
                command.WhereParameters.Parameters);
        }
    }

    /// <summary>
    /// 表示一个查找多项的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class FindAllWhere<TEntity> : FindAllWhere<TEntity, TEntity>
    {
        /// <summary>
        /// 初始化一个 <see cref="CMD.FindAllWhere{TEntity}"/> 类的新实例。
        /// </summary>
        public FindAllWhere() { }

        class Executor : FindAllWhereExecutorBase<TEntity, TEntity, FindAllWhere<TEntity>> { }
    }

    /// <summary>
    /// 表示一个查找多项、返回结果为视图类型的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    /// <typeparam name="TView">实体的视图类型。</typeparam>
    public class FindAllWhere<TEntity, TView> : WhereCommandBase, ICommand<List<TView>>
    {
        /// <summary>
        /// 设置或获取一个实体的集合。
        /// </summary>
        public List<TView> ResultValue { get; set; }
        /// <summary>
        /// 初始化一个 <see cref="CMD.FindAllWhere{TEntity, TView}"/> 类的新实例。
        /// </summary>
        public FindAllWhere() { }

        class Executor : FindAllWhereExecutorBase<TEntity, TView, FindAllWhere<TEntity, TView>> { }
    }

    #endregion

    #region FindAllPage

    abstract class FindAllPageExecutorBase<TEntity, TView, TCommand> : IExecutor<TCommand>
        where TCommand : FindAllPage<TEntity, TView>
    {
        public void Execute(IContext context, TCommand command)
        {
            command.ResultValue = context.Engine.FindAllPage<TEntity, TView>(command.Page,
                command.WhereParameters.Where,
                command.WhereParameters.Parameters);
        }
    }

    /// <summary>
    /// 表示一个以分页方式查找多项的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class FindAllPage<TEntity> : FindAllPage<TEntity, TEntity>
    {
        /// <summary>
        /// 初始化一个 <see cref="CMD.FindAllPage{TEntity}"/> 类的新实例。
        /// </summary>
        public FindAllPage() { }

        /// <summary>
        /// 提供分页数据，初始化一个 <see cref="CMD.FindAllPage{TEntity}"/> 类的新实例。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。默认为 10。</param>
        public FindAllPage(int pageNumber, int pageSize = 10)
        {
            this.Page = new Pagination(pageNumber, pageSize);
        }

        class Executor : FindAllPageExecutorBase<TEntity, TEntity, FindAllPage<TEntity>> { }
    }

    /// <summary>
    /// 表示一个以分页方式查找多项、返回结果为视图类型的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    /// <typeparam name="TView">实体的视图类型。</typeparam>
    public class FindAllPage<TEntity, TView> : WhereCommandBase, ICommand<PageData<TView>>
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

        /// <summary>
        /// 提供分页数据，初始化一个 <see cref="CMD.FindAllPage{TEntity, TView}"/> 类的新实例。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。默认为 10。</param>
        public FindAllPage(int pageNumber, int pageSize = 10)
        {
            this.Page = new Pagination(pageNumber, pageSize);
        }

        class Executor : FindAllPageExecutorBase<TEntity, TView, FindAllPage<TEntity, TView>> { }

    }

    #endregion

    #region Exists

    /// <summary>
    /// 表示一个查询主键是否存在的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class Exists<TEntity> : FindOne<bool>
    {
        /// <summary>
        /// 初始化一个 <see cref="CMD.Exists{TEntity}"/> 类的新实例。
        /// </summary>
        public Exists() { }
        /// <summary>
        /// 提供实体主键的列值，初始化一个 <see cref="CMD.Exists{TEntity}"/> 类的新实例。
        /// </summary>
        /// <param name="keyValue">主键的列值。</param>
        public Exists(object keyValue) : base(keyValue) { }

        class Executor : IExecutor<Exists<TEntity>>
        {
            public void Execute(IContext context, Exists<TEntity> command)
            {
                command.ResultValue = context.Engine.Exists<TEntity>(command.KeyName, command.KeyValue);
            }
        }
    }

    #endregion

    #region ExistsWhere

    /// <summary>
    /// 表示一个查询条件是否存在的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class ExistsWhere<TEntity> : WhereCommandBase, ICommand<bool>
    {
        /// <summary>
        /// 设置或获取一个值，指示数据是否存在。
        /// </summary>
        public bool ResultValue { get; set; }
        /// <summary>
        /// 初始化一个 <see cref="CMD.ExistsWhere{TEntity}"/> 类的新实例。
        /// </summary>
        public ExistsWhere() { }

        class Executor : IExecutor<ExistsWhere<TEntity>>
        {
            public void Execute(IContext context, ExistsWhere<TEntity> command)
            {
                command.ResultValue = context.Engine.ExistsWhere<TEntity>(command.WhereParameters.Where,
                    command.WhereParameters.Parameters);
            }
        }
    }

    #endregion

    #region RowCount

    /// <summary>
    /// 表示一个获取查询条件的数据表行数的命令模型。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class RowCount<TEntity> : WhereCommandBase, ICommand<long>
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
                command.ResultValue = context.Engine.RowCount<TEntity>(command.WhereParameters.Where,
                    command.WhereParameters.Parameters);
            }
        }
    }

    #endregion
}

