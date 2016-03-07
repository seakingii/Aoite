using System;
using System.Collections.Generic;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型的事件。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    public class Event<TCommand> : IEvent<TCommand> where TCommand : ICommand
    {
        private List<CommandExecutingHandler<TCommand>> _ExecutingEvents = new List<CommandExecutingHandler<TCommand>>();
        /// <summary>
        /// 命令模型执行前发生的事件。事件在顺序广播时，遇到返回 false 则会中断广播。
        /// </summary>
        public event CommandExecutingHandler<TCommand> Executing
        {
            add { lock (_ExecutingEvents) _ExecutingEvents.Add(value); }
            remove { lock (_ExecutingEvents) _ExecutingEvents.Remove(value); }
        }

        /// <summary>
        /// 命令模型执行后发生的事件。
        /// </summary>
        public event CommandExecutedHandler<TCommand> Executed;

        /// <summary>
        /// 初始化一个 <see cref="Event{TCommand}"/> 类的新实例。
        /// </summary>
        public Event() { }

        /// <summary>
        /// 命令模型执行前发生的方法。
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        public virtual bool RaiseExecuting(IContext context, TCommand command)
        {
            lock (_ExecutingEvents)
            {
                foreach(var item in this._ExecutingEvents)
                {
                    if(!item(context, command)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 命令模型执行后发生的方法。
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <param name="exception">抛出的异常。</param>
        public virtual void RaiseExecuted(IContext context, TCommand command, Exception exception)
            => this.Executed?.Invoke(context, command, exception);
    }
}
