using System;
using System.Threading;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个提供锁功能的提供程序。
    /// </summary>
    [SingletonMapping]
    public class LockProvider : CommandModelContainerProviderBase, ILockProvider
    {
        /// <summary>
        /// 初始化一个 <see cref="LockProvider"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public LockProvider(IIocContainer container) : base(container) { }

        /// <summary>
        /// 提供锁的功能。
        /// </summary>
        /// <param name="key">锁的键名。</param>
        /// <param name="timeout">锁的超时设定。</param>
        /// <returns>可释放的锁实例。</returns>
        public virtual IDisposable Lock(string key, TimeSpan? timeout = null)
        {
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            var redisProvider = this.Container.GetFixed<IRedisProvider>();
            if(redisProvider != null)
            {
                var client = this.Container.Get<IRedisProvider>().GetRedisClient();
                return client.Lock(key, timeout);
            }

            key = "$LockProvider::" + key;
            var realTimeout = timeout ?? RedisExtensions.DefaultLockTimeout;
            return GA.Lock(key, realTimeout);
        }

#if !NET40
        /// <summary>
        /// 提供异步锁的功能。
        /// </summary>
        /// <param name="key">锁的键名。</param>
        /// <param name="timeout">锁的超时设定。</param>
        /// <returns>可释放的异步锁操作。</returns>
        public System.Threading.Tasks.Task<IDisposable> LockAsync(string key, TimeSpan? timeout = null)
        {
            var redisProvider = this.Container.GetFixed<IRedisProvider>();
            if(redisProvider != null)
            {
                return System.Threading.Tasks.Task.Run(() =>
                {
                    var client = this.Container.Get<IRedisProvider>().GetRedisClient();
                    return client.Lock(key, timeout);
                });
            }
            key = "$LockProvider::" + key;
            var realTimeout = timeout ?? RedisExtensions.DefaultLockTimeout;
            return GA.LockAsync(key, realTimeout);
        }
#endif
    }
}
