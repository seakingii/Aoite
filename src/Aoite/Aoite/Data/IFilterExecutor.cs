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
    public interface IFilterExecutor
    {
        /// <summary>
        /// 根据当前提供匹配条件，执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        int Modify<TEntity>(object entity, string tableName = null);
        /// <summary>
        /// 根据当前提供匹配条件，执行一个删除的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        int Remove<TEntity>(string tableName = null);

        /// <summary>
        /// 根据当前提供匹配条件，判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        bool Exists<TEntity>(string tableName = null);

        /// <summary>
        /// 根据当前提供匹配条件，获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>数据的行数。</returns>
        long RowCount<TEntity>(string tableName = null);

        /// <summary>
        /// 根据当前提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个实体。</returns>
        TEntity FindOne<TEntity>(string tableName = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个实体。</returns>
        TView FindOne<TEntity, TView>(string tableName = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个实体的集合。</returns>
        List<TEntity> FindAll<TEntity>(string tableName = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个实体的集合。</returns>
        List<TView> FindAll<TEntity, TView>(string tableName = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个实体的分页集合。</returns>
        PageData<TEntity> FindAll<TEntity>(IPagination page, string tableName = null);
        /// <summary>
        /// 根据当前提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个实体的分页集合。</returns>
        PageData<TView> FindAll<TEntity, TView>(IPagination page, string tableName = null);
    }

    class FilterExecutor : IFilterExecutor
    {
        IDbEngine _engine;
        WhereParameters _where;

        public FilterExecutor(IDbEngine engine, WhereParameters where)
        {
            this._engine = engine;
            this._where = where;
        }

        public int Modify<TEntity>(object entity, string tableName = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateUpdateCommand(TypeMapper.Instance<TEntity>.Mapper, entity, this._where, tableName);
            return this._engine.Execute(command).ToNonQuery();
        }

        public int Remove<TEntity>(string tableName = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateDeleteCommand(TypeMapper.Instance<TEntity>.Mapper, this._where, tableName);
            return this._engine.Execute(command).ToNonQuery();
        }

        public bool Exists<TEntity>(string tableName = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateExistsCommand(TypeMapper.Instance<TEntity>.Mapper, this._where, tableName);
            var r = this._engine.Execute(command).ToScalar();

            return r != null;
        }

        public long RowCount<TEntity>(string tableName = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateRowCountCommand(TypeMapper.Instance<TEntity>.Mapper, this._where, tableName);
            return this._engine.Execute(command).ToScalar<long>();
        }

        public TEntity FindOne<TEntity>(string tableName = null)
            => this.FindOne<TEntity, TEntity>(tableName);

        public TView FindOne<TEntity, TView>(string tableName = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, TypeMapper.Instance<TView>.Mapper, this._where, tableName, 1);
            return this._engine.Execute(command).ToEntity<TView>();
        }

        public List<TEntity> FindAll<TEntity>(string tableName = null)
            => this.FindAll<TEntity, TEntity>(tableName);

        public List<TView> FindAll<TEntity, TView>(string tableName = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, TypeMapper.Instance<TView>.Mapper, this._where, tableName);
            return this._engine.Execute(command).ToEntities<TView>();
        }

        public PageData<TEntity> FindAll<TEntity>(IPagination page, string tableName = null)
         => this.FindAll<TEntity, TEntity>(page, tableName);

        public PageData<TView> FindAll<TEntity, TView>(IPagination page, string tableName = null)
        {
            if(page == null) throw new ArgumentNullException(nameof(page));

            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, TypeMapper.Instance<TView>.Mapper, this._where, tableName);
            return this._engine.Execute(command).ToEntities<TView>(page);
        }
    }
}
