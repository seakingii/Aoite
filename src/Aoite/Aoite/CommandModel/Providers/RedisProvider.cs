using Aoite.Redis;
using System;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个默认的 Redis 提供程序。
    /// </summary>
    [SingletonMapping]
    public class RedisProvider : CommandModelContainerProviderBase, IRedisProvider
    {
        /// <summary>
        /// 初始化一个 <see cref="RedisProvider"/> 类的新实例
        /// </summary>
        /// <param name="container">服务容器。</param>
        public RedisProvider(IIocContainer container) : base(container) { }

        /// <summary>
        /// 获取一个 Redis 的客户端，在代码中无需释放此客户端。
        /// </summary>
        /// <returns>Redis 的客户端。</returns>
        public virtual IRedisClient GetRedisClient()
        {
            return RedisManager.Context;
        }
    }
}
