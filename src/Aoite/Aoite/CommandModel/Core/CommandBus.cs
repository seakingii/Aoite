using System;
using System.Collections.Specialized;
using System.Data;
using System.Threading.Tasks;
using Aoite.Data;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型的总线。
    /// </summary>
    [SingletonMapping]
    public class CommandBus : CommandModelContainerProviderBase, ICommandBus
    {
        /// <summary>
        /// 初始化一个 <see cref="CommandBus"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public CommandBus(IIocContainer container) : base(container) { }

        /// <summary>
        /// 创建一个命令模型的上下文。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <returns>命令模型的上下文。</returns>
        protected virtual IContext CreateContext<TCommand>(TCommand command) where TCommand : ICommand
        {
            return this.Container.Get<IContextFactory>().Create(command
                , new Lazy<HybridDictionary>()
                , new Lazy<IDbEngine>(() => this.GetDbEngine()));
        }

        /// <summary>
        /// 执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="executing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        /// <returns>命令模型。</returns>
        public virtual TCommand Execute<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand
        {
            if(this.IsPipeCreated) return Pipe.Execute(command, executing, executed);

            return GlobalExecute(this.Container, this.CreateContext(command), command, executing, executed);
        }

        /// <summary>
        /// 以异步的方式执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="executing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        /// <returns>异步操作。</returns>
        public virtual Task<TCommand> ExecuteAsync<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand
        {
            if(this.IsPipeCreated) return Pipe.ExecuteAsync(command, executing, executed);

            return GlobalExecuteAsync(this.Container, this.CreateContext(command), command, executing, executed);
        }

        internal static TCommand GlobalExecute<TCommand>(IIocContainer container, IContext context, TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand
        {
            if(Equals(command, default(TCommand))) throw new ArgumentNullException(nameof(command));

            var executorFactory = container.Get<IExecutorFactory>();
            var eventStore = container.Get<IEventStore>();

            var executorMetadata = executorFactory.Create(command);
            var executor = executorMetadata.Executor;

            // 执行器元数据 && 事件仓储
            if(executorMetadata.RaiseExecuting(context, command)
                && eventStore.RaiseExecuting(context, command)
                && (executing == null || executing(context, command)))
            {
                try
                {
                    executor.Execute(context, command);
                }
                catch(Exception ex)
                {
                    GA.TraceError("执行命令 {0} 时发生了异常：\r\n{1}", command, ex);
                    if(executed != null) executed(context, command, ex);
                    eventStore.RaiseExecuted(context, command, ex);
                    executorMetadata.RaiseExecuted(context, command, ex);
                    //- 报错不执行 RaiseExecuted
                    throw;
                }
            }/*有一些特殊情况是不需要执行命令的，比如缓存命中。*/

            if(executed != null) executed(context, command, null);
            eventStore.RaiseExecuted(context, command, null);
            executorMetadata.RaiseExecuted(context, command, null);
            return command;
        }

        internal static Task<TCommand> GlobalExecuteAsync<TCommand>(IIocContainer container, IContext context, TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand
        {
            if(Equals(command, default(TCommand))) throw new ArgumentNullException(nameof(command));

            var executorFactory = container.Get<IExecutorFactory>();
            var eventStore = container.Get<IEventStore>();

            var executorMetadata = executorFactory.Create(command);
            var executor = executorMetadata.Executor;

            Task<TCommand> task;
            // 执行器元数据 && 事件仓储
            if(executorMetadata.RaiseExecuting(context, command)
                && eventStore.RaiseExecuting(context, command)
                && (executing == null || executing(context, command)))
            {
#if !NET40
                var executorAsync = executor as IExecutorAsync<TCommand>;
                if(executorAsync != null)
                {//- 异步命令
                    task = executorAsync.ExecuteAsync(context, command);
                }
                else
#endif
                {//- 非异步命令
                    task = Task.Factory.StartNew(() =>
                    {
                        executor.Execute(context, command);
                        return command;
                    });
                }

                task.ContinueWith(t =>
                {
                    var ex = t.Exception;
                    GA.TraceError("执行命令 {0} 时发生了异常：\r\n{1}", command, ex);
                    if(executed != null) executed(context, command, ex);
                    eventStore.RaiseExecuted(context, command, ex);
                    executorMetadata.RaiseExecuted(context, command, ex);

                }, TaskContinuationOptions.OnlyOnFaulted);

            }
            else
            {/*有一些特殊情况是不需要执行命令的，比如缓存命中。*/
#if !NET40
                task = Task.FromResult(command);
#else
                task = GA.FromResult(command);
#endif
            }

            task.ContinueWith(t =>
            {
                if(executed != null) executed(context, command, null);
                eventStore.RaiseExecuted(context, command, null);
                executorMetadata.RaiseExecuted(context, command, null);
            }, TaskContinuationOptions.NotOnFaulted);

            //task.ContinueWith(t =>
            //{
            //    GA.ResetContexts();
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();
            //}, TaskContinuationOptions.ExecuteSynchronously);

            return task;
        }

        private readonly Lazy<System.Threading.ThreadLocal<ICommandBusPipe>> _lazyThreadLocalPipe = new Lazy<System.Threading.ThreadLocal<ICommandBusPipe>>();

        /// <summary>
        /// 获取一个值，指示当前命令总线的上下文管道是否已创建。
        /// </summary>
        public virtual bool IsPipeCreated
        {
            get
            {
                if(this._lazyThreadLocalPipe.IsValueCreated)
                {
                    var pipe = this._lazyThreadLocalPipe.Value.Value;
                    if(pipe != null && !pipe.IsDisposed) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 获取一个命令总线的上下文管道。
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public virtual ICommandBusPipe Pipe
        {
            get
            {

                var pipe = _lazyThreadLocalPipe.Value.Value;
                if(pipe == null || pipe.IsDisposed)
                {
                    pipe = new CommandBusPipe(this);
                    _lazyThreadLocalPipe.Value.Value = pipe;
                }
                return pipe;
            }
        }

        internal void ResetPipe()
        {
            if(this.IsPipeCreated) _lazyThreadLocalPipe.Value.Value = null;
        }

        /// <summary>
        /// 获取一个命令总线的上下文管道，并开始事务模式。
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public ICommandBusPipe PipeTransaction => Pipe.UseTransaction();
    }


    class CommandBusPipe : ObjectDisposableBase, ICommandBusPipe
    {
        readonly CommandBus _bus;
        Lazy<IDbEngine> _lazyEngine;
        readonly Lazy<HybridDictionary> _lazyData;

        IIocContainer IContainerProvider.Container { get { return this._bus.Container; } set { this._bus.Container = value; } }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        ICommandBusPipe ICommandBus.Pipe => this;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        ICommandBusPipe ICommandBus.PipeTransaction => this.UseTransaction();

        bool ICommandBus.IsPipeCreated => true;

        internal CommandBusPipe(CommandBus bus)
        {
            if(bus == null) throw new ArgumentNullException(nameof(bus));
            this._bus = bus;
            this._lazyData = new Lazy<HybridDictionary>();
            this._lazyEngine = new Lazy<IDbEngine>(() => this.GetDbEngine().Context);
        }


        public ICommandBusPipe UseTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            this._lazyEngine.Value.ContextTransaction.OpenTransaction(isolationLevel);
            return this;
        }


        IContext CreateContext<TCommand>(TCommand command) where TCommand : ICommand
        {
            return this._bus.Container.Get<IContextFactory>().Create(command
                , this._lazyData
                , this._lazyEngine);
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            if(this._lazyEngine.IsValueCreated) this._lazyEngine.Value.Context.Dispose();
            this._bus.ResetPipe();
        }

        ICommandBusPipe ICommandBusPipe.UseTransaction(IsolationLevel isolationLevel)
        {
            if(this._lazyEngine.IsValueCreated) this._lazyEngine.Value.ContextTransaction.OpenTransaction(isolationLevel);
            else this._lazyEngine = new Lazy<IDbEngine>(() => this.GetDbEngine().Context.OpenTransaction(isolationLevel));

            return this;
        }

        void ICommandBusPipe.Commit()
        {
            if(this._lazyEngine.IsValueCreated) this._lazyEngine.Value.Context.Commit();
        }

        void ICommandBusPipe.Rollback()
        {
            if(this._lazyEngine.IsValueCreated) this._lazyEngine.Value.Context.Rollback();
        }

        TCommand ICommandBus.Execute<TCommand>(TCommand command, CommandExecutingHandler<TCommand> executing, CommandExecutedHandler<TCommand> executed)
            => CommandBus.GlobalExecute(this._bus.Container, this.CreateContext(command), command, executing, executed);

        Task<TCommand> ICommandBus.ExecuteAsync<TCommand>(TCommand command, CommandExecutingHandler<TCommand> executing, CommandExecutedHandler<TCommand> executed)
            => CommandBus.GlobalExecuteAsync(this._bus.Container, this.CreateContext(command), command, executing, executed);
    }
}
