using System;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 命令模型执行的方法。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    /// <param name="context">执行的上下文。</param>
    /// <param name="command">执行的命令模型。</param>
    public delegate void CommandExecuteHandler<TCommand>(IContext context, TCommand command) where TCommand : ICommand;
    /// <summary>
    /// 命令模型执行前发生的方法。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    /// <param name="context">执行的上下文。</param>
    /// <param name="command">执行的命令模型。</param>
    /// <returns>一个值，指示是否继续执行命令。</returns>
    public delegate bool CommandExecutingHandler<TCommand>(IContext context, TCommand command) where TCommand : ICommand;
    /// <summary>
    /// 命令模型执行后发生的方法。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    /// <param name="context">执行的上下文。</param>
    /// <param name="command">执行的命令模型。</param>
    /// <param name="exception">抛出的异常。</param>
    public delegate void CommandExecutedHandler<TCommand>(IContext context, TCommand command, Exception exception) where TCommand : ICommand;
}
