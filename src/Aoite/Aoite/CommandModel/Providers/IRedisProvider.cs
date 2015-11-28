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
}
