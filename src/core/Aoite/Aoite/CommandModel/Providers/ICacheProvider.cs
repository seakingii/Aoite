using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个提供缓存功能的提供程序。
    /// </summary>
    public interface ICacheProvider : IContainerProvider
    {
        /// <summary>
        /// 设置缓存。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="value">缓存值。</param>
        void Set(string key, object value);
        /// <summary>
        /// 获取缓存。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="valueFactory">若找不到缓存时的延迟设置回调方法。</param>
        /// <returns>返回缓存值，或一个 null 值。</returns>
        object Get(string key, Func<object> valueFactory = null);
        /// <summary>
        /// 检测指定的缓存键是否存在。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        bool Exists(string key);
    }

    /// <summary>
    /// 表示一个提供缓存功能的提供程序。
    /// </summary>
    [SingletonMapping]
    public class CacheProvider : CommandModelContainerProviderBase, ICacheProvider
    {
        const string RedisHashKey = "$RedisCacheProvider$";
        /// <summary>
        /// 初始化一个 <see cref="Aoite.CommandModel.CacheProvider"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public CacheProvider(IIocContainer container)
            : base(container)
        {
            this._lazyLdb = new Lazy<LevelDB.LDB>(() =>
            {
                var dbFolder = GA.IsWebRuntime
                    ? System.Web.HttpContext.Current.Server.MapPath("~/App_Data/CacheDb")
                    : GA.FullPath("CacheDb");
                GA.IO.CreateDirectory(dbFolder);
                return new LevelDB.LDB(dbFolder);
            });
        }

        readonly Lazy<LevelDB.LDB> _lazyLdb;

        private string GetMoneryCache(string key)
        {
            return RedisHashKey + ":" + key;
        }
        /// <summary>
        /// 设置缓存。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="value">缓存值。</param>
        public virtual void Set(string key, object value)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            if(GA.IsUnitTestRuntime)
            {
                this.Container.GetService<CommandMemoryCache>()[this.GetMoneryCache(key)] = value;
                return;
            }

            var redisProvider = this.Container.GetFixedService<IRedisProvider>();
            if(redisProvider != null)
            {
                var client = redisProvider.GetRedisClient();
                if(value == null) client.HDel(RedisHashKey, key);
                else client.HSet(RedisHashKey, key, new BinaryValue(value));
            }
            else
            {
                var ldb = _lazyLdb.Value;
                if(value == null) ldb.Delete(key);
                else ldb.Put(key, new BinaryValue(value));
            }
        }

        /// <summary>
        /// 获取缓存。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="valueFactory">若找不到缓存时的延迟设置回调方法。</param>
        /// <returns>返回缓存值，或一个 null 值。</returns>
        public virtual object Get(string key, Func<object> valueFactory = null)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            object value;
            if(GA.IsUnitTestRuntime)
            {
                var mc = this.Container.GetService<CommandMemoryCache>();
                key = this.GetMoneryCache(key);
                value = mc[key];
                if(value == null && valueFactory != null)
                {
                    value = valueFactory();
                    if(value == null) return null;
                    mc[key] = value;
                }
                return value;
            }

            var redisProvider = this.Container.GetFixedService<IRedisProvider>();

            if(redisProvider != null)
            {
                var client = redisProvider.GetRedisClient();
                var binaryValue = client.HGet(RedisHashKey, key);
                value = binaryValue == null ? null : binaryValue.ToModel();
                if(value == null && valueFactory != null)
                {
                    using(this.Container.GetService<ILockProvider>().Lock(RedisHashKey + ":" + key))
                    {
                        value = valueFactory();
                        if(value == null) return null;
                        client.HSet(RedisHashKey, key, new BinaryValue(value));
                    }
                    return value;
                }
            }
            else
            {
                var ldb = _lazyLdb.Value;
                var binaryValue = ldb.Get(key);
                value = binaryValue == null ? null : binaryValue.ToModel();
                if(value == null && valueFactory != null)
                {
                    lock(string.Intern(RedisHashKey + ":" + key))
                    {
                        value = valueFactory();
                        if(value == null) return null;
                        ldb.Put(key, new BinaryValue(value));
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// 检测指定的缓存键是否存在。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public virtual bool Exists(string key)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            if(GA.IsUnitTestRuntime)
                return this.Container.GetService<CommandMemoryCache>().Contains(this.GetMoneryCache(key));

            var redisProvider = this.Container.GetFixedService<IRedisProvider>();
            if(redisProvider != null) return redisProvider.GetRedisClient().HExists(RedisHashKey, key);

            return _lazyLdb.Value.Get(key) != null;
        }
    }
}
