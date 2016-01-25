using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DependencyInjection
{
    public interface IService1 { }
    public class Service1 : IService1 { }
    public class Service1_2 : IService1 { }

    [DefaultMapping(typeof(Service2_2))]
    public interface IService2 { }
    public class Service2_2: IService2 { }
}
