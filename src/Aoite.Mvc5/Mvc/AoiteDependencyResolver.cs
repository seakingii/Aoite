using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// 表示一个依赖关系解析程序。
    /// </summary>
    public class AoiteDependencyResolver : IDependencyResolver
    {
        private IIocContainer _container;

        /// <summary>
        /// 提供服务容器，初始化一个 <see cref="AoiteDependencyResolver"/> 类的新实例。
        /// </summary>
        /// <param name="container"></param>
        public AoiteDependencyResolver(IIocContainer container)
        {
            if(container == null) throw new ArgumentNullException(nameof(container));
            this._container = container;
        }

        /// <summary>
        /// 解析支持任意对象创建的一次注册的服务。
        /// </summary>
        /// <param name="serviceType">所请求的服务或对象的类型。</param>
        /// <returns>请求的服务或对象。</returns>
        public virtual object GetService(Type serviceType)
        {
            return this._container.Get(serviceType);
        }

        /// <summary>
        /// 解析多次注册的服务。
        /// </summary>
        /// <param name="serviceType">所请求的服务或对象的类型。</param>
        /// <returns></returns>
        public virtual IEnumerable<object> GetServices(Type serviceType)
        {
            return this._container.GetAll(serviceType);
        }
    }
}
