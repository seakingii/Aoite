using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源查询与交互引擎的提供程序名称。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DbProvidersAttribute : Attribute
    {
        /// <summary>
        /// 获取提供程序名称的集合。
        /// </summary>
        public string[] Names { get; }

        /// <summary>
        /// 获取提供程序名称说明。
        /// </summary>
        public string Type => Names.FirstOrDefault();

        /// <summary>
        /// 初始化一个 <see cref="DbProvidersAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="names">提供程序名称的集合。</param>
        public DbProvidersAttribute(params string[] names)
        {
            if(names == null) throw new ArgumentNullException(nameof(names));
            this.Names = names ?? new string[0];
        }
    }
}
