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
        /// <returns>返回命令模型。</returns>
        public virtual TCommand Execute<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand
        {
            if(command == null) throw new ArgumentNullException(nameof(command));

            var contextFactory = this.Container.GetService<IContextFactory>();
            var executorFactory = this.Container.GetService<IExecutorFactory>();
            var eventStore = this.Container.GetService<IEventStore>();

            var context = contextFactory.Create<TCommand>(command);
            var executorMetadata = executorFactory.Create<TCommand>(command);
            var executor = executorMetadata.Executor;

            // 执行器元数据 && 事务仓储
            if(executorMetadata.RaiseExecuting(context, command)
                && eventStore.RaiseExecuting<TCommand>(context, command)
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

                    throw;
                }
            }

            if(executed != null) executed(context, command, null);
            eventStore.RaiseExecuted<TCommand>(context, command, null);
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
        /// <returns>返回一个异步操作。</returns>
        public virtual Task<TCommand> ExecuteAsync<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand
        {
            //- https://github.com/StephenCleary/AspNetBackgroundTasks/blob/master/src/AspNetBackgroundTasks/Internal/RegisteredTasks.cs
            //TODO: 如何解决升级维护时，异步任务丢失？IRegisteredObject？
            if(command == null) throw new ArgumentNullException(nameof(command));

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    return this.Execute(command, executing, executed);
                }
                finally
                {
                    GA.ResetContexts();//- 这里开了一个线程，线程结束后就会释放所有上下文。
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            });

        }
    }
}
