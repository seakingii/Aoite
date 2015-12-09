namespace Aoite.Data
{
    /// <summary>
    /// 定义数据源查询与交互引擎的方法。
    /// </summary>
    public interface IDbEngine
    {
        /// <summary>
        /// 获取数据源查询与交互引擎的提供程序。
        /// </summary>
        IDbEngineProvider Provider { get; }
        /// <summary>
        /// 获取当前上下文所属的交互引擎。
        /// </summary>
        DbEngine Owner { get; }
        /// <summary>
        /// 执行指定的命令。
        /// </summary>
        /// <param name="command">执行的命令。</param>
        /// <returns>数据源查询与交互的执行器。</returns>
        IDbExecutor Execute(ExecuteCommand command);
    }
}
