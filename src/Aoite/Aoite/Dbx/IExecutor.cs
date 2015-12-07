using System;
using System.Collections.Generic;
using System.Data;

namespace Aoite.Dbx
{
    /// <summary>
    /// 定义一个数据源查询与交互的执行器。
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        /// 获取数据源查询与交互引擎的实例。
        /// </summary>
        IEngine Engine { get; }
        /// <summary>
        /// 获取执行的命令。
        /// </summary>
        ExecuteCommand Command { get; }
        /// <summary>
        /// 执行查询命令，并返回数据集。
        /// </summary>
        /// <returns>一个数据集。</returns>
        DataSet ToDataSet();
        /// <summary>
        /// 执行查询命令，并返回自定义的数据集。
        /// </summary>
        /// <typeparam name="TDataSet">自定义的数据集类型。</typeparam>
        /// <returns>具备强类型的一个数据集。</returns>
        TDataSet ToDataSet<TDataSet>() where TDataSet : DataSet, new();
        /// <summary>
        /// 执行查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <returns>一个匿名实体的集合。</returns>
        List<dynamic> ToEntities();
        /// <summary>
        /// 执行分页查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一个包含总记录数的匿名实体集合。</returns>
        PageData<dynamic> ToEntities(IPagination page);
        /// <summary>
        /// 执行分页查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一个包含总记录数的匿名实体集合。</returns>
        PageData<dynamic> ToEntities(int pageNumber, int pageSize = 10);
        /// <summary>
        /// 执行查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>一个实体的集合。</returns>
        List<TEntity> ToEntities<TEntity>();
        /// <summary>
        /// 执行分页查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一个包含总记录数的实体集合。</returns>
        PageData<TEntity> ToEntities<TEntity>(IPagination page);
        /// <summary>
        /// 执行分页查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一个包含总记录数的实体集合。</returns>
        PageData<TEntity> ToEntities<TEntity>(int pageNumber, int pageSize = 10); 
        /// <summary>
        /// 执行查询命令，并返回匿名实体。
        /// </summary>
        /// <returns>一个匿名实体。</returns>
        dynamic ToEntity();
        /// <summary>
        /// 执行查询命令，并返回实体。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>一个实体。</returns>
        TEntity ToEntity<TEntity>();
        /// <summary>
        /// 执行查询命令，并返回受影响的行数。
        /// </summary>
        /// <returns>受影响的行数。</returns>
        int ToNonQuery();
        /// <summary>
        /// 执行查询命令，并构建一个读取器结果。
        /// </summary>
        /// <param name="callback">给定的读取器委托。</param>
        void ToReader(ExecuteReaderHandler callback);
        /// <summary>
        /// 执行查询命令，并构建一个读取器结果。
        /// </summary>
        /// <typeparam name="TValue">返回值的数据类型。</typeparam>
        /// <param name="callback">给定的读取器委托。</param>
        /// <returns>回调结果的值。</returns>
        TValue ToReader<TValue>(ExecuteReaderHandler<TValue> callback);
        /// <summary>
        /// 执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <returns>结果集中第一行的第一列。</returns>
        object ToScalar();
        /// <summary>
        /// 指定值的数据类型，执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <returns>具备强类型的结果集中第一行的第一列。</returns>
        TValue ToScalar<TValue>();
        /// <summary>
        /// 执行查询命令，并返回表。
        /// </summary>
        /// <returns>一张表。</returns>
        PageTable ToTable();
        /// <summary>
        /// 执行分页查询命令，并返回表。
        /// </summary>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一张包含总记录数的表。</returns>
        PageTable ToTable(IPagination page);
        /// <summary>
        /// 执行分页查询命令，并返回表。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一张包含总记录数的表。</returns>
        PageTable ToTable(int pageNumber, int pageSize = 10);
    }
}