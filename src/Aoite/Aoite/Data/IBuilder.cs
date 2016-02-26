namespace Aoite.Data
{
    /// <summary>
    /// 定义一个 SQL 语句生成的实现。
    /// </summary>
    public interface IBuilder
    {
        /// <summary>
        /// 获取查询命令的 Transact-SQL 查询字符串。
        /// </summary>
        string Text { get; }
        /// <summary>
        /// 获取查询命令的参数的键值集合。
        /// </summary>
        ExecuteParameterCollection Parameters { get; }
        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <returns>执行数据源查询与交互的执行器。</returns>
        ExecuteCommand End();
        /// <summary>
        /// 编译并执行当前 SQL 语句生成。
        /// </summary>
        /// <returns>数据源查询与交互的执行器。</returns>
        IDbExecutor Execute();
        /// <summary>
        /// 添加 ORDER BY 的字段。
        /// </summary>
        /// <param name="fields">字段的集合。</param>
        /// <returns> <see cref="ISelect"/> 的实例。</returns>
        ISelect OrderBy(params string[] fields);
        /// <summary>
        /// 添加 ORDER BY 倒序的字段。
        /// </summary>
        /// <param name="fields">字段的集合。</param>
        /// <returns> <see cref="ISelect"/> 的实例。</returns>
        ISelect OrderByDescending(params string[] fields);
        /// <summary>
        /// 添加 GROUP BY 的字段。
        /// </summary>
        /// <param name="fields">字段的集合。</param>
        /// <returns> <see cref="ISelect"/> 的实例。</returns>
        ISelect GroupBy(params string[] fields);
    }
}
