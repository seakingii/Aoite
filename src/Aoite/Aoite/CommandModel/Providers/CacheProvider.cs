using System;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个提供缓存功能的提供程序。
    /// </summary>
    [SingletonMapping]
    public class CacheProvider : CommandModelContainerProviderBase, ICacheProvider
    {
        const string RedisHashKey = "$RedisCacheProvider$";

        /// <summary>
        /// 初始化一个 <see cref="CacheProvider"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public CacheProvider(IIocContainer container) : base(container) { }

        private string GetMoneryCache(string key)
        {
            return RedisHashKey + ":" + key;
        }

        private void MemorySet(string key, object value)
        {
            var mc = this.Container.GetService<CommandMemoryCache>();
            if(value == null) mc.Remove(this.GetMoneryCache(key));
            else mc[this.GetMoneryCache(key)] = value;
        }

        /// <summary>
        /// 设置缓存。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="value">缓存值。</param>
        public virtual void Set(string key, object value)
        {
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            if(!GA.IsUnitTestRuntime)
            {
                var redisProvider = this.Container.GetFixedService<IRedisProvider>();
                if(redisProvider != null)
                {
                    var client = redisProvider.GetRedisClient();
                    if(value == null) client.HDel(RedisHashKey, key);
                    else client.HSet(RedisHashKey, key, new BinaryValue(value));
                }
            }

            this.MemorySet(key, value);
        }
        private object MemoryGet(string key, Func<object> valueFactory)
        {
            var mc = this.Container.GetService<CommandMemoryCache>();
            key = this.GetMoneryCache(key);
            var value = mc[key];
            if(value == null && valueFactory != null)
            {
                value = valueFactory();
                if(value != null) mc[key] = value;
            }
            return value;
        }

        /// <summary>
        /// 获取缓存。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="valueFactory">若找不到缓存时的延迟设置回调方法。</param>
        /// <returns>缓存值，或一个 null 值。</returns>
        public virtual object Get(string key, Func<object> valueFactory = null)
        {
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            if(!GA.IsUnitTestRuntime)
            {
                var redisProvider = this.Container.GetFixedService<IRedisProvider>();
                if(redisProvider != null)
                {
                    var client = redisProvider.GetRedisClient();
                    var binaryValue = client.HGet(RedisHashKey, key);
                    var value = binaryValue == null ? null : binaryValue.ToModel();
                    if(value == null && valueFactory != null)
                    {
                        using(this.Container.GetService<ILockProvider>().Lock(RedisHashKey + ":" + key))
                        {
                            value = valueFactory();
                            if(value == null) return null;
                            client.HSet(RedisHashKey, key, new BinaryValue(value));
                        }
                    }
                    return value;
                }
            }

            return MemoryGet(key, valueFactory);
        }

        private bool MemoryExists(string key)
        {
            return this.Container.GetService<CommandMemoryCache>().Contains(this.GetMoneryCache(key));
        }

        /// <summary>
        /// 检测指定的缓存键是否存在。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public virtual bool Exists(string key)
        {
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            if(!GA.IsUnitTestRuntime)
            {
                var redisProvider = this.Container.GetFixedService<IRedisProvider>();
                if(redisProvider != null) return redisProvider.GetRedisClient().HExists(RedisHashKey, key);
            }
            return this.MemoryExists(key);
        }
    }
}
