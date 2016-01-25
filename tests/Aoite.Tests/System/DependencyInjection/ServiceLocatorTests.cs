using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.DependencyInjection
{
    public class ServiceLocatorTests
    {
        [Fact]
        public void Create_Test()
        {
            var locator = new ServiceLocator();
            Assert.Null(locator.Parent);
            Assert.False(locator.DisabledAutoResolving);
        }

        [Fact]
        public void FindActualType_Test()
        {
            var locator = new ServiceLocator();
            Assert.Equal(typeof(Service2_2), locator.FindActualType(typeof(IService2)));
            Assert.Null(locator.FindActualType(Types.Int32));

            Assert.Equal(typeof(Service2_2), locator.FindActualType(typeof(Service2_2)));

            Assert.Equal(typeof(Service1), locator.FindActualType(typeof(IService1)));
        }

        private ServiceLocator Create()
        {
            var locator = new ServiceLocator();
            locator.Imports(build => build
                                        .UseRange<IService1>().As<Service1>()
                                        .UseRange<IService1>().As<Service1_2>()
                                        .Use<IService2>().Singleton(lmps => new Service2_2())
                                        .Use("a").Instance(1)
                                        .Use<IService1>("a").Instance(2));
            return locator;
        }

        [Fact]
        public void Imports_Test()
        {
            var locator = this.Create();

            Assert.IsType<Service1_2>(locator.Get<IService1>());
            var services = locator.GetAll<IService1>();
            Assert.IsType<Service1_2>(services[0]);
            Assert.IsType<Service1>(services[1]);
            Assert.IsType<Service2_2>(locator.Get<IService2>());

            Assert.Equal(1, locator.Get("a"));
            Assert.Equal(2, locator.Get<IService1>("a"));
        }

        [Fact]
        public void GetFixed_Test()
        {
            var locator = this.Create();
            locator.Remove<IService1>();

            Assert.False(locator.Contains<IService1>());
            Assert.Null(locator.GetFixed<IService1>());
            Assert.IsType<Service1>(locator.Get<IService1>());
            Assert.True(locator.Contains<IService1>());
            Assert.IsType<Service1>(locator.GetFixed<IService1>());

            Assert.True(locator.Contains<IService1>("a"));
            Assert.True(locator.Contains("a"));

            Assert.False(locator.Contains<IService1>("b"));
            Assert.False(locator.Contains("b"));

            locator.Remove<IService1>("a");
            Assert.False(locator.Contains<IService1>("a"));

            locator.Remove("a");
            Assert.False(locator.Contains("a"));
        }
    }
}
