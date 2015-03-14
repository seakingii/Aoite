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
        const string RedisHashKey = "$RedisCounterProvider$";

        /// <summary>
        /// 初始化一个 <see cref="Aoite.CommandModel.CounterProvider"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public CounterProvider(IIocContainer container)
            : base(container)
        {
            this._lazyLdb = new Lazy<LevelDB.LDB>(() =>
            {
                var dbFolder = GA.IsWebRuntime
                    ? System.Web.HttpContext.Current.Server.MapPath("~/App_Data/CounterDb")
                    : GA.FullPath("CounterDb");
                GA.IO.CreateDirectory(dbFolder);
                return new LevelDB.LDB(dbFolder);
            });
        }

        readonly Lazy<System.Collections.Concurrent.ConcurrentDictionary<string, long>> _lazyDict = new Lazy<System.Collections.Concurrent.ConcurrentDictionary<string, long>>();

        readonly Lazy<LevelDB.LDB> _lazyLdb;

        /// <summary>
        /// 获取指定键的原子递增序列。
        /// </summary>
        /// <param name="key">序列的键。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        public virtual long Increment(string key, long increment = 1)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            if(GA.IsUnitTestRuntime)
            {
                return _lazyDict.Value.AddOrUpdate(key, increment, (k, v) => v += increment);
            }

            var redisProvider = this.Container.GetFixedService<IRedisProvider>();
            if(redisProvider != null)
            {
                var client = redisProvider.GetRedisClient();
                return client.HIncrBy(RedisHashKey, key, increment);
            }
            lock(string.Intern(key))
            {
                var ldb = _lazyLdb.Value;
                int value = ldb.Get(key);
                value += 1;
                ldb.Put(key, value);
                return value;
            }
        }
    }
}
