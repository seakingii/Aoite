using System;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令总线。
    /// </summary>
    public interface ICommandBus : IContainerProvider
    {
        /// <summary>
        /// 执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="executing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        /// <returns>命令模型。</returns>
        TCommand Execute<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand;
        /// <summary>
        /// 以异步的方式执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="executing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        /// <returns>异步操作。</returns>
        System.Threading.Tasks.Task<TCommand> ExecuteAsync<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand;

        /// <summary>
        /// 获取一个命令总线的上下文管道。
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        ICommandBusPipe Pipe { get; }
        /// <summary>
        /// 获取一个命令总线的上下文管道，并开始事务模式。
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        ICommandBusPipe PipeTransaction { get; }
        /// <summary>
        /// 获取一个值，指示当前命令总线的上下文管道是否已创建。
        /// </summary>
        bool IsPipeCreated { get; }
    }

    /// <summary>
    /// 定义一个命令模型总线的管道。
    /// </summary>
    public interface ICommandBusPipe : ICommandBus, IObjectDisposable
    {
        /// <summary>
        /// 指定事务的隔离级别，并打开数据源连接（如果没有打开）。
        /// </summary>
        /// <param name="isolationLevel">指定事务的隔离级别。</param>
        /// <returns>总线管道。</returns>
        ICommandBusPipe UseTransaction(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.Unspecified);
        /// <summary>
        /// 提交数据库事务。
        /// </summary>
        void Commit();
        /// <summary>
        /// 从挂起状态回滚事务。
        /// </summary>
        void Rollback();
    }
}
