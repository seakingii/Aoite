using Aoite.Net;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示一个 Redis 客户端。
    /// </summary>
    public class RedisClient : ObjectDisposableBase, IRedisClient
    {
        static long GLOBAL_ID = 0;

        internal RedisExecutor _executor;
        internal IConnector _connector;
        internal LinkedList<RedisCommand> _tranCommands;
        private string _password;
        private long _Id = System.Threading.Interlocked.Increment(ref GLOBAL_ID);
        /// <summary>
        /// 获取当前 Redis 的在应用程序域中的唯一编号。
        /// </summary>
        public long Id { get { return _Id; } }

        /// <summary>
        /// 初始化一个 <see cref="RedisClient"/> 类的新实例。
        /// </summary>
        /// <param name="address">Redis 的连接地址。</param>
        /// <param name="password">Redis 的连接密码。</param>
        public RedisClient(SocketInfo address, string password = null)
            : this(new DefaultConnector(address), password) { }

        internal RedisClient(IConnector connector, string password = null)
        {
            this._connector = connector;
            this._password = password;
            if(!string.IsNullOrWhiteSpace(password)) this._connector.Connected += _connector_Connected;
            this._executor = new RedisExecutor(connector);
        }

        void _connector_Connected(object sender, EventArgs e)
        {
            this.Execute(new RedisStatus("AUTH", this._password)).ThrowIfFailded();
        }

        /// <summary>
        /// 执行指定的 Redis 命令。
        /// </summary>
        /// <typeparam name="T">命令返回值的数据类型。</typeparam>
        /// <param name="command">Redis 命令。</param>
        /// <returns>执行后的返回值。</returns>
        public virtual T Execute<T>(RedisCommand<T> command)
        {
            if(command == null) throw new ArgumentNullException(nameof(command));
            this.ThrowIfDisposed();
            if(this._tranCommands != null) throw new RedisException("Redis 事务期间，禁止通过非事务方式调用。");

            return this._executor.Execute(command);
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            base.DisposeManaged();
            this._connector.Dispose();
        }

        /// <summary>
        /// 开始一个新的事务。
        /// <para>在事务期间请勿通过 <see cref="IRedisClient"/> 执行任何命令。</para>
        /// </summary>
        /// <returns>如果事务已存在，将会抛出一个错误，否则返回一个新的事务。</returns>
        public virtual IRedisTransaction BeginTransaction()
        {
            this.ThrowIfDisposed();
            if(this._tranCommands != null) throw new RedisException("Redis 不支持嵌套事务。");
            this.Execute(new RedisStatus("MULTI")).ThrowIfFailded();
            return new RedisTransaction(this);
        }
    }
}
