using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Dbx
{
    /// <summary>
    /// 定义数据源查询与交互引擎的提供程序。
    /// </summary>
    public interface IEngineProvider
    {
        /// <summary>
        /// 获取用于创建提供程序对数据源类的实现的实例。
        /// </summary>
        DbProviderFactory DbFactory { get; }
        /// <summary>
        /// 获取一个值，表示当前数据提供程序的名称。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 转义指定位置的名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="point">名称的位置。</param>
        /// <returns>返回转义后的名称。</returns>
        string EscapeName(string name, NamePoint point);
    }
}
