#if !NET40
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个数据源的异步数据筛选执行器。
    /// </summary>
    public interface IFilterExecutorAsync
    {
        /// <summary>
        /// 根据当前提供匹配条件，异步执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        Task<int> ModifyAsync<TEntity>(object entity, ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，异步执行一个删除的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        Task<int> RemoveAsync<TEntity>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，异步判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>表示数据是否存在。</returns>
        Task<bool> ExistsAsync<TEntity>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，异步获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>数据的行数。</returns>
        Task<long> RowCountAsync<TEntity>(ICommandTunnel tunnel = null);

        /// <summary>
        /// 根据当前提供匹配条件，异步获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="select">视图选择器。可以为 null 值，表示不采用匿名对象的方式。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        Task<TView> FindOneAsync<TEntity, TView>(Func<TEntity, TView> select, ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，异步获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="select">视图选择器。可以为 null 值，表示不采用匿名对象的方式。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体的集合。</returns>
        Task<List<TView>> FindAllAsync<TEntity, TView>(Func<TEntity, TView> select, ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，异步获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="select">视图选择器。可以为 null 值，表示不采用匿名对象的方式。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>包含总记录数的实体的集合。</returns>
        Task<PageData<TView>> FindAllAsync<TEntity, TView>(Func<TEntity, TView> select, IPagination page, ICommandTunnel tunnel = null);

        /// <summary>
        /// 根据当前提供匹配条件，异步获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        Task<TEntity> FindOneAsync<TEntity>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，异步获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体。</returns>
        Task<TView> FindOneAsync<TEntity, TView>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，异步获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体的集合。</returns>
        Task<List<TEntity>> FindAllAsync<TEntity>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，异步获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>实体的集合。</returns>
        Task<List<TView>> FindAllAsync<TEntity, TView>(ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，异步获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>包含总记录数的实体的集合。</returns>
        Task<PageData<TEntity>> FindAllAsync<TEntity>(IPagination page, ICommandTunnel tunnel = null);
        /// <summary>
        /// 根据当前提供匹配条件，异步获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>包含总记录数的实体的集合。</returns>
        Task<PageData<TView>> FindAllAsync<TEntity, TView>(IPagination page, ICommandTunnel tunnel = null);
    }

    partial class FilterExecutor : IFilterExecutorAsync
    {
        public Task<int> ModifyAsync<TEntity>(object entity, ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateUpdateCommand(TypeMapper.Instance<TEntity>.Mapper, entity, this._where, tunnel);
            return this._engine.Execute(command).ToNonQueryAsync();
        }

        public Task<int> RemoveAsync<TEntity>(ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateDeleteCommand(TypeMapper.Instance<TEntity>.Mapper, this._where, tunnel);
            return this._engine.Execute(command).ToNonQueryAsync();
        }

        public Task<bool> ExistsAsync<TEntity>(ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateExistsCommand(TypeMapper.Instance<TEntity>.Mapper, this._where, tunnel);
            return this._engine.Execute(command).ToScalarAsync().ContinueWith(t => t.Result != null);

        }

        public Task<long> RowCountAsync<TEntity>(ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateRowCountCommand(TypeMapper.Instance<TEntity>.Mapper, this._where, tunnel);
            return this._engine.Execute(command).ToScalarAsync<long>();
        }


        public Task<TView> FindOneAsync<TEntity, TView>(Func<TEntity, TView> select, ICommandTunnel tunnel = null)
        {
            if(select == null) return this.FindOneAsync<TEntity, TView>(tunnel);

            var atObj = new AnonymousTypeObject<TView>();
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, atObj.Fields, this._where, 1, tunnel);
            return this._engine.Execute(command).ToEntityAsync().ContinueWith(t =>
            {
                if(t.Result == null) return default(TView);
                return atObj.Create(t.Result as DynamicEntityValue);
            });
        }
        public Task<List<TView>> FindAllAsync<TEntity, TView>(Func<TEntity, TView> select, ICommandTunnel tunnel = null)
        {
            if(select == null) return this.FindAllAsync<TEntity, TView>(tunnel);

            var atObj = new AnonymousTypeObject<TView>();
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, atObj.Fields, this._where, 0, tunnel);
            return this._engine.Execute(command).ToEntitiesAsync().ContinueWith(t =>
            {
                return (from item in t.Result select atObj.Create(item as DynamicEntityValue)).ToList();
            });
        }
        public Task<PageData<TView>> FindAllAsync<TEntity, TView>(Func<TEntity, TView> select, IPagination page, ICommandTunnel tunnel = null)
        {
            if(select == null) return this.FindAllAsync<TEntity, TView>(page, tunnel);

            var atObj = new AnonymousTypeObject<TView>();
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, atObj.Fields, this._where, 0, tunnel);
            return this._engine.Execute(command).ToEntitiesAsync(page).ContinueWith(t =>
            {
                var pageData = t.Result;
                return new PageData<TView>()
                {
                    Rows = (from item in pageData.Rows select atObj.Create(item as DynamicEntityValue)).ToArray(),
                    Total = pageData.Total
                };
            });
        }

        public Task<TEntity> FindOneAsync<TEntity>(ICommandTunnel tunnel = null)
            => this.FindOneAsync<TEntity, TEntity>(tunnel);

        public Task<TView> FindOneAsync<TEntity, TView>(ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, TypeMapper.Instance<TView>.Mapper, this._where, 1, tunnel);
            return this._engine.Execute(command).ToEntityAsync<TView>();
        }

        public Task<List<TEntity>> FindAllAsync<TEntity>(ICommandTunnel tunnel = null)
            => this.FindAllAsync<TEntity, TEntity>(tunnel);

        public Task<List<TView>> FindAllAsync<TEntity, TView>(ICommandTunnel tunnel = null)
        {
            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, TypeMapper.Instance<TView>.Mapper, this._where, 0, tunnel);
            return this._engine.Execute(command).ToEntitiesAsync<TView>();
        }

        public Task<PageData<TEntity>> FindAllAsync<TEntity>(IPagination page, ICommandTunnel tunnel = null)
         => this.FindAllAsync<TEntity, TEntity>(page, tunnel);

        public Task<PageData<TView>> FindAllAsync<TEntity, TView>(IPagination page, ICommandTunnel tunnel = null)
        {
            if(page == null) throw new ArgumentNullException(nameof(page));

            var command = this._engine.Provider.SqlFactory.CreateQueryCommand(TypeMapper.Instance<TEntity>.Mapper, TypeMapper.Instance<TView>.Mapper, this._where, 0, tunnel);
            return this._engine.Execute(command).ToEntitiesAsync<TView>(page);
        }
    }
}
#endif