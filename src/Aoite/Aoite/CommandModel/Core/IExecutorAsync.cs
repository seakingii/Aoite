#if !NET40
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.CommandModel
{

    /// <summary>
    /// 定义一个异步命令模型的执行器。
    /// </summary>
    public interface IExecutorAsync : IExecutor { }

    /// <summary>
    /// 定义一个异步命令模型的执行器。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    public interface IExecutorAsync<TCommand> : IExecutorAsync, IExecutor<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// 异步执行一个命令模型。
        /// </summary>
        /// <param name="context">命令模型的上下文。</param>
        /// <param name="command">命令模型。</param>
        /// <returns>异步操作。</returns>
        Task<TCommand> ExecuteAsync(IContext context, TCommand command);
    }

    /// <summary>
    /// 表示一个异步命令模型的执行器的基类。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    /// <typeparam name="TResult">返回值的数据类型。</typeparam>
    public abstract class ExecutorAsyncBase<TCommand, TResult> : ExecutorBase<TCommand, TResult>, IExecutorAsync<TCommand> where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// 初始化一个 <see cref="ExecutorAsyncBase{TCommand, TResult}"/> 类的新实例。
        /// </summary>
        protected ExecutorAsyncBase() { }

        /// <summary>
        /// 异步执行一个命令模型。
        /// </summary>
        /// <param name="context">命令模型的上下文。</param>
        /// <param name="command">命令模型。</param>
        /// <returns>异步操作。</returns>
        public virtual Task<TCommand> ExecuteAsync(IContext context, TCommand command)
        {
            var task = this.ExecuteResultAsync(context, command);

            return task.ContinueWith(t =>
            {
                command.Result = t.Result;
                return command;
            });
        }

        /// <summary>
        /// 异步执行一个命令模型。
        /// </summary>
        /// <param name="context">命令模型的上下文。</param>
        /// <param name="command">命令模型。</param>
        /// <returns>异步操作。</returns>
        protected abstract Task<TResult> ExecuteResultAsync(IContext context, TCommand command);
    }
}
#endif