using System;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个数据源交互的执行命令参数。
    /// </summary>
    public interface IExecuteParameter : ICloneable
    {
        /// <summary>
        /// 获取参数的名称。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 获取或设置参数的值。
        /// </summary>
        object Value { get; set; }
        /// <summary>
        /// 指定 <see cref="DbCommand"/>，生成一个 <see cref="DbParameter"/>。
        /// </summary>
        /// <param name="command">一个 <see cref="DbCommand"/>。</param>
        /// <returns>已生成的 <see cref="DbParameter"/>。</returns>
        DbParameter CreateParameter(DbCommand command);
    }
}
