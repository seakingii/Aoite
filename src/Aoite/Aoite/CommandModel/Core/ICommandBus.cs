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
        /// <param name="executeing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        /// <returns>命令模型。</returns>
        TCommand Execute<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> executeing = null
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
    }
}
