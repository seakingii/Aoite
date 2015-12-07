using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型的缓存策略。
    /// </summary>
    public class CommandCacheStrategy : ICommandCacheStrategy
    {
        /// <summary>
        /// 获取缓存项的键。
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 获取执行的命令模型。
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// 获取执行的上下文。
        /// </summary>
        public IContext Context { get; }

        /// <summary>
        /// 获取一个值，该值指示如果某个缓存项在给定时段内未被访问，是否应被逐出。
        /// </summary>
        public TimeSpan SlidingExpiration { get; }

        /// <summary>
        /// 获取一个值，表示 <seealso cref="SlidingExpiration"/> 是否具有有效值。
        /// </summary>
        protected bool HasSlidingExpiration { get { return this.SlidingExpiration != ObjectCache.NoSlidingExpiration; } }

        /// <summary>
        /// 获取一个值，该值指示是否应在指定持续时间过后逐出某个缓存项。
        /// </summary>
        public DateTimeOffset AbsoluteExpiration { get; }

        /// <summary>
        /// 获取一个值，表示 <seealso cref="AbsoluteExpiration"/> 是否具有有效值。
        /// </summary>
        protected bool HasAbsoluteExpiration { get { return this.AbsoluteExpiration != ObjectCache.InfiniteAbsoluteExpiration; } }

        /// <summary>
        /// 以滑动间隔过期方式，初始化一个 <see cref="CommandCacheStrategy"/> 类的新实例。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="slidingExpiration">缓存项的过期间隔。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <param name="context">执行的上下文。</param>
        public CommandCacheStrategy(string key, TimeSpan slidingExpiration, ICommand command, IContext context)
            : this(key, command, context, slidingExpiration, ObjectCache.InfiniteAbsoluteExpiration)  { }

        /// <summary>
        /// 以绝对间隔过期方式，初始化一个 <see cref="CommandCacheStrategy"/> 类的新实例。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="absoluteExpiration">缓存项的过期间隔。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <param name="context">执行的上下文。</param>
        public CommandCacheStrategy(string key, DateTimeOffset absoluteExpiration, ICommand command, IContext context)
            : this(key, command, context, ObjectCache.NoSlidingExpiration, absoluteExpiration) { }

        private CommandCacheStrategy(string key, ICommand command, IContext context, TimeSpan slidingExpiration, DateTimeOffset absoluteExpiration)
        {
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            if(command == null) throw new ArgumentNullException(nameof(command));
            if(context == null) throw new ArgumentNullException(nameof(context));

            this.Key = key;
            this.Command = command;
            this.Context = context;
            this.SlidingExpiration = slidingExpiration;
            this.AbsoluteExpiration = absoluteExpiration;
        }


        const string RedisHashKey = "$RedisCCS::";

        /// <summary>
        /// 获取缓存中的项。
        /// </summary>
        /// <param name="group">缓存键的分组。</param>
        /// <returns>缓存中的项，如果缓存不存在项，则返回 null 值。</returns>
        public virtual object GetCache(string group)
        {
            var container = this.Context.Container;
            var key = group + this.Key;
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
            var container = this.Context.Container;
            var key = group + this.Key;

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
                cacheProvider.Set(key, value, new CacheItemPolicy() { SlidingExpiration = this.SlidingExpiration, AbsoluteExpiration = this.AbsoluteExpiration });
            }
        }
    }
}
