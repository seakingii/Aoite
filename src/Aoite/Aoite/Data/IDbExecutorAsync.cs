using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个数据源查询与交互的异步执行器。
    /// </summary>
    public interface IDbExecutorAsync
    {
        /// <summary>
        /// 异步执行查询命令，并返回受影响的行数。
        /// </summary>
        /// <returns>受影响的行数。</returns>
        Task<int> ToNonQueryAsync();
        /// <summary>
        /// 异步执行查询命令，并返回受影响的行数。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>受影响的行数。</returns>
        Task<int> ToNonQueryAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 异步执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <returns>结果集中第一行的第一列。</returns>
        Task<object> ToScalarAsync();
        /// <summary>
        /// 异步执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>结果集中第一行的第一列。</returns>
        Task<object> ToScalarAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 指定值的数据类型，异步执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <returns>具备强类型的结果集中第一行的第一列。</returns>
        Task<TValue> ToScalarAsync<TValue>();
        /// <summary>
        /// 指定值的数据类型，异步执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <returns>具备强类型的结果集中第一行的第一列。</returns>
        Task<TValue> ToScalarAsync<TValue>(CancellationToken cancellationToken);

        /// <summary>
        /// 异步执行查询命令，并返回数据集。
        /// </summary>
        /// <returns>一个数据集。</returns>
        Task<DataSet> ToDataSetAsync();
        /// <summary>
        /// 异步执行查询命令，并返回数据集。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>一个数据集。</returns>
        Task<DataSet> ToDataSetAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 异步执行查询命令，并返回自定义的数据集。
        /// </summary>
        /// <typeparam name="TDataSet">自定义的数据集类型。</typeparam>
        /// <returns>具备强类型的一个数据集。</returns>
        Task<TDataSet> ToDataSetAsync<TDataSet>() where TDataSet : DataSet, new();
        /// <summary>
        /// 异步执行查询命令，并返回自定义的数据集。
        /// </summary>
        /// <typeparam name="TDataSet">自定义的数据集类型。</typeparam>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>具备强类型的一个数据集。</returns>
        Task<TDataSet> ToDataSetAsync<TDataSet>(CancellationToken cancellationToken) where TDataSet : DataSet, new();

        /// <summary>
        /// 异步执行查询命令，并返回表。
        /// </summary>
        /// <returns>一张表。</returns>
        Task<PageTable> ToTableAsync();
        /// <summary>
        /// 异步执行查询命令，并返回表。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>一张表。</returns>
        Task<PageTable> ToTableAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 异步执行分页查询命令，并返回表。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一张包含总记录数的表。</returns>
        Task<PageTable> ToTableAsync(int pageNumber, int pageSize = 10);
        /// <summary>
        /// 异步执行分页查询命令，并返回表。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一张包含总记录数的表。</returns>
        Task<PageTable> ToTableAsync(CancellationToken cancellationToken, int pageNumber, int pageSize = 10);

        /// <summary>
        /// 异步执行查询命令，并执行给定的读取器的回调函数。
        /// </summary>
        /// <param name="callback">给定的读取器委托。</param>
        Task ToReaderAsync(ExecuteReaderHandler callback);
        /// <summary>
        /// 异步执行查询命令，并执行给定的读取器的回调函数。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="callback">给定的读取器委托。</param>
        Task ToReaderAsync(CancellationToken cancellationToken, ExecuteReaderHandler callback);

        /// <summary>
        /// 执行查询命令，并执行给定的读取器的回调函数。
        /// </summary>
        /// <typeparam name="TValue">返回值的数据类型。</typeparam>
        /// <param name="callback">给定的读取器委托。</param>
        /// <returns>回调结果的值。</returns>
        Task<TValue> ToReaderAsync<TValue>(ExecuteReaderHandler<TValue> callback);
        /// <summary>
        /// 执行查询命令，并执行给定的读取器的回调函数。
        /// </summary>
        /// <typeparam name="TValue">返回值的数据类型。</typeparam>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="callback">给定的读取器委托。</param>
        /// <returns>回调结果的值。</returns>
        Task<TValue> ToReaderAsync<TValue>(CancellationToken cancellationToken, ExecuteReaderHandler<TValue> callback);

        /// <summary>
        /// 异步执行查询命令，并返回实体。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>一个实体。</returns>
        Task<TEntity> ToEntityAsync<TEntity>();
        /// <summary>
        /// 异步执行查询命令，并返回实体。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>一个实体。</returns>
        Task<TEntity> ToEntityAsync<TEntity>(CancellationToken cancellationToken);

        /// <summary>
        /// 异步执行查询，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>一个实体。</returns>
        Task<List<TEntity>> ToEntitiesAsync<TEntity>();

        /// <summary>
        /// 异步执行查询，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>一个实体。</returns>
        Task<List<TEntity>> ToEntitiesAsync<TEntity>(CancellationToken cancellationToken);

        /// <summary>
        /// 执行分页查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一个包含总记录数的实体集合。</returns>
        Task<PageData<TEntity>> ToEntitiesAsync<TEntity>(int pageNumber, int pageSize = 10);

        /// <summary>
        /// 执行分页查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一个包含总记录数的实体集合。</returns>
        Task<PageData<TEntity>> ToEntitiesAsync<TEntity>(CancellationToken cancellationToken, int pageNumber, int pageSize = 10);
        /// <summary>
        /// 异步执行查询命令，并返回匿名实体。
        /// </summary>
        /// <returns>一个实体。</returns>
        Task<dynamic> ToEntityAsync();
        /// <summary>
        /// 异步执行查询命令，并返回匿名实体。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>一个实体。</returns>
        Task<dynamic> ToEntityAsync(CancellationToken cancellationToken);
        /// <summary>
        /// 异步执行查询，并返回匿名实体的集合。
        /// </summary>
        /// <returns>一个实体。</returns>
        Task<List<dynamic>> ToEntitiesAsync();
        /// <summary>
        /// 异步执行查询，并返回匿名实体的集合。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>一个实体。</returns>
        Task<List<dynamic>> ToEntitiesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 执行分页查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一个包含总记录数的实体集合。</returns>
        Task<PageData<dynamic>> ToEntitiesAsync(int pageNumber, int pageSize = 10);
        /// <summary>
        /// 执行分页查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一个包含总记录数的实体集合。</returns>
        Task<PageData<dynamic>> ToEntitiesAsync(CancellationToken cancellationToken, int pageNumber, int pageSize = 10);
    }
}
