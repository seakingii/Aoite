using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;

namespace Aoite.Redis
{
    class RedisTransaction : ObjectDisposableBase, IRedisTransaction
    {
        private readonly RedisClient _client;

        public long Id { get { return _client.Id; } }

        public RedisTransaction(RedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            this._client = client;
            this._client._tranCommands = new LinkedList<RedisCommand>();
        }

        protected override void ThrowIfDisposed()
        {
            base.ThrowIfDisposed();
            if(this._client._tranCommands == null) throw new RedisException("Redis 的事务已结束。");
        }

        public T Execute<T>(RedisCommand<T> command)
        {
            if(command == null) throw new ArgumentNullException(nameof(command));

            this.ThrowIfDisposed();
            this._client._tranCommands.AddLast(command);
            this._client._executor.Execute(new RedisTran.Queue(command)).ThrowIfFailded();
            return default(T);
        }

        IRedisTransaction IRedisClient.BeginTransaction()
        {
            throw new NotImplementedException();
        }

        void IRedisTransaction.On<T>(T executor, Action<T> callback)
        {
            this.ThrowIfDisposed();
            //executor();
            this._client._tranCommands.Last.Value.SetCallback(callback);
        }

        private bool _IsCommited;
        void IRedisTransaction.Commit()
        {
            this.ThrowIfDisposed();
            if(this._IsCommited) return;

            this._client._executor.Execute(new RedisTran.Exec(this._client._tranCommands)).ThrowIfFailded();
            this._IsCommited = true;
        }

        protected override void DisposeManaged()
        {
            if(!this._IsCommited)
            {
                try
                {
                    var r = this._client._executor.Execute(new RedisStatus("DISCARD"));
                    Console.WriteLine(r.ToString());
                }
                catch(Exception)
                {
                    // Redis client is disposed?
                }
            }
            this._client._tranCommands = null;
            base.DisposeManaged();
        }

    }
}
