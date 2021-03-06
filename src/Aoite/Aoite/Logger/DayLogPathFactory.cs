﻿using System;
using System.IO;

namespace Aoite.Logger
{
    /// <summary>
    /// 表示一个以“天”为单位的日志路径生成工厂。
    /// </summary>
    public class DayLogPathFactory : ILogPathFactory
    {
        /// <summary>
        /// 初始化一个 <see cref="DayLogPathFactory"/> 类的新实例。
        /// </summary>
        public DayLogPathFactory() { }

        private DateTime _lastCreateTime;
        /// <summary>
        /// 指定当前时间，判断路径是否已创建。
        /// </summary>
        /// <param name="now">当前时间。</param>
        /// <returns>如果路径已创建返回 true，否则返回 false。</returns>
        public virtual bool IsCreated(DateTime now)
        {
            return this._lastCreateTime.Year == now.Year
                && this._lastCreateTime.Month == now.Month
                && this._lastCreateTime.Day == now.Day;
        }

        /// <summary>
        /// 创建指定时间、日志目录和日志后缀名，创建一个路径。
        /// </summary>
        /// <param name="now">当前时间。</param>
        /// <param name="logFolder">日志的目录。</param>
        /// <param name="logExtension">日志的后缀名。</param>
        /// <returns>日志的路径。</returns>
        public virtual string CreatePath(DateTime now, string logFolder, string logExtension)
        {
            this._lastCreateTime = now;
            var folder = Path.Combine(logFolder, now.ToString("yyyy年MM月"));
            var path = Path.Combine(folder, now.Day.ToString("00") + logExtension);
            GA.IO.CreateDirectory(folder);
            return path;
        }
    }
}
