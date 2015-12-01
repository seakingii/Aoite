using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 基于 MSSQL CE 的分页。
    /// </summary>
    public class MsSqlCePaginationProcess : MsSqlPaginationProcess
    {
        /// <summary>
        /// 分页实例。
        /// </summary>
        new public static readonly MsSqlCePaginationProcess Instance = new MsSqlCePaginationProcess();
        
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.MsSqlCePaginationProcess"/> 类的新实例。
        /// </summary>
        public MsSqlCePaginationProcess() { }
        /// <summary>
        /// 获取分页的字符串格式项。
        /// </summary>
        protected override string PageFormat
        {
            get
            {
                return @"{0} {3} OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY";
            }
        }

        /// <summary>
        /// 对指定的 <see cref="DbCommand"/> 进行分页处理。
        /// </summary>
        /// <param name="pageNumber">从 1 开始的页码。</param>
        /// <param name="pageSize">页的大小。</param>
        /// <param name="command">数据源查询命令</param>
        public override void ProcessCommand(int pageNumber, int pageSize, DbCommand command)
        {
            var start = (pageNumber - 1) * pageSize;
            var match = GetOrderByMatch(command.CommandText);
            var orderBy = "ORDER BY GETDATE()";
            if(match.Success)
            {
                command.CommandText = command.CommandText.Remove(match.Index);
                orderBy = match.Value.Trim();
            }

            command.CommandText = string.Format(PageFormat
                , command.CommandText
                , start
                , pageSize
                , orderBy);
        }
    }
}
