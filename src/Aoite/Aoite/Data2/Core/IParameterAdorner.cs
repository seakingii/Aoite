using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 定义数据参数的装饰。
    /// </summary>
    public interface IParameterAdorner
    {
        /// <summary>
        /// 提供数据源命令和查询参数，渲染一个数据源参数。
        /// </summary>
        /// <param name="command">数据源命令。</param>
        /// <param name="executeParameter">查询参数。</param>
        /// <returns>一个数据源参数。</returns>
        DbParameter Render(DbCommand command, ExecuteParameter executeParameter);
    }
}
