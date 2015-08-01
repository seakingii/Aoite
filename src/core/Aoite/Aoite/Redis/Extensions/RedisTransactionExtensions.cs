using Aoite.Redis;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示 Redis 的 Transaction 扩展方法。
    /// </summary>
    public static class RedisTransactionExtensions
    {
        /// <summary>
        /// 监视一个(或多个)键 ，如果在事务执行之前这个(或这些)键被其他命令所改动，那么事务将被打断。。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="keys">键的数组。</param>
        /// <returns>返回一个结果。</returns>
        public static Result Watch(this IRedisClient client, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(keys == null || keys.Length == 0) throw new ArgumentNullException("keys");

            return client.Execute(new RedisStatus("WATCH", keys));
        }

        /// <summary>
        /// 取消 WATCH 命令对所有键的监视。
        /// <para>如果在执行 WATCH 命令之后， EXEC 命令或 DISCARD 命令先被执行了的话，那么就不需要再执行 UNWATCH 了。</para>
        /// <para>因为 EXEC 命令会执行事务，因此 WATCH 命令的效果已经产生了；而 DISCARD 命令在取消事务的同时也会取消所有对 key 的监视，因此这两个命令执行之后，就没有必要执行 UNWATCH 了。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回一个结果。</returns>
        public static Result Unwatch(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException("client");

            return client.Execute(new RedisStatus("UNWATCH"));
        }
    }
}
