using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个支持缓存的命令模型。
    /// </summary>
    public interface ICommandCache
    {
        /// <summary>
        /// 提供执行的上下文，获取缓存策略。
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <returns>返回一个缓存策略。</returns>
        ICommandCacheStrategy CreateStrategy(IContext context);
        /// <summary>
        /// 设置缓存的值。
        /// </summary>
        /// <param name="value">缓存值。</param>
        /// <returns>返回一个值，表示缓存值是否有效的赋值。返回 false 表示缓存值无效。</returns>
        bool SetCacheValue(object value);
        /// <summary>
        /// 获取需缓存的值。
        /// </summary>
        /// <returns>返回缓存值。</returns>
        object GetCacheValue();
    }
}
