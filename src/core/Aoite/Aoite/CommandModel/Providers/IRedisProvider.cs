using Aoite.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义 Redis 的提供程序。
    /// </summary>
    public interface IRedisProvider : IContainerProvider
    {
        /// <summary>
        /// 获取一个 Redis 的客户端，在代码中无需释放此客户端。
        /// </summary>
        /// <returns>返回一个 Redis 的客户端。</returns>
        IRedisClient GetRedisClient();
    }

    /// <summary>
    /// 表示一个默认的 Redis 提供程序。
    /// </summary>
    [SingletonMapping]
    public class RedisProvider : CommandModelContainerProviderBase, IRedisProvider
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.CommandModel.RedisProvider"/> 类的新实例
        /// </summary>
        /// <param name="container">服务容器。</param>
        public RedisProvider(IIocContainer container) : base(container) { }

        /// <summary>
        /// 获取一个 Redis 的客户端，在代码中无需释放此客户端。
        /// </summary>
        /// <returns>返回一个 Redis 的客户端。</returns>
        public virtual IRedisClient GetRedisClient()
        {
            return RedisManager.Context;
        }
    }
}
