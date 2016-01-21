using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// 表示异步操作的一系列扩展。
    /// </summary>
    public static class Ajob
    {
        class AsyncJob : ObjectDisposableBase, IAsyncJob
        {
            private Task _Task;
            private object _State;
            private TimeSpan _Interval;
            private AsyncJobHandler _Job;
            private CancellationTokenSource _calcelSource;

            public bool IsCanceled { get { return this._Task.IsCanceled || this._calcelSource.IsCancellationRequested; } }
            public bool IsFaulted { get { return this._Task.IsFaulted; } }
            public bool IsSuccessed { get { return this._Task.IsCompleted && !this.IsCanceled; } }
            public bool IsRunning { get { return this._Task.Status == TaskStatus.Running && !this.IsCanceled; } }

            public object State { get { return _State; } }

            public Task Task { get { return this._Task; } }
            public TimeSpan Interval { get { return _Interval; } set { _Interval = value; } }

            public AsyncJob(AsyncJobHandler job, TimeSpan timeSpan, object state)
            {
                this._Interval = timeSpan;
                this._Job = job;
                this._State = state;
                this._calcelSource = new CancellationTokenSource();
            }

            public AsyncJob Start(Action<object> action, TaskCreationOptions options)
            {
                this._Task = Task.Factory.StartNew(action, this, this._calcelSource.Token, options, TaskScheduler.Default);
                return this;
            }

            public bool Wait(int millisecondsTimeout = Timeout.Infinite)
            {
                this.ThrowIfDisposed();
                if(_Task.IsCompleted) return true;
                return _Task.Wait(millisecondsTimeout);
            }

            public void Cancel()
            {
                this.ThrowIfDisposed();
                this._calcelSource.Cancel();
            }
            public bool WaitForNextTask()
            {
                this.ThrowIfDisposed();
                return this._calcelSource.IsCancellationRequested
                    || Thread.CurrentThread.Join(this._Interval)
                    || this._calcelSource.IsCancellationRequested;
            }

            public void RunTask()
            {
                this.ThrowIfDisposed();
                this._Job(this);
            }
            public void Delay(int millisecondsDelay)
            {
                this.ThrowIfDisposed();
                Thread.CurrentThread.Join(millisecondsDelay);
            }
            public void Delay(TimeSpan timeSpanDelay)
            {
                this.ThrowIfDisposed();
                Thread.CurrentThread.Join(timeSpanDelay);
            }
            protected override void Dispose(bool disposing)
            {
                if(disposing)
                {
                    _calcelSource.Dispose();
                    _calcelSource = null;
                }
                base.Dispose(disposing);
            }
        }

        private static void OnceInvoke(object state)
        {
            AsyncJob tme = state as AsyncJob;
            if(tme.WaitForNextTask()) return;
            try
            {
                tme.RunTask();
            }
            catch(Exception ex)
            {
                GA.OnGlobalError(tme, ex);
                throw;
            }
        }
        private static void LoopInvoke(object state)
        {
            AsyncJob tme = state as AsyncJob;
            while(true)
            {
                if(tme.WaitForNextTask()) return;
                try
                {
                    tme.RunTask();
                }
                catch(Exception ex)
                {
                    GA.OnGlobalError(tme, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// 指定 <paramref name="milliseconds"/> 毫秒后执行一次 <paramref name="job"/>。
        /// </summary>
        /// <param name="job">异步任务。</param>
        /// <param name="milliseconds">间隔的毫秒数，为 0 时表示立即执行。</param>
        /// <param name="state">自定义数据。</param>
        /// <returns>异步任务的标识</returns>
        public static IAsyncJob Once(AsyncJobHandler job, double milliseconds = 0, object state = null)
            => Once(job, TimeSpan.FromMilliseconds(milliseconds), state);

        /// <summary>
        /// 指定间隔时间执行一次 <paramref name="job"/>。
        /// </summary>
        /// <param name="interval">间隔的时间。</param>
        /// <param name="job">异步任务。</param>
        /// <param name="state">自定义数据。</param>
        /// <returns>异步任务的标识</returns>
        public static IAsyncJob Once(AsyncJobHandler job, TimeSpan interval, object state = null)
            => new AsyncJob(job, interval, state).Start(OnceInvoke, TaskCreationOptions.PreferFairness);

        /// <summary>
        /// 指定间隔 <paramref name="milliseconds"/> 毫秒后重复执行 <paramref name="job"/>。
        /// </summary>
        /// <param name="milliseconds">间隔的毫秒数，为 0 时表示立即执行。</param>
        /// <param name="job">异步任务。</param>
        /// <param name="state">自定义数据。</param>
        /// <returns>异步任务的标识</returns>
        public static IAsyncJob Loop(AsyncJobHandler job, double milliseconds = 0, object state = null)
            => Loop(job, TimeSpan.FromMilliseconds(milliseconds), state);

        /// <summary>
        /// 指定间隔时间重复执行 <paramref name="job"/>。
        /// </summary>
        /// <param name="interval">间隔的时间。</param>
        /// <param name="job">异步任务。</param>
        /// <param name="state">自定义数据。</param>
        /// <returns>异步任务的标识</returns>
        public static IAsyncJob Loop(AsyncJobHandler job, TimeSpan interval, object state = null)
            => new AsyncJob(job, interval, state).Start(LoopInvoke, TaskCreationOptions.LongRunning);

        /// <summary>
        /// 等待所有的异步任务完成。
        /// </summary>
        /// <param name="jobs">异步任务列表。</param>
        public static void WaitAll(params IAsyncJob[] jobs)
        {
            if(jobs == null) throw new ArgumentNullException(nameof(jobs));

            foreach(var job in jobs)
            {
                if(job != null)
                    job.Wait();
            }
        }
        /// <summary>
        /// 等待所有的异步任务完成。
        /// </summary>
        /// <param name="jobs">异步任务列表。</param>
        public static void WaitAll(IEnumerable<IAsyncJob> jobs)
            => WaitAll(jobs.ToArray());
    }
}
