using Aoite.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace System
{
    /// <summary>
    /// 表示 Redis 的扩展方法。
    /// </summary>
    public static class RedisExtensions
    {
        const string LockKey = "$Reids.Locks$";
        /// <summary>
        /// 获取或设置默认的锁超时间隔。
        /// </summary>
        public static TimeSpan DefaultLockTimeout = TimeSpan.FromMinutes(1);

        /// <summary>
        /// 实现一个 Redis 锁的功能。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">锁的键名。</param>
        /// <param name="timeout">锁的超时设定。</param>
        /// <returns>返回一个锁，释放时解除占用的锁。</returns>
        /// <exception cref="System.TimeoutException">获取锁超时。</exception>
        public static IDisposable Lock(this IRedisClient client, string key, TimeSpan? timeout = null)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            var realSpan = timeout ?? DefaultLockTimeout;
            if(!SpinWait.SpinUntil(() => client.HSet(LockKey, key, 1, nx: true), realSpan))
               SimpleLockItem.TimeoutError(key, realSpan);
            return new SimpleLockItem(() => client.HDel(LockKey, key));
        }
    }
}
