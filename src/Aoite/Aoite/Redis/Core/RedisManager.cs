using System;
using Aoite.Net;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示一个 Redis 的管理器。
    /// </summary>
    public class RedisManager : ObjectPool<IRedisClient>
    {
        /// <summary>
        /// 获取或设置 Redis 的连接地址，默认为 localhost:6379。
        /// </summary>
        public static SocketInfo DefaultAddress = 6379;
        /// <summary>
        /// 获取或设置 Redis 的连接密码，默认为 nul 值。
        /// </summary>
        public static string DefaultPassword;

        private static readonly System.Threading.ThreadLocal<IRedisClient> ThreadClient = new System.Threading.ThreadLocal<IRedisClient>();

        /// <summary>
        /// 获取当前线程上下文的 Redis 上下文。
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public static IRedisClient Context
        {
            get
            {
                if(DefaultAddress == null) throw new NotSupportedException("没有配置默认的 Redis 连接地址。");

                if(ThreadClient.Value == null) ThreadClient.Value = new RedisClient(DefaultAddress, DefaultPassword);
                return ThreadClient.Value;
            }
        }

        /// <summary>
        /// 获取一个值，指示当前 Redis 上下文在线程中是否已创建。
        /// </summary>
        public static bool IsThreadContext { get { return ThreadClient.Value != null; } }

        /// <summary>
        /// 释放并关闭当前线程上下文的 Redis 上下文。
        /// </summary>
        public static void ResetContext()
        {
            var context = ThreadClient.Value;
            if(context != null)
            {
                context.Dispose();
                ThreadClient.Value = null;
            }
        }

        private SocketInfo _Address;
        /// <summary>
        /// 获取 Redis 的连接地址。
        /// </summary>
        public SocketInfo Address { get { return this._Address; } }

        private string _Password;
        /// <summary>
        /// 获取 Redis 的连接密码。
        /// </summary>
        public string Password { get { return this._Password; } }

        /// <summary>
        /// 初始化一个 <see cref="RedisManager"/> 类的新实例。
        /// </summary>
        public RedisManager() : this(DefaultAddress, DefaultPassword) { }

        /// <summary>
        /// 初始化一个 <see cref="RedisManager"/> 类的新实例。
        /// </summary>
        /// <param name="address">Redis 的连接地址。</param>
        /// <param name="password">Redis 的连接密码。</param>
        public RedisManager(SocketInfo address, string password = null)
        {
            this._Address = address;
            this._Password = password;
        }

        /// <summary>
        /// 获取一个对象池的对象。
        /// </summary>
        /// <returns>新的对象。</returns>
        public override IRedisClient Acquire()
        {
            return new RedisClient(this._Address, this._Password);
        }

        /// <summary>
        /// 释放一个对象，并将其放入对象池中。
        /// </summary>
        /// <param name="obj">对象池。</param>
        public override void Release(IRedisClient obj)
        {
            obj.Dispose();
        }
    }
}
