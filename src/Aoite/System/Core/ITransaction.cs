using System;

namespace System
{
    /// <summary>
    /// 定义一个事务。
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// 提交事务。
        /// </summary>
        void Commit();
    }
}

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个默认的事务。
    /// </summary>
    public sealed class DefaultTransaction : ITransaction
    {
        private System.Transactions.TransactionScope _t;

        /// <summary>
        /// 初始化一个 <see cref="DefaultTransaction"/> 类的新实例。
        /// </summary>
        /// <param name="asyncEnabled">描述了当使用 Task 或 async/await .NET 异步编程模式时，与事务范围关联的环境事务将跨线程连续任务执行。</param>
        public DefaultTransaction(bool asyncEnabled)
        {
            var op = asyncEnabled ? System.Transactions.TransactionScopeAsyncFlowOption.Enabled : System.Transactions.TransactionScopeAsyncFlowOption.Suppress;
            _t = new System.Transactions.TransactionScope(op);
        }

        void ITransaction.Commit() => _t.Complete();

        void IDisposable.Dispose() => _t.Dispose();
    }
}
