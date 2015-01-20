using Aoite.Net;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示一个 Redis 客户端。
    /// </summary>
    public class RedisClient : ObjectDisposableBase, IRedisClient
    {
        internal RedisExecutor _executor;
        internal IConnector _connector;
        internal LinkedList<RedisCommand> _tranCommands;
        private string _password;

        /// <summary>
        /// 指定套接字的信息，初始化一个 <see cref="Aoite.Redis.RedisClient"/> 类的新实例。
        /// </summary>
        /// <param name="socketInfo">套接字的信息。</param>
        /// <param name="password">Redis 服务器授权密码。</param>
        public RedisClient(SocketInfo socketInfo, string password = null)
            : this(new DefaultConnector(socketInfo), password) { }

        internal RedisClient(IConnector connector, string password = null)
        {
            this._connector = connector;
            this._password = password;
            if(password != null) this._connector.Connected += _connector_Connected;
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
        /// <returns>返回执行后的返回值。</returns>
        public virtual T Execute<T>(RedisCommand<T> command)
        {
            if(command == null) throw new ArgumentNullException("command");
            this.ThrowWhenDisposed();
            if(this._tranCommands != null) throw new RedisException("Redis 事务期间，禁止通过非事务方式调用。");

            return this._executor.Execute(command);
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public override void Dispose()
        {
            this._connector.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// 开始一个新的事务。
        /// <para>在事务期间请勿通过 <see cref="Aoite.Redis.IRedisClient"/> 执行任何命令。</para>
        /// </summary>
        /// <returns>如果事务已存在，将会抛出一个错误，否则返回一个新的事务。</returns>
        public virtual IRedisTransaction BeginTransaction()
        {
            this.ThrowWhenDisposed();
            if(this._tranCommands != null) throw new RedisException("Redis 不支持嵌套事务。");
            this.Execute(new RedisStatus("MULTI")).ThrowIfFailded();
            return new RedisTransaction(this);
        }

    }
}
