using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Dbx
{
    /// <summary>
    /// 定义数据源查询与交互引擎的方法。
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// 获取数据源查询与交互引擎的提供程序。
        /// </summary>
        IEngineProvider Provider { get; }
        /// <summary>
        /// 获取当前上下文所属的交互引擎。
        /// </summary>
        Engine Owner { get; }
        /// <summary>
        /// 执行指定的命令。
        /// </summary>
        /// <param name="command">执行的命令。</param>
        /// <returns>数据源查询与交互的执行器。</returns>
        IExecutor Execute(ExecuteCommand command);
    }

    /// <summary>
    /// 表示一个数据源查询与交互引擎。
    /// </summary>
    public class Engine : IEngine
    {
        /// <summary>
        /// 获取数据源查询与交互引擎的提供程序。
        /// </summary>
        public IEngineProvider Provider { get; }

        /// <summary>
        /// 获取当前上下文所属的交互引擎。
        /// </summary>
        public Engine Owner { get { return this; } }

        /// <summary>
        /// 给定数据源查询与交互引擎的提供程序，初始化一个 <see cref="Engine"/> 类的新实例。
        /// </summary>
        /// <param name="provider">数据源查询与交互引擎的提供程序。</param>
        public Engine(IEngineProvider provider)
        {
            if(provider == null) throw new ArgumentNullException(nameof(provider));

            this.Provider = provider;
        }

        /// <summary>
        /// 执行指定的命令。
        /// </summary>
        /// <param name="command">执行的命令。</param>
        /// <returns>数据源查询与交互的执行器。</returns>
        public IExecutor Execute(ExecuteCommand command)
        {
            if(command == null) throw new ArgumentNullException(nameof(command));
            return new Executor(this, command, null, null, true);
        }

        private readonly System.Threading.ThreadLocal<Context> _threadLocalContent = new System.Threading.ThreadLocal<Context>();
        /// <summary>
        /// 释放并关闭当前线程上下文的 <see cref="IContext"/>。
        /// </summary>
        public void ResetContext()
        {
            var context = this._threadLocalContent.Value;

            if(context != null)
            {
                context.Dispose();
                this._threadLocalContent.Value = null;
            }
        }

        /// <summary>
        /// 获取一个值，指示当前上下文在线程中是否已创建。
        /// </summary>
        public bool IsThreadContext { get { return this._threadLocalContent.Value != null; } }

        /// <summary>
        /// 创建并返回一个 <see cref="IContext"/>。返回当前线程上下文包含的 <see cref="IContext"/> 或创建一个新的  <see cref="IContext"/>。
        /// <para>当释放一个 <see cref="IContext"/> 后，下一次调用获取将会重新创建上下文。</para>
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public virtual IContext Context
        {
            get
            {
                if(this._threadLocalContent.Value == null) this._threadLocalContent.Value = new Context(this);
                return this._threadLocalContent.Value;
            }
        }

        /// <summary>
        /// 创建并返回一个事务性 <see cref="IContext"/>。返回当前线程上下文包含的 <see cref="IContext"/> 或创建一个新的  <see cref="IContext"/>。
        /// <para>当释放一个 <see cref="IContext"/> 后，下一次调用获取将会重新创建上下文。</para>
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public virtual IContext ContextTransaction { get { return this.Context.OpenTransaction(); } }

        /// <summary>
        /// 在引擎执行命令时发生。
        /// </summary>
        public event ExecutingEventHandler Executing;
        /// <summary>
        /// 在引擎执行命令后发生。
        /// </summary>
        public event ExecutedEventHandler Executed;

        /// <summary>
        /// 表示 <see cref="Executing"/> 事件的处理方法。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="type">执行的类型。</param>
        /// <param name="command">执行的命令。</param>
        internal protected virtual void OnExecuting(IEngine engine, ExecuteType type, ExecuteCommand command)
            => this.Executing?.Invoke(engine, command.GetEventArgs(type, null));

        /// <summary>
        /// 表示 <see cref="Executed"/> 事件的处理方法。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="type">执行的类型。</param>
        /// <param name="result">操作的返回值。</param>
        /// <param name="command">执行的命令。</param>
        internal protected virtual void OnExecuted(IEngine engine, ExecuteType type, ExecuteCommand command, object result)
            => this.Executed?.Invoke(engine, command.GetEventArgs(type, result));

    }

    /// <summary>
    /// 定义一个数据源查询与交互引擎的上下文对象，一个对象创建一个数据源连接。
    /// </summary>
    public interface IContext : IObjectDisposable, IEngine
    {
        /// <summary>
        /// 打开连接。在执行查询时，若数据源尚未打开则自动打开数据源。
        /// </summary>
        IContext Open();
        /// <summary>
        /// 启动数据源事务，并打开数据源连接。
        /// </summary>
        IContext OpenTransaction();
        /// <summary>
        /// 指定事务的隔离级别，并打开数据源连接（如果没有打开）。
        /// </summary>
        /// <param name="isolationLevel">指定事务的隔离级别。。</param>
        IContext OpenTransaction(IsolationLevel isolationLevel);
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

    /// <summary>
    /// 表示一个数据源查询与交互引擎的上下文对象，一个对象创建一个数据源连接。
    /// </summary>
    class Context : ObjectDisposableBase, IContext
    {
        DbConnection _connection;
        DbTransaction _transaction;

        /// <summary>
        /// 获取当前上下文所属的交互引擎。
        /// </summary>
        public Engine Owner { get; }

        /// <summary>
        /// 获取一个值，该值指示当前上下文的连接是否已关闭。
        /// </summary>
        public bool IsClosed { get { return this._connection.State == ConnectionState.Closed; } }

        IEngineProvider IEngine.Provider { get { return this.Owner.Provider; } }

        internal Context(Engine owner)
        {
            this.Owner = owner;
            this._connection = owner.Provider.CreateConnection();
        }

        /// <summary>
        /// 打开连接。在执行查询时，若数据源尚未打开则自动打开数据源。
        /// </summary>
        public IContext Open()
        {
            if(this._connection.State == ConnectionState.Closed) this._connection.Open();
            return this;
        }

        /// <summary>
        /// 启动数据源事务，并打开数据源连接。
        /// </summary>
        public IContext OpenTransaction() => this.OpenTransaction(IsolationLevel.Unspecified);

        /// <summary>
        /// 指定事务的隔离级别，并打开数据源连接（如果没有打开）。
        /// </summary>
        /// <param name="isolationLevel">指定事务的隔离级别。。</param>
        public IContext OpenTransaction(IsolationLevel isolationLevel)
        {
            this.Open();
            if(this._transaction != null) throw new NotSupportedException("已打开了一个新的事务，无法确定对旧的事务是否已执行提交或回滚操作。");
            this._transaction = this._connection.BeginTransaction(isolationLevel);
            return this;
        }

        /// <summary>
        /// 执行指定的命令。
        /// </summary>
        /// <param name="command">执行的命令。</param>
        /// <returns>数据源查询与交互的执行器。</returns>
        public IExecutor Execute(ExecuteCommand command)
        {
            if(command == null) throw new ArgumentNullException(nameof(command));
            return new Executor(this, command, this._connection, this._transaction, false);
        }

        private void TransactionHandler(Action action)
        {
            try
            {
                action();
            }
            finally
            {
                this._transaction.Dispose();
                this._transaction = null;
            }
        }

        /// <summary>
        /// 提交数据源事务。
        /// </summary>
        public void Commit()
        {
            if(this._transaction == null) throw new NotSupportedException("并非以事务的方式打开连接。");
            TransactionHandler(this._transaction.Commit);
        }

        /// <summary>
        /// 从挂起状态回滚事务。
        /// </summary>
        public void Rollback()
        {
            if(this._transaction == null) throw new NotSupportedException("并非以事务的方式打开连接。");
            TransactionHandler(this._transaction.Rollback);
        }

        /// <summary>
        /// 关闭并释放数据源连接。
        /// </summary>
        public void Close()
        {
            this.Dispose();
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            if(this._connection != null)
            {
                lock (this)
                {
                    var conn = this._connection;
                    if(conn != null)
                    {
                        this._connection = null;
                        if(conn.State != ConnectionState.Closed)
                            try
                            {
                                using(conn)
                                {
                                    if(this._transaction != null) this._transaction.Dispose();
                                    conn.TryClose();
                                }
                            }
                            catch(Exception)
                            {

                                throw;
                            }

                        this._transaction = null;
                        this.Owner.ResetContext();
                    }
                }
            }
        }
    }
}
