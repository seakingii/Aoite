using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public class AoiteDependencyResolver : IDependencyResolver
    {
        private IIocContainer _container;
        public AoiteDependencyResolver(IIocContainer container)
        {
            if(container == null) throw new ArgumentNullException(nameof(container));
            this._container = container;
        }

        public virtual object GetService(Type serviceType)
        {
            return this._container.Get(serviceType);
        }

        public virtual IEnumerable<object> GetServices(Type serviceType)
        {
            return this._container.GetAll(serviceType);
        }
    }
}
