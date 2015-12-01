using Aoite.Redis;
using Aoite.Redis.Commands;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// 表示 Redis 的 Server 扩展方法。
    /// </summary>
    public static class RedisServerExtensions
    {
        /// <summary>
        /// 执行一个 AOF文件重写操作。重写会创建一个当前 AOF 文件的体积优化版本。
        /// <para>即使 BGREWRITEAOF 执行失败，也不会有任何数据丢失，因为旧的 AOF 文件在 BGREWRITEAOF 成功之前不会被修改。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回一个结果。</returns>
        public static Result BgRewriteAof(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));

            return client.Execute(new RedisStatus("BGREWRITEAOF"));
        }
        /// <summary>
        /// 在后台异步保存当前数据库的数据到磁盘。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回一个结果。</returns>
        public static Result BgSave(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));

            return client.Execute(new RedisStatus("BGSAVE"));
        }

        /// <summary>
        /// 返回 CLIENT SETNAME 命令为连接设置的名字。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>如果连接没有设置名字，那么返回空白回复；如果有设置名字，那么返回名字。</returns>
        public static string ClientGetName(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));

            return client.Execute(new RedisString("CLIENT GETNAME"));
        }
        /// <summary>
        /// 关闭地址为指定地址和端口的客户端。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="ip">客户端地址。</param>
        /// <param name="port">客户端端口。</param>
        /// <returns>返回一个结果。</returns>
        public static Result ClientKill(this IRedisClient client, string ip, int port)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            if(string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip));

            return client.Execute(new RedisStatus("CLIENT KILL", ip + ":" + port));
        }
        /// <summary>
        /// With the new form it is possible to kill clients by different attributes instead of killing just by address.
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="addr">ip:port. This is exactly the same as the old three-arguments behavior.</param>
        /// <param name="id">client-id. Allows to kill a client by its unique ID field, which was introduced in the CLIENT LIST command starting from Redis 2.8.12.</param>
        /// <param name="type">type, where type is one of normal, slave, pubsub. This closes the connections of all the clients in the specified class. Note that clients blocked into the MONITOR command are considered to belong to the normal class.</param>
        /// <param name="skipMe">By default this option is set to yes, that is, the client calling the command will not get killed, however setting this option to no will have the effect of also killing the client calling the command.</param>
        /// <returns>Returns the number of clients killed.</returns>
        public static long ClientKill(this IRedisClient client, string addr = null, string id = null, string type = null, bool? skipMe = null)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));

            var args = new List<string>();
            if(addr != null)
                args.AddRange(new[] { "ADDR", addr });
            if(id != null)
                args.AddRange(new[] { "ID", id });
            if(type != null)
                args.AddRange(new[] { "TYPE", type });
            if(skipMe != null)
                args.AddRange(new[] { "SKIPME", skipMe.Value ? "yes" : "no" });
            return client.Execute(new RedisInteger("CLIENT KILL", args.ToArray()));
        }
        /// <summary>
        /// 返回 CLIENT SETNAME 命令为连接设置的名字。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>如果连接没有设置名字，那么返回空白回复；如果有设置名字，那么返回名字。</returns>
        public static string ClientList(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));

            return client.Execute(new RedisString("CLIENT LIST"));
        }
        /// <summary>
        /// 为当前连接分配一个名字。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="connectionName">用于识别当前正在与服务器进行连接的客户端名称。
        /// <para>要移除一个连接的名字， 可以将连接的名字设为空字符串 "" 。</para>
        /// </param>
        /// <returns>返回一个结果。</returns>
        public static Result ClientSetName(this IRedisClient client, string connectionName)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            if(string.IsNullOrEmpty(connectionName)) throw new ArgumentNullException(nameof(connectionName));
            if(connectionName.Contains(' ')) connectionName = connectionName.Replace(' ', '_');
            return client.Execute(new RedisStatus("CLIENT SETNAME", connectionName));
        }
        /// <summary>
        /// 取得运行中的 Redis 服务器的配置参数。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="parameter">搜索关键字，查找所有匹配的配置参数。</param>
        /// <returns>返回给定配置参数和值。</returns>
        public static RedisKeyItem[] ConfigGet(this IRedisClient client, string parameter)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            if(string.IsNullOrEmpty(parameter)) throw new ArgumentNullException(nameof(parameter));
            return client.Execute(RedisArray.Create(new RedisItem<RedisKeyItem>(false, "CONFIG GET", parameter), 2));
        }
        /// <summary>
        /// 在后台异步保存当前数据库的数据到磁盘。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回一个结果。</returns>
        public static Result ConfigResetStat(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));

            return client.Execute(new RedisStatus("CONFIG RESETSTAT"));
        }
        /// <summary>
        /// 启动 Redis 服务器时所指定的 redis.conf 文件进行改写。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回一个结果。</returns>
        public static Result ConfigRewrite(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));

            return client.Execute(new RedisStatus("CONFIG REWRITE"));
        }
        /// <summary>
        /// 动态地调整 Redis 服务器的配置。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="parameter">配置参数。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回一个结果。</returns>
        public static Result ConfigSet(this IRedisClient client, string parameter, BinaryValue value)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            if(string.IsNullOrEmpty(parameter)) throw new ArgumentNullException(nameof(parameter));
            return client.Execute(new RedisStatus("CONFIG SET", parameter, value));
        }

        /// <summary>
        /// 返回当前数据库的键的数量。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回当前数据库的键的数量。</returns>
        public static long DbSize(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));

            return client.Execute(new RedisInteger("DBSIZE"));
        }

        /// <summary>
        /// 清空整个 Redis 服务器的数据(删除所有数据库的所有键)。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回一个结果。</returns>
        public static Result FlushAll(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));

            return client.Execute(new RedisStatus("FLUSHALL"));
        }
        /// <summary>
        /// 清空当前数据库中的所有键。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回一个结果。</returns>
        public static Result FlushDb(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));

            return client.Execute(new RedisStatus("FLUSHDB"));
        }

        /// <summary>
        /// 以一种易于解释（parse）且易于阅读的格式，返回关于 Redis 服务器的各种信息和统计数值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="section">节点。</param>
        /// <returns>返回一个字符串。</returns>
        public static string Info(this IRedisClient client, string section = null)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));

            return client.Execute(new RedisString("INFO", section == null ? new object[0] : new object[] { section }));
        }

        /// <summary>
        /// 返回最近一次 Redis 成功将数据保存到磁盘上的时间。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回一个时间。</returns>
        public static DateTime LastSave(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            return client.Execute(new RedisDate("LASTSAVE"));
        }

        /// <summary>
        /// 将当前 Redis 实例的所有数据快照(snapshot)以 RDB 文件的形式保存到硬盘。
        /// <para>一般来说，在生产环境很少执行 SAVE 操作，因为它会阻塞所有客户端，保存数据库的任务通常由 BGSAVE 命令异步地执行。然而，如果负责保存数据的后台子进程不幸出现问题时， SAVE 可以作为保存数据的最后手段来使用。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>返回一个结果。</returns>
        public static Result Save(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            return client.Execute(new RedisStatus("SAVE"));
        }
    }
}
