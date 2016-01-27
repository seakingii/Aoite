#if !NET40
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Aoite.Data
{
    partial class DbExecutor : IDbExecutorAsync
    {
        private TValue ConvertScallrValueAsync<TValue>(Task<object> t)
        {
            if(t.IsFaulted) return default(TValue);
            var value = t.Result;
            if(Convert.IsDBNull(value)) value = default(TValue);
            else if(!(value is TValue)) value = value.CastTo<TValue>();
            return (TValue)value;
        }

        /// <summary>
        /// 异步执行数据源的查询与交互。
        /// </summary>
        /// <typeparam name="TValue">返回结果的数据类型。</typeparam>
        /// <param name="type">执行的命令类型。</param>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="callback">执行时的回调方法。</param>
        /// <returns>执行结果。</returns>
        protected virtual Task<TValue> ExecuteAsync<TValue>(ExecuteType type, CancellationToken cancellationToken, Func<DbCommand, CancellationToken, Task<TValue>> callback)
        {
            var dbCommand = this.CreateDbCommand();
            this.OnExecuting(type, dbCommand);

            this.Open();

            return callback(dbCommand, cancellationToken).ContinueWith(t =>
            {
                if(t.IsFaulted) GA.OnGlobalError(this.Engine, t.Exception);
                if(this._closeAfterFinally) this.Close();

                var value = t.Result;
                this.OnExecuted(type, dbCommand, value);
                return value;
            });
        }

        #region Basic

        /// <summary>
        /// 异步执行查询命令，并返回受影响的行数。
        /// </summary>
        /// <returns>受影响的行数。</returns>
        public Task<int> ToNonQueryAsync() => this.ToNonQueryAsync(CancellationToken.None);
        /// <summary>
        /// 异步执行查询命令，并返回受影响的行数。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>受影响的行数。</returns>
        public virtual Task<int> ToNonQueryAsync(CancellationToken cancellationToken)
        {
            return this.ExecuteAsync(ExecuteType.NoQuery, cancellationToken, (dbCommand, ct) => dbCommand.ExecuteNonQueryAsync(ct));
        }

        /// <summary>
        /// 异步执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <returns>结果集中第一行的第一列。</returns>
        public Task<object> ToScalarAsync() => this.ToScalarAsync(CancellationToken.None);
        /// <summary>
        /// 异步执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>结果集中第一行的第一列。</returns>
        public Task<object> ToScalarAsync(CancellationToken cancellationToken) => this.ToScalarAsync<object>(cancellationToken);
        /// <summary>
        /// 指定值的数据类型，异步执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <returns>具备强类型的结果集中第一行的第一列。</returns>
        public Task<TValue> ToScalarAsync<TValue>() => this.ToScalarAsync<TValue>(CancellationToken.None);
        /// <summary>
        /// 指定值的数据类型，异步执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <returns>具备强类型的结果集中第一行的第一列。</returns>
        public virtual Task<TValue> ToScalarAsync<TValue>(CancellationToken cancellationToken)
        {
            return this.ExecuteAsync(ExecuteType.Scalar, cancellationToken, (dbCommand, ct) => dbCommand.ExecuteScalarAsync(ct).ContinueWith(ConvertScallrValueAsync<TValue>));
        }

        /// <summary>
        /// 异步执行查询命令，并返回数据集。
        /// </summary>
        /// <returns>数据集。</returns>
        public Task<DataSet> ToDataSetAsync()
        {
            return ToDataSetAsync(CancellationToken.None);
        }
        /// <summary>
        /// 异步执行查询命令，并返回数据集。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>数据集。</returns>
        public Task<DataSet> ToDataSetAsync(CancellationToken cancellationToken)
        {
            return ToDataSetAsync<DataSet>(cancellationToken);
        }
        /// <summary>
        /// 异步执行查询命令，并返回自定义的数据集。
        /// </summary>
        /// <typeparam name="TDataSet">自定义的数据集类型。</typeparam>
        /// <returns>具备强类型的一个数据集。</returns>
        public Task<TDataSet> ToDataSetAsync<TDataSet>() where TDataSet : DataSet, new()
        {
            return ToDataSetAsync<TDataSet>(CancellationToken.None);
        }
        /// <summary>
        /// 异步执行查询命令，并返回自定义的数据集。
        /// </summary>
        /// <typeparam name="TDataSet">自定义的数据集类型。</typeparam>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>具备强类型的一个数据集。</returns>
        public virtual Task<TDataSet> ToDataSetAsync<TDataSet>(CancellationToken cancellationToken) where TDataSet : DataSet, new()
        {
            return this.ExecuteAsync(ExecuteType.DataSet, cancellationToken, (dbCommand, ct) => Task.Run(() => dbCommand.ExecuteDataSet<TDataSet>(this.CreateDataAdapter(dbCommand)), ct));
        }

        /// <summary>
        /// 异步执行查询命令，并返回表。
        /// </summary>
        /// <returns>一张表。</returns>
        public Task<PageTable> ToTableAsync()
        {
            return ToTableAsync(CancellationToken.None);
        }
        /// <summary>
        /// 异步执行查询命令，并返回表。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>一张表。</returns>
        public virtual Task<PageTable> ToTableAsync(CancellationToken cancellationToken)
        {
            return this.ExecuteAsync(ExecuteType.Table, cancellationToken, (dbCommand, ct) => Task.Run(() => dbCommand.ExecuteTable(this.CreateDataAdapter(dbCommand)), ct));
        }
        /// <summary>
        /// 异步执行分页查询命令，并返回表。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一张包含总记录数的表。</returns>
        public Task<PageTable> ToTableAsync(int pageNumber, int pageSize = 10)
        {
            return ToTableAsync(CancellationToken.None, pageNumber, pageSize);
        }
        /// <summary>
        /// 异步执行分页查询命令，并返回表。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一张包含总记录数的表。</returns>
        public virtual Task<PageTable> ToTableAsync(CancellationToken cancellationToken, int pageNumber, int pageSize = 10)
        {
            if(pageNumber < 1) pageNumber = 1;
            if(pageSize < 1) pageSize = 1;

            return this.ExecuteAsync(ExecuteType.Table
                , cancellationToken
                , (dbCommand, ct) => Task.Run(() => InnerToTable(dbCommand, pageNumber, pageSize), ct));
        }

        /// <summary>
        /// 异步异步执行查询命令，并执行给定的读取器的回调函数。
        /// </summary>
        /// <param name="callback">给定的读取器委托。</param>
        public Task ToReaderAsync(ExecuteReaderHandler callback) => this.ToReaderAsync(CancellationToken.None, callback);
        /// <summary>
        /// 异步异步执行查询命令，并执行给定的读取器的回调函数。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="callback">给定的读取器委托。</param>
        public virtual Task ToReaderAsync(CancellationToken cancellationToken, ExecuteReaderHandler callback)
        {
            return this.ExecuteAsync(ExecuteType.Reader, cancellationToken, (dbCommand, ct) =>
            {
                return dbCommand.ExecuteReaderAsync(ct).ContinueWith<object>(t =>
                {
                    using(var reader = t.Result)
                    {
                        callback(reader);
                    }
                    return null;
                });
            });
        }

        /// <summary>
        /// 异步执行查询命令，并执行给定的读取器的回调函数。
        /// </summary>
        /// <typeparam name="TValue">返回值的数据类型。</typeparam>
        /// <param name="callback">给定的读取器委托。</param>
        /// <returns>回调结果的值。</returns>
        public virtual Task<TValue> ToReaderAsync<TValue>(ExecuteReaderHandler<TValue> callback) => this.ToReaderAsync(CancellationToken.None, callback);
        /// <summary>
        /// 异步执行查询命令，并执行给定的读取器的回调函数。
        /// </summary>
        /// <typeparam name="TValue">返回值的数据类型。</typeparam>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="callback">给定的读取器委托。</param>
        /// <returns>回调结果的值。</returns>
        public virtual Task<TValue> ToReaderAsync<TValue>(CancellationToken cancellationToken, ExecuteReaderHandler<TValue> callback)
        {
            return this.ExecuteAsync(ExecuteType.Reader, cancellationToken, (dbCommand, ct) =>
            {
                return dbCommand.ExecuteReaderAsync(ct).ContinueWith(t =>
                {
                    using(var reader = t.Result)
                    {
                        return callback(reader);
                    }
                });
            });
        }

        #endregion

        #region Custom Entity

        /// <summary>
        /// 异步执行查询命令，并返回实体。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>实体。</returns>
        public Task<TEntity> ToEntityAsync<TEntity>() => this.ToEntityAsync<TEntity>(CancellationToken.None);
        /// <summary>
        /// 异步执行查询命令，并返回实体。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>实体。</returns>
        public virtual Task<TEntity> ToEntityAsync<TEntity>(CancellationToken cancellationToken)
        {
            return this.ExecuteAsync(ExecuteType.Reader, cancellationToken, DbExtensions.ExecuteEntityAsync<TEntity>);
        }

        /// <summary>
        /// 异步执行查询，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>实体的集合。</returns>
        public Task<List<TEntity>> ToEntitiesAsync<TEntity>() => this.ToEntitiesAsync<TEntity>(CancellationToken.None);
        /// <summary>
        /// 异步执行查询，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>实体的集合。</returns>
        public virtual Task<List<TEntity>> ToEntitiesAsync<TEntity>(CancellationToken cancellationToken)
        {
            return this.ExecuteAsync(ExecuteType.Reader, cancellationToken, DbExtensions.ExecuteEntitiesAsync<TEntity>);
        }

        /// <summary>
        /// 执行分页查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>包含总记录数的实体的集合。</returns>
        public Task<PageData<TEntity>> ToEntitiesAsync<TEntity>(int pageNumber, int pageSize = 10) => this.ToEntitiesAsync<TEntity>(CancellationToken.None, pageNumber, pageSize);

        /// <summary>
        /// 执行分页查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>包含总记录数的实体的集合。</returns>
        public virtual Task<PageData<TEntity>> ToEntitiesAsync<TEntity>(CancellationToken cancellationToken, int pageNumber, int pageSize = 10)
        {
            if(pageNumber < 1) pageNumber = 1;
            if(pageSize < 1) pageSize = 1;

            return this.ExecuteAsync(ExecuteType.Reader
                , cancellationToken
                , (dbCommand, ct) => Task.Run(() => this.InnerToEntities<TEntity>(dbCommand, pageNumber, pageSize), ct));
        }

        #endregion

        #region Dynamic Entity

        /// <summary>
        /// 异步执行查询命令，并返回匿名实体。
        /// </summary>
        /// <returns>实体。</returns>
        public Task<dynamic> ToEntityAsync() => this.ToEntityAsync(CancellationToken.None);
        /// <summary>
        /// 异步执行查询命令，并返回匿名实体。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>实体。</returns>
        public virtual Task<dynamic> ToEntityAsync(CancellationToken cancellationToken)
        {
            return this.ExecuteAsync(ExecuteType.Reader, cancellationToken, DbExtensions.ExecuteEntityAsync);
        }

        /// <summary>
        /// 异步执行查询，并返回匿名实体的集合。
        /// </summary>
        /// <returns>实体的集合。</returns>
        public Task<List<dynamic>> ToEntitiesAsync() => this.ToEntitiesAsync(CancellationToken.None);
        /// <summary>
        /// 异步执行查询，并返回匿名实体的集合。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>实体的集合。</returns>
        public virtual Task<List<dynamic>> ToEntitiesAsync(CancellationToken cancellationToken)
        {
            return this.ExecuteAsync(ExecuteType.Reader, cancellationToken, DbExtensions.ExecuteEntitiesAsync);
        }

        /// <summary>
        /// 执行分页查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>包含总记录数的实体的集合。</returns>
        public Task<PageData<dynamic>> ToEntitiesAsync(int pageNumber, int pageSize = 10) => this.ToEntitiesAsync(CancellationToken.None, pageNumber, pageSize);
        /// <summary>
        /// 执行分页查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>包含总记录数的实体的集合。</returns>
        public virtual Task<PageData<dynamic>> ToEntitiesAsync(CancellationToken cancellationToken, int pageNumber, int pageSize = 10)
        {
            if(pageNumber < 1) pageNumber = 1;
            if(pageSize < 1) pageSize = 1;

            return this.ExecuteAsync(ExecuteType.Reader
                , cancellationToken
                , (dbCommand, ct) => Task.Run(() => this.InnerToEntities(dbCommand, pageNumber, pageSize), ct));
        }

        #endregion
    }
}
#endif