using System;

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
        /// <returns>缓存值，或一个 null 值。</returns>
        object Get(string key, Func<object> valueFactory = null);
        /// <summary>
        /// 检测指定的缓存键是否存在。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        bool Exists(string key);
    }
}
