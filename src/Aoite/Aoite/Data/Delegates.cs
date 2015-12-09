using System;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 表示 <see cref="IDbExecutor.ToReader(ExecuteReaderHandler)"/> 的委托。
    /// </summary>
    /// <param name="reader">数据读取器。</param>
    public delegate void ExecuteReaderHandler(DbDataReader reader);

    /// <summary>
    /// 表示 <see cref="IDbExecutor.ToReader{TResultValue}(ExecuteReaderHandler{TResultValue})"/> 的委托。
    /// </summary>
    /// <typeparam name="TResultValue">返回值的类型。</typeparam>
    /// <param name="reader">数据读取器。</param>
    /// <returns>操作结果的值。</returns>
    public delegate TResultValue ExecuteReaderHandler<TResultValue>(DbDataReader reader);

    /// <summary>
    /// 数据源查询与交互执行前发生的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void ExecutingEventHandler(object sender, ExecutingEventArgs e);
    /// <summary>
    /// 数据源查询与交互执行前发生的事件参数。
    /// </summary>
    public class ExecutingEventArgs : EventArgs
    {
        internal ExecutingEventArgs(ExecuteCommand command, DbCommand dbCommand)
        {
            this.Command = command;
            this.DbCommand = dbCommand;
        }

        /// <summary>
        /// 获取一个值，表示执行命令。
        /// </summary>
        public ExecuteCommand Command { get; }

        /// <summary>
        /// 获取一个值，表示执行的 <see cref="DbCommand"/>。
        /// </summary>
        public DbCommand DbCommand { get; }

        /// <summary>
        /// 获取一个值，表示执行查询的操作类型。
        /// </summary>
        public ExecuteType ExecuteType { get; internal set; }
    }

    /// <summary>
    /// 数据源查询与交互完成后的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void ExecutedEventHandler(object sender, ExecutedEventArgs e);

    /// <summary>
    /// 表示数据源查询与交互完成后的事件参数。
    /// </summary>
    public sealed class ExecutedEventArgs : ExecutingEventArgs
    {
        internal ExecutedEventArgs(ExecuteCommand command, DbCommand dbCommand) : base(command, dbCommand) { }

        /// <summary>
        /// 获取一个值，表示数据源的返回值结果。
        /// </summary>
        public object Result { get; internal set; }
    }
}
