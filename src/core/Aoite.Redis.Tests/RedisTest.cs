using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    public abstract class RedisTest : IDisposable
    {
        protected virtual string Host { get { return "127.0.0.1:6379"; } }
        protected readonly IRedisClient redis;

        private readonly static System.Threading.CountdownEvent cde = new System.Threading.CountdownEvent(1);
        protected RedisTest()
        {
            cde.AddCount();
            this.redis = new RedisClient(6379);
            this.redis.FlushDb();
        }


        public virtual void Dispose()
        {
            this.redis.Dispose();
            cde.Signal();
        }
    }
}
