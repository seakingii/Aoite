namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个包含返回值的命令模型。
    /// </summary>
    /// <typeparam name="TResult">返回值的数据类型。</typeparam>
    public interface ICommand<TResult> : ICommand
    {
        /// <summary>
        /// 获取或设置命令模型的执行后的返回值。
        /// </summary>
        TResult Result { get; set; }
    }

    /// <summary>
    /// 表示一个包含返回值的命令模型基类。
    /// </summary>
    /// <typeparam name="TResult">返回值的数据类型。</typeparam>
    public abstract class CommandBase<TResult> : ICommand<TResult>
    {
        /// <summary>
        /// 获取或设置命令模型的执行后的返回值。
        /// </summary>
        public TResult Result { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CommandBase{TResultValue}"/> 类的新实例。
        /// </summary>
        public CommandBase() { }
    }
}
