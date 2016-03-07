using System;
using System.Data;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源查询与交互引擎的上下文对象，一个对象创建一个数据源连接。
    /// </summary>
    class DbContext : ObjectDisposableBase, IDbContext
    {
        Lazy<DbConnection> _lazyConnection;
        DbTransaction _transaction;

        /// <summary>
        /// 获取当前上下文所属的交互引擎。
        /// </summary>
        public DbEngine Owner { get; }

        /// <summary>
        /// 获取当前上下文的唯一标识符。
        /// </summary>
        public Guid Id { get; }

        IDbConnection IDbContext.Connection => this._lazyConnection.Value;
        IDbTransaction IDbContext.Transaction => this._transaction;

        /// <summary>
        /// 获取一个值，该值指示当前上下文的连接是否已关闭。
        /// </summary>
        public bool IsClosed => this._lazyConnection.Value.State == ConnectionState.Closed;

        IDbEngineProvider IDbEngine.Provider => this.Owner.Provider;

        public IDbContext Context => this;

        public IDbContext ContextTransaction => this.OpenTransaction();

        internal DbContext(DbEngine owner)
        {
            this.Id = Guid.NewGuid();
            this.Owner = owner;
            this._lazyConnection = new Lazy<DbConnection>(owner.Provider.CreateConnection);
        }

        /// <summary>
        /// 打开连接。在执行查询时，若数据源尚未打开则自动打开数据源。
        /// </summary>
        public IDbContext Open()
        {
            this.ThrowIfDisposed();
            if(this._lazyConnection.Value.State == ConnectionState.Closed) this._lazyConnection.Value.Open();
            return this;
        }

        /// <summary>
        /// 启动数据源事务，并打开数据源连接。
        /// </summary>
        public IDbContext OpenTransaction() => this.OpenTransaction(IsolationLevel.Unspecified);

        /// <summary>
        /// 指定事务的隔离级别，并打开数据源连接（如果没有打开）。
        /// </summary>
        /// <param name="isolationLevel">指定事务的隔离级别。</param>
        public IDbContext OpenTransaction(IsolationLevel isolationLevel)
        {
            if(this._transaction == null)
            {
                this.Open();
                //if(this._transaction != null) throw new NotSupportedException("已打开了一个新的事务，无法确定对旧的事务是否已执行提交或回滚操作。");
                this._transaction = this._lazyConnection.Value.BeginTransaction(isolationLevel);
            }
            return this;
        }

        /// <summary>
        /// 执行指定的命令。
        /// </summary>
        /// <param name="command">执行的命令。</param>
        /// <returns>数据源查询与交互的执行器。</returns>
        public IDbExecutor Execute(ExecuteCommand command)
        {
            this.ThrowIfDisposed();
            if(command == null) throw new ArgumentNullException(nameof(command));
            return new DbExecutor(this, command, this._lazyConnection.Value, this._transaction, false);
        }

#if !NET45 && !NET40

        /// <summary>
        /// 执行指定的命令。
        /// </summary>
        /// <param name="fs">一个复合格式字符串</param>
        /// <returns>数据源查询与交互的执行器。</returns>
        public IDbExecutor Execute(FormattableString fs)
        {
            this.ThrowIfDisposed();
            return this.Execute(this.Owner.Parse(fs));
        }

#endif

        private void TransactionHandler(Action action)
        {
            this.ThrowIfDisposed();

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
            base.DisposeManaged();
            if(this._lazyConnection != null)
            {
                lock (this)
                {
                    var conn = this._lazyConnection.Value;
                    if(conn != null)
                    {
                        this._lazyConnection = null;
                        if(conn.State != ConnectionState.Closed)
                            try
                            {
                                using(conn)
                                {
                                    if(this._transaction != null) this._transaction.Dispose();
                                    conn.TryClose();
                                }
                            }
                            catch(Exception) { }

                        this._transaction = null;
                        this.Owner.ResetContext();
                    }
                }
            }
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
        }
    }
}
