using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个数据源的数据筛选执行器。
    /// </summary>
    public partial interface IFilterExecutor
#if !NET40
        : IFilterExecutorAsync
#endif
    {
        /// <summary>
        /// 根据当前提供匹配条件，执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        int Modify<TEntity>(object entity, ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，执行一个删除的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        int Remove<TEntity>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>表示数据是否存在。</returns>
        bool Exists<TEntity>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>数据的行数。</returns>
        long RowCount<TEntity>(ICommandTunnel tunnel = null);

        /// <summary>
        /// 根据当前提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="select">视图选择器。可以为 null 值，表示不采用匿名对象的方式。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        TView FindOne<TEntity, TView>(Func<TEntity, TView> select, ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="select">视图选择器。可以为 null 值，表示不采用匿名对象的方式。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体的集合。</returns>
        List<TView> FindAll<TEntity, TView>(Func<TEntity, TView> select, ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="select">视图选择器。可以为 null 值，表示不采用匿名对象的方式。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>包含总记录数的实体的集合。</returns>
        PageData<TView> FindAll<TEntity, TView>(Func<TEntity, TView> select, IPagination page, ICommandTunnel tunnel = null);

        /// <summary>
        /// 根据当前提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        TEntity FindOne<TEntity>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        TView FindOne<TEntity, TView>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体的集合。</returns>
        List<TEntity> FindAll<TEntity>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体的集合。</returns>
        List<TView> FindAll<TEntity, TView>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>包含总记录数的实体的集合。</returns>
        PageData<TEntity> FindAll<TEntity>(IPagination page, ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>包含总记录数的实体的集合。</returns>
        PageData<TView> FindAll<TEntity, TView>(IPagination page, ICommandTunnel tunnel = null);
    }

    partial class FilterExecutor : IFilterExecutor
    {
        IDbEngine _engine;
        WhereParameters _where;

        public FilterExecutor(IDbEngine engine, WhereParameters where)
        {
            this._engine = engine;
            this._where = where;
        }

        public int Modify<TEntity>(object entity, ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateUpdateCommand(TypeMapper.Instance<TEntity>.Mapper, entity, this._where, tunnel);
            return this._engine.Execute(command).ToNonQuery();
        }

        public int Remove<TEntity>(ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateDeleteCommand(TypeMapper.Instance<TEntity>.Mapper, this._where, tunnel);
            return this._engine.Execute(command).ToNonQuery();
        }

        public bool Exists<TEntity>(ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateExistsCommand(TypeMapper.Instance<TEntity>.Mapper, this._where, tunnel);
            var r = this._engine.Execute(command).ToScalar();

            return r != null;
        }

        public long RowCount<TEntity>(ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateRowCountCommand(TypeMapper.Instance<TEntity>.Mapper, this._where, tunnel);
            return this._engine.Execute(command).ToScalar<long>();
        }

        public TEntity FindOne<TEntity>(ICommandTunnel tunnel = null)
            => this.FindOne<TEntity, TEntity>(tunnel);

        class AnonymousTypeObject<TView>
        {
            private readonly System.Reflection.ConstructorInfo _ctor;
            public string Fields { get; }

            public AnonymousTypeObject()
            {
                var type = typeof(TView);
                if(!type.IsAnonymous()) throw new ArgumentException("只支持匿名对象的类型", type.FullName);
                this._ctor = type.GetConstructors().First();
                this.Fields = this._ctor.GetParameters().Join(p => p.Name);
            }

            public TView Create(DynamicEntityValue item) =>
                (TView)DynamicFactory.CreateConstructorHandler(_ctor)(item.NameValues.Values.ToArray());
        }

        public TView FindOne<TEntity, TView>(Func<TEntity, TView> select, ICommandTunnel tunnel = null)
        {
            if(select == null) return this.FindOne<TEntity, TView>(tunnel);
            var atObj = new AnonymousTypeObject<TView>();
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, atObj.Fields, this._where, 1, tunnel);
            var entity = this._engine.Execute(command).ToEntity();
            if(entity == null) return default(TView);
            return atObj.Create(entity as DynamicEntityValue);
        }
        public List<TView> FindAll<TEntity, TView>(Func<TEntity, TView> select, ICommandTunnel tunnel = null)
        {
            if(select == null) return this.FindAll<TEntity, TView>(tunnel);
            var atObj = new AnonymousTypeObject<TView>();
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, atObj.Fields, this._where, 0, tunnel);
            var entities = this._engine.Execute(command).ToEntities();

            return (from item in entities select atObj.Create(item as DynamicEntityValue)).ToList();
        }
        public PageData<TView> FindAll<TEntity, TView>(Func<TEntity, TView> select, IPagination page, ICommandTunnel tunnel = null)
        {
            if(select == null) return this.FindAll<TEntity, TView>(page, tunnel);
            var atObj = new AnonymousTypeObject<TView>();
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, atObj.Fields, this._where, 0, tunnel);
            var pageData = this._engine.Execute(command).ToEntities(page);

            return new PageData<TView>()
            {
                Rows = (from item in pageData.Rows select atObj.Create(item as DynamicEntityValue)).ToArray(),
                Total = pageData.Total
            };
        }

        public TView FindOne<TEntity, TView>(ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, TypeMapper.Instance<TView>.Mapper, this._where, 1, tunnel);
            return this._engine.Execute(command).ToEntity<TView>();
        }

        public List<TEntity> FindAll<TEntity>(ICommandTunnel tunnel = null)
            => this.FindAll<TEntity, TEntity>(tunnel);

        public List<TView> FindAll<TEntity, TView>(ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, TypeMapper.Instance<TView>.Mapper, this._where, 0, tunnel);
            return this._engine.Execute(command).ToEntities<TView>();
        }

        public PageData<TEntity> FindAll<TEntity>(IPagination page, ICommandTunnel tunnel = null)
         => this.FindAll<TEntity, TEntity>(page, tunnel);

        public PageData<TView> FindAll<TEntity, TView>(IPagination page, ICommandTunnel tunnel = null)
        {
            if(page == null) throw new ArgumentNullException(nameof(page));

            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, TypeMapper.Instance<TView>.Mapper, this._where, 0, tunnel);
            return this._engine.Execute(command).ToEntities<TView>(page);
        }

    }
}
