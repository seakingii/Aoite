using System;
using System.Threading.Tasks;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型的总线。
    /// </summary>
    [SingletonMapping]
    public class CommandBus : CommandModelContainerProviderBase, ICommandBus
    {
        /// <summary>
        /// 初始化 <see cref="CommandBus"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public CommandBus(IIocContainer container) : base(container) { }

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
            if(command == null) throw new ArgumentNullException(nameof(command));

            var contextFactory = this.Container.Get<IContextFactory>();
            var executorFactory = this.Container.Get<IExecutorFactory>();
            var eventStore = this.Container.Get<IEventStore>();

            var context = contextFactory.Create(command);
            var executorMetadata = executorFactory.Create(command);
            var executor = executorMetadata.Executor;

            // 执行器元数据 && 事务仓储
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
            if(command == null) throw new ArgumentNullException(nameof(command));

            var contextFactory = this.Container.Get<IContextFactory>();
            var executorFactory = this.Container.Get<IExecutorFactory>();
            var eventStore = this.Container.Get<IEventStore>();

            var context = contextFactory.Create(command);
            var executorMetadata = executorFactory.Create(command);
            var executor = executorMetadata.Executor;

            Task<TCommand> task;
            // 执行器元数据 && 事务仓储
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

            task.ContinueWith(t =>
            {
                GA.ResetContexts();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }, TaskContinuationOptions.ExecuteSynchronously);

            return task;
        }
    }
}
