using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.DI
{
    /// <summary>
    /// 表示服务的生命周期。
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// 表示短暂服务。每一次调用都创建一个新的服务。
        /// </summary>
        Transient,
        /// <summary>
        /// 表示范围服务。每一个线程（或 Web 请求）第一次调用时才会创建服务。
        /// </summary>
        Scoped,
        /// <summary>
        /// 表示单例服务。只有当第一次调用时才会创建服务。
        /// </summary>
        Singleton,
        /// <summary>
        /// 表示后期绑定服务。
        /// </summary>
        LastMapping
    }
}
