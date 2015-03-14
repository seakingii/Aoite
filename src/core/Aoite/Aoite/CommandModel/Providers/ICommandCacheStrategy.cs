using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令模型的缓存策略。
    /// </summary>
    public interface ICommandCacheStrategy
    {
        /// <summary>
        /// 获取缓存项的键。
        /// </summary>
        string Key { get; }
        /// <summary>
        /// 获取一个值，该值指示如果某个缓存项在给定时段内未被访问，是否应被逐出。
        /// </summary>
        TimeSpan SlidingExpiration { get; }

        /// <summary>
        /// 获取一个值，该值指示是否应在指定持续时间过后逐出某个缓存项。
        /// </summary>
        DateTimeOffset AbsoluteExpiration { get; }

        /// <summary>
        /// 获取缓存中的项。
        /// </summary>
        /// <param name="group">缓存键的分组。</param>
        /// <returns>返回缓存中的项，如果缓存不存在项，则返回 null 值。</returns>
        object GetCache(string group);
        /// <summary>
        /// 提供缓存键的分组，设置缓存项。
        /// </summary>
        /// <param name="group">缓存键的分组。</param>
        /// <param name="value">缓存的值。</param>
        void SetCache(string group, object value);
    }

    [SingletonMapping]
    class CommandMemoryCache : MemoryCache
    {
        public CommandMemoryCache() : base("CommandMemoryCache") { }
    }

    /// <summary>
    /// 表示一个命令模型的缓存策略。
    /// </summary>
    public class CommandCacheStrategy : ICommandCacheStrategy
    {
        private string _Key;
        /// <summary>
        /// 获取缓存项的键。
        /// </summary>
        public string Key { get { return _Key; } }

        private TimeSpan _SlidingExpiration;
        /// <summary>
        /// 获取一个值，该值指示如果某个缓存项在给定时段内未被访问，是否应被逐出。
        /// </summary>
        public TimeSpan SlidingExpiration { get { return _SlidingExpiration; } }
        /// <summary>
        /// 获取一个值，表示 <seealso cref="SlidingExpiration"/> 是否具有有效值。
        /// </summary>
        protected bool HasSlidingExpiration { get { return this._SlidingExpiration != ObjectCache.NoSlidingExpiration; } }

        private DateTimeOffset _AbsoluteExpiration;
        /// <summary>
        /// 获取一个值，该值指示是否应在指定持续时间过后逐出某个缓存项。
        /// </summary>
        public DateTimeOffset AbsoluteExpiration { get { return _AbsoluteExpiration; } }
        /// <summary>
        /// 获取一个值，表示 <seealso cref="AbsoluteExpiration"/> 是否具有有效值。
        /// </summary>
        protected bool HasAbsoluteExpiration { get { return this._AbsoluteExpiration != ObjectCache.InfiniteAbsoluteExpiration; } }

        private ICommand _Command;
        /// <summary>
        /// 获取执行的命令模型。
        /// </summary>
        public ICommand Command { get { return _Command; } }

        private IContext _Context;
        /// <summary>
        /// 获取执行的上下文。
        /// </summary>
        public IContext Context { get { return _Context; } }

        /// <summary>
        /// 以滑动间隔过期方式，初始化一个 <see cref="Aoite.CommandModel.CommandCacheStrategy"/> 类的新实例。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="slidingExpiration">缓存项的过期间隔。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <param name="context">执行的上下文。</param>
        public CommandCacheStrategy(string key, TimeSpan slidingExpiration, ICommand command, IContext context)
            : this(key, command, context, slidingExpiration, ObjectCache.InfiniteAbsoluteExpiration) { }

        /// <summary>
        /// 以绝对间隔过期方式，初始化一个 <see cref="Aoite.CommandModel.CommandCacheStrategy"/> 类的新实例。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="absoluteExpiration">缓存项的过期间隔。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <param name="context">执行的上下文。</param>
        public CommandCacheStrategy(string key, DateTimeOffset absoluteExpiration, ICommand command, IContext context)
            : this(key, command, context, ObjectCache.NoSlidingExpiration, absoluteExpiration) { }

        private CommandCacheStrategy(string key, ICommand command, IContext context, TimeSpan slidingExpiration, DateTimeOffset absoluteExpiration)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(command == null) throw new ArgumentNullException("command");
            if(context == null) throw new ArgumentNullException("context");

            this._Key = key;
            this._Command = command;
            this._Context = context;
            this._SlidingExpiration = slidingExpiration;
            this._AbsoluteExpiration = absoluteExpiration;
        }


        const string RedisHashKey = "$RedisCCS::";

        /// <summary>
        /// 获取缓存中的项。
        /// </summary>
        /// <param name="group">缓存键的分组。</param>
        /// <returns>返回缓存中的项，如果缓存不存在项，则返回 null 值。</returns>
        public virtual object GetCache(string group)
        {
            var container = this._Context.Container;
            var key = group + this._Key;
            var redisProvider = container.GetFixedService<IRedisProvider>();
            if(redisProvider != null)
            {
                key = RedisHashKey + key;
                var client = redisProvider.GetRedisClient();
                var model = client.Get(key).ToModel();
                if(model != null && this.HasSlidingExpiration)
                {
                    client.Expire(key, this.SlidingExpiration);
                }
                return model;
            }
            else
            {
                var cacheProvider = container.GetService<CommandMemoryCache>();
                return cacheProvider.Get(key);
            }
        }

        /// <summary>
        /// 提供缓存键的分组，设置缓存项。
        /// </summary>
        /// <param name="group">缓存键的分组。</param>
        /// <param name="value">缓存的值。</param>
        public virtual void SetCache(string group, object value)
        {
            if(value == null) return;
            var container = this._Context.Container;
            var key = group + this._Key;
            
            var redisProvider = container.GetFixedService<IRedisProvider>();
            if(redisProvider != null)
            {
                key = RedisHashKey + key;
                var expiration = this.HasSlidingExpiration ? this.SlidingExpiration : (DateTimeOffset.Now - this.AbsoluteExpiration);

                var client = redisProvider.GetRedisClient();
                client.Set(key, new BinaryValue(value), (long)expiration.TotalSeconds);
            }
            else
            {
                var cacheProvider = container.GetService<CommandMemoryCache>();
                cacheProvider.Set(key, value, new CacheItemPolicy() { SlidingExpiration = this._SlidingExpiration, AbsoluteExpiration = this._AbsoluteExpiration });
            }
        }
    }
}
