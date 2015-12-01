using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Aoite.Logger
{
    /// <summary>
    /// 表示一个日志管理器的基类。
    /// </summary>
    public abstract class LoggerBase : ILogger
    {
        private readonly ConcurrentQueue<LogItem> _QueueItems = new ConcurrentQueue<LogItem>();

        /// <summary>
        /// 获取或设置日志等待时间的间隔。默认为 1 秒钟。
        /// </summary>
        public TimeSpan Interval { get { return this._ajob.Interval; } set { this._ajob.Interval = value; } }

        /// <summary>
        /// 获取或设置一个值，指示是否为异步模式。默认为 true。
        /// </summary>
        public bool Asynchronous { get; set; }


        private IAsyncJob _ajob;
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Logger.LoggerBase"/> 类的新实例。
        /// </summary>
        public LoggerBase()
        {
            this._ajob = Ajob.Loop(AsyncWrite, TimeSpan.FromSeconds(1));
        }

        void AsyncWrite(IAsyncJob job)
        {
            if(this._QueueItems.Count == 0) return;

            List<LogItem> items = new List<LogItem>(this._QueueItems.Count);

            LogItem item;
            while(this._QueueItems.TryDequeue(out item)) items.Add(item);

            try
            {
                this.OnWrite(items.ToArray());
            }
            catch(Exception ex)
            {
                GA.WriteUnhandledException("日志线程崩溃了：{0}", ex);
                job.Cancel();
            }
        }


        /// <summary>
        /// 写入一个或多个日志项。
        /// </summary>
        /// <param name="items">日志项的数组。</param>
        public void Write(params LogItem[] items)
        {
            if(items == null) throw new ArgumentNullException(nameof(items));
            if(this.Asynchronous)
            {
                foreach(var item in items)
                {
                    this._QueueItems.Enqueue(item);
                }
            }
            else
            {
                lock(this)
                {
                    OnWrite(items);
                }
            }
        }

        /// <summary>
        /// 异步写入一个或多个日志项。
        /// </summary>
        /// <param name="items">日志项的数组。</param>
        protected abstract void OnWrite(params LogItem[] items);
    }
}
