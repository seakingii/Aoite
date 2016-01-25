using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public class AoiteDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            return ObjectFactory.Context.Get(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            //ObjectFactory.Context.GetService
            throw new NotImplementedException();
        }
    }
}
