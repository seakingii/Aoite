using Aoite.Redis;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示 Redis 的 Connection 扩展方法。
    /// </summary>
    public static class RedisConnectionExtensions
    {
        /// <summary>
        /// 使用客户端向 Redis 服务器发送一个 PING。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回一个结果。</returns>
        public static Result Ping(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException("client");
            return client.Execute(new RedisStatus.Pong("PING"));
        }

        /// <summary>
        /// 请求服务器关闭与当前客户端的连接。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回一个结果。</returns>
        public static Result Quit(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException("client");
            return client.Execute(new RedisStatus("QUIT"));
        }

        /// <summary>
        /// 切换到指定的数据库，数据库索引号 <paramref name="index"/> 用数字值指定。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="index">以 0 作为起始索引值。</param>
        /// <returns>返回一个结果。</returns>
        public static Result Select(this IRedisClient client, int index)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(index < 0) throw new ArgumentOutOfRangeException("index");
            return client.Execute(new RedisStatus("SELECT", index));
        }
    }
}
