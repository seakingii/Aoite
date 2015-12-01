using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个分页的处理。
    /// </summary>
    public interface IPaginationProcess
    {
        /// <summary>
        /// 创建指定查询字符串的统计总行数查询字符串。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        string CreateTotalCountCommand(string commandText);

        /// <summary>
        /// 对指定的 <see cref="DbCommand"/> 进行分页处理。
        /// </summary>
        /// <param name="pageNumber">从 1 开始的页码。</param>
        /// <param name="pageSize">页的大小。</param>
        /// <param name="command">数据源查询命令</param>
        void ProcessCommand(int pageNumber, int pageSize, DbCommand command);
    }
}