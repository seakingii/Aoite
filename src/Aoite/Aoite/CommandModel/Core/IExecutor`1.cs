namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令模型的执行器。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    public interface IExecutor<TCommand> : IExecutor where TCommand : ICommand
    {
        /// <summary>
        /// 执行一个命令模型。
        /// </summary>
        /// <param name="context">命令模型的上下文。</param>
        /// <param name="command">命令模型。</param>
        void Execute(IContext context, TCommand command);
    }

    /// <summary>
    /// 表示一个命令模型的执行器的基类。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    /// <typeparam name="TResult">返回值的数据类型。</typeparam>
    public abstract class ExecutorBase<TCommand, TResult> : IExecutor<TCommand> where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// 初始化一个 <see cref="ExecutorBase{TCommand, TResult}"/> 类的新实例。
        /// </summary>
        protected ExecutorBase() { }

        /// <summary>
        /// 执行一个命令模型。
        /// </summary>
        /// <param name="context">命令模型的上下文。</param>
        /// <param name="command">命令模型。</param>
        public virtual void Execute(IContext context, TCommand command)
        {
            command.Result = this.ExecuteResult(context, command);
        }

        /// <summary>
        /// 执行一个命令模型。
        /// </summary>
        /// <param name="context">命令模型的上下文。</param>
        /// <param name="command">命令模型。</param>
        /// <returns>一个执行的结果值。</returns>
        protected abstract TResult ExecuteResult(IContext context, TCommand command);

    }
}
