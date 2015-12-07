using System;

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
        /// <returns>缓存中的项，如果缓存不存在项，则返回 null 值。</returns>
        object GetCache(string group);
        /// <summary>
        /// 提供缓存键的分组，设置缓存项。
        /// </summary>
        /// <param name="group">缓存键的分组。</param>
        /// <param name="value">缓存的值。</param>
        void SetCache(string group, object value);
    }
}
