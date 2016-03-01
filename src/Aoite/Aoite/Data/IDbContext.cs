using System;
using System.Data;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个数据源查询与交互引擎的上下文对象，一个对象创建一个数据源连接。
    /// </summary>
    public interface IDbContext : IObjectDisposable, IDbEngine
    {
        /// <summary>
        /// 获取当前上下文的唯一标识符。
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 获取当前上下文的数据库连接对象。
        /// </summary>
        IDbConnection Connection { get; }
        /// <summary>
        /// 获取当前上下文的数据库连接的事务对象。
        /// </summary>
        IDbTransaction Transaction { get; }
        /// <summary>
        /// 打开连接。在执行查询时，若数据源尚未打开则自动打开数据源。
        /// </summary>
        IDbContext Open();
        /// <summary>
        /// 启动数据源事务，并打开数据源连接。
        /// </summary>
        IDbContext OpenTransaction();
        /// <summary>
        /// 指定事务的隔离级别，并打开数据源连接（如果没有打开）。
        /// </summary>
        /// <param name="isolationLevel">指定事务的隔离级别。</param>
        IDbContext OpenTransaction(IsolationLevel isolationLevel);
        /// <summary>
        /// 提交数据源事务。
        /// </summary>
        void Commit();
        /// <summary>
        /// 从挂起状态回滚事务。
        /// </summary>
        void Rollback();
        /// <summary>
        /// 关闭并释放数据源连接。
        /// </summary>
        void Close();
    }

}
