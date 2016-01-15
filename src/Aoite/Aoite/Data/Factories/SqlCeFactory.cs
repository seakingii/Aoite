using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Data.Factories
{
    /// <summary>
    /// 表示一个基于 Microsoft SQL Server Compact 数据源命令生成工厂。
    /// </summary>
    public class SqlCeFactory : SqlFactory
    {
        /// <summary>
        /// 获取数据库命令生成工厂的唯一实例。
        /// </summary>
        public readonly static new SqlCeFactory Instance = new SqlCeFactory();

        /// <summary>
        /// 初始化一个 <see cref="SqlCeFactory"/> 类的新实例。
        /// </summary>
        protected SqlCeFactory() { }

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
        public override void PageProcessCommand(int pageNumber, int pageSize, DbCommand command)
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
