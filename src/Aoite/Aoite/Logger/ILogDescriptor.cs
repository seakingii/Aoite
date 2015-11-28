using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Logger
{
    /// <summary>
    /// 定义一个日志描述器。
    /// </summary>
    [DefaultMapping(typeof(LogDescriptor))]
    public interface ILogDescriptor
    {
        /// <summary>
        /// 描述指定日志管理器的日志项。
        /// </summary>
        /// <param name="logger">日志管理器。</param>
        /// <param name="item">日志项。</param>
        /// <returns>返回日志项的字符串形式。</returns>
        string Describe(ILogger logger, LogItem item);
    }
}
