using Aoite.Redis;
using Aoite.Redis.Commands;
using System.Linq;

namespace System
{
    /// <summary>
    /// 表示 Redis 的 Script 扩展方法。
    /// </summary>
    public static class RedisScriptExtensions
    {
        /// <summary>
        /// 执行 Lua 脚本进行求值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="script">Lua 脚本代码。</param>
        /// <param name="keyArgs">键名和参数值的字典。</param>
        /// <returns>执行脚本后的值。</returns>
        public static object Eval(this IRedisClient client, string script, RedisDictionary keyArgs)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            if(string.IsNullOrWhiteSpace(script)) throw new ArgumentNullException(nameof(script));
            if(keyArgs == null) throw new ArgumentNullException(nameof(keyArgs));

            var args = RedisArgs.ConcatAll(new object[] { script, keyArgs.Keys.Count }, keyArgs.Keys, keyArgs.Values);
            return client.Execute(new RedisObject("EVAL", args.ToArray()));
        }

        /// <summary>
        /// 根据给定的 sha1 校验码，对缓存在服务器中的脚本进行求值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="sha1">sha1 校验码。</param>
        /// <param name="keyArgs">键名和参数值的字典。</param>
        /// <returns>执行脚本后的值。</returns>
        public static object EvalSHA(this IRedisClient client, string sha1, RedisDictionary keyArgs)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            if(string.IsNullOrWhiteSpace(sha1)) throw new ArgumentNullException(nameof(sha1));
            if(keyArgs == null) throw new ArgumentNullException(nameof(keyArgs));

            var args = RedisArgs.ConcatAll(new object[] { sha1, keyArgs.Keys.Count }, keyArgs.Keys, keyArgs.Values);
            return client.Execute(new RedisObject("EVALSHA", args.ToArray()));
        }

        /// <summary>
        /// 给定一个或多个脚本的 SHA1 校验和，表示校验和所指定的脚本是否已经被保存在缓存当中。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="scripts">sha1 校验码列表。</param>
        /// <returns>脚本已经被保存在缓存当中为 true，否则为 false 的数组。</returns>
        public static bool[] ScriptExists(this IRedisClient client, params string[] scripts)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            if(scripts == null) throw new ArgumentNullException(nameof(scripts));
            if(scripts.Length == 0) return new bool[0];
            return client.Execute(RedisArray.Create(new RedisBoolean("SCRIPT EXISTS", scripts)));
        }

        /// <summary>
        /// 清除所有 Lua 脚本缓存。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>结果。</returns>
        public static Result ScriptFlush(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            return client.Execute(new RedisStatus("SCRIPT FLUSH"));
        }

        /// <summary>
        /// 杀死当前正在运行的 Lua 脚本，当且仅当这个脚本没有执行过任何写操作时，这个命令才生效。
        /// <para>这个命令主要用于终止运行时间过长的脚本，比如一个因为 BUG 而发生无限 loop 的脚本，诸如此类。</para>
        /// <para>SCRIPT KILL 执行之后，当前正在运行的脚本会被杀死，执行这个脚本的客户端会从 EVAL 命令的阻塞当中退出，并收到一个错误作为返回值。</para>
        /// <para>另一方面，假如当前正在运行的脚本已经执行过写操作，那么即使执行 SCRIPT KILL ，也无法将它杀死，因为这是违反 Lua 脚本的原子性执行原则的。在这种情况下，唯一可行的办法是使用 SHUTDOWN NOSAVE 命令，通过停止整个 Redis 进程来停止脚本的运行，并防止不完整(half-written)的信息被写入数据库中。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>结果。</returns>
        public static Result ScriptKill(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            return client.Execute(new RedisStatus("SCRIPT KILL"));
        }

        /// <summary>
        /// 将脚本 <paramref name="script"/> 添加到脚本缓存中，但并不立即执行这个脚本。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="script">Lua 脚本代码。</param>
        /// <returns>给定 <paramref name="script"/> 的 SHA1 校验和。</returns>
        public static string ScriptLoad(this IRedisClient client, string script)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            if(string.IsNullOrWhiteSpace(script)) throw new ArgumentNullException(nameof(script));

            return client.Execute(new RedisString("SCRIPT LOAD", script));
        }

    }
}
