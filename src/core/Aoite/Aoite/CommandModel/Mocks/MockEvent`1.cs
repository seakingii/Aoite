using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个模拟命令模型的事件。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    public class MockEvent<TCommand> : IEvent<TCommand> where TCommand : ICommand
    {
        CommandExecutingHandler<TCommand> _executing;
        CommandExecutedHandler<TCommand> _executed;

        /// <summary>
        /// 初始化 <see cref="Aoite.CommandModel.MockEvent&lt;TCommand&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="executing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        public MockEvent(CommandExecutingHandler<TCommand> executing, CommandExecutedHandler<TCommand> executed)
        {
            this._executing = executing;
            this._executed = executed;
        }

        /// <summary>
        /// 命令模型执行前发生的方法。
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        public virtual bool RaiseExecuting(IContext context, TCommand command)
        {
            if(this._executing != null) return this._executing(context, command);
            return true;
        }

        /// <summary>
        /// 命令模型执行后发生的方法。
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <param name="exception">抛出的异常。</param>
        public virtual void RaiseExecuted(IContext context, TCommand command, Exception exception)
        {
            var ev = this._executed;
            if(ev != null) ev(context, command, exception);
        }
    }
}
