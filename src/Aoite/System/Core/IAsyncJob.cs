﻿namespace System
{
    /// <summary>
    /// 表示一个异步任务的委托。
    /// </summary>
    /// <param name="job">异步任务。</param>
    public delegate void AsyncJobHandler(IAsyncJob job);

    /// <summary>
    /// 表示一个异步任务的标识。
    /// </summary>
    public interface IAsyncJob
    {
        /// <summary>
        /// 获取自定义数据。
        /// </summary>
        object State { get; }
        /// <summary>
        /// 获取关联的异步操作。
        /// </summary>
        System.Threading.Tasks.Task Task { get; }
        /// <summary>
        /// 获取或设置一个值，表示间隔的时间。只有当循环的任务时，此值才有效。
        /// </summary>
        TimeSpan Interval { get; set; }
        /// <summary>
        /// 获取一个值，指示任务是否已取消。
        /// </summary>
        bool IsCanceled { get; }
        /// <summary>
        /// 获取一个值，指示任务是否发生异常。
        /// </summary>
        bool IsFaulted { get; }
        /// <summary>
        /// 获取一个值，指示任务是否已成功完成。
        /// </summary>
        bool IsSuccessed { get; }
        /// <summary>
        /// 获取一个值，指示任务是否正在进行。
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// 立即取消异步任务。
        /// </summary>
        void Cancel();
        /// <summary>
        /// 等待完成执行过程。
        /// </summary>
        /// <param name="millisecondsTimeout">等待的毫秒数，或为 <see cref="Threading.Timeout.Infinite"/>(-1)，表示无限期等待。</param>
        /// <returns>如果在分配的时间内完成执行，则为 true；否则为 false。</returns>
        bool Wait(int millisecondsTimeout = System.Threading.Timeout.Infinite);

        /// <summary>
        /// 延迟时间执行任务。
        /// </summary>
        /// <param name="millisecondsDelay">等待延迟的毫秒数。</param>
        void Delay(int millisecondsDelay);
        /// <summary>
        /// 延迟时间执行任务。
        /// </summary>
        /// <param name="timeSpanDelay">等待延迟的间隔。</param>
        void Delay(TimeSpan timeSpanDelay);
    }
}
