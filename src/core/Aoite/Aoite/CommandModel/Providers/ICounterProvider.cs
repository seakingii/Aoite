using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个计数的提供程序。
    /// </summary>
    public interface ICounterProvider : IContainerProvider
    {
        /// <summary>
        /// 获取指定键的原子递增序列。
        /// </summary>
        /// <param name="key">序列的键。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        long Increment(string key, long increment = 1);
    }

    /// <summary>
    /// 表示一个计数的提供程序。
    /// </summary>
    [SingletonMapping]
    public class CounterProvider : CommandModelContainerProviderBase, ICounterProvider
    {
        const string ReidsHashKey = "$RedisCounterProvider$";

        /// <summary>
        /// 初始化一个 <see cref="Aoite.CommandModel.CounterProvider"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public CounterProvider(IIocContainer container) : base(container) { }

        readonly System.Collections.Concurrent.ConcurrentDictionary<string, long> _dict = new System.Collections.Concurrent.ConcurrentDictionary<string, long>();

        /// <summary>
        /// 获取指定键的原子递增序列。
        /// </summary>
        /// <param name="key">序列的键。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        public virtual long Increment(string key, long increment = 1)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            var redisProvider = this.Container.GetFixedService<IRedisProvider>();
            if(redisProvider != null)
            {
                var client = redisProvider.GetRedisClient();
                return client.HIncrBy(ReidsHashKey, key, increment);
            }
            return _dict.AddOrUpdate(key, increment, (k, v) => v += increment);
        }
    }
}
