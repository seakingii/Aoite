using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.DI
{
    public class TypeServiceBinderTests
    {
        readonly IocContainer _locator;
        readonly ServiceBuilder _builder;
        public TypeServiceBinderTests()
        {
            this._locator = new IocContainer();
            this._builder = new ServiceBuilder(this._locator);
        }

        private TypeServiceBinder Create(Type expectType, bool overwrite)
        {
            return new TypeServiceBinder(this._locator, this._builder, expectType, overwrite);
        }

        [Fact]
        public void Create_Test()
        {
            var binder = this.Create(Types.String, true);
            Assert.Equal(Types.String, binder.ExpectType);
            Assert.True(binder.Overwrite);
        }

        [Fact]
        public void As_Test()
        {
            var binder = this.Create(typeof(IService1), true);
            binder.As(typeof(Service1));
            var callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Transient, callSite.Lifetime);
            Assert.IsType<Service1>(callSite.Invoke());
        }

        [Fact]
        public void Scoped_Test()
        {
            var binder = this.Create(typeof(IService1), true);
            binder.Scoped();
            var callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Scoped, callSite.Lifetime);
            Assert.IsType<Service1>(callSite.Invoke());

            binder.Scoped(typeof(Service1_2));
            callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Scoped, callSite.Lifetime);
            Assert.IsType<Service1_2>(callSite.Invoke());

            binder.Scoped(lms => new Service1());
            callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Scoped, callSite.Lifetime);
            Assert.IsType<Service1>(callSite.Invoke());
        }

        [Fact]
        public void Transient_Test()
        {
            var binder = this.Create(typeof(IService1), true);
            binder.Transient();
            var callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Transient, callSite.Lifetime);
            Assert.IsType<Service1>(callSite.Invoke());

            binder.Transient(typeof(Service1_2));
            callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Transient, callSite.Lifetime);
            Assert.IsType<Service1_2>(callSite.Invoke());

            binder.Transient(lms => new Service1());
            callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Transient, callSite.Lifetime);
            Assert.IsType<Service1>(callSite.Invoke());
        }

        [Fact]
        public void Singleton_Test()
        {
            var binder = this.Create(typeof(IService1), true);
            binder.Singleton();
            var callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Singleton, callSite.Lifetime);
            Assert.IsType<Service1>(callSite.Invoke());

            binder.Singleton(typeof(Service1_2));
            callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Singleton, callSite.Lifetime);
            Assert.IsType<Service1_2>(callSite.Invoke());

            binder.Singleton(lms => new Service1());
            callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Singleton, callSite.Lifetime);
            Assert.IsType<Service1>(callSite.Invoke());
        }

        [Fact]
        public void Instance_Test()
        {
            var binder = this.Create(typeof(IService1), true);
            binder.Singleton(new Service1());
            var callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Singleton, callSite.Lifetime);
            Assert.IsType<Service1>(callSite.Invoke());

            binder.Singleton(new Service1_2());
            callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Singleton, callSite.Lifetime);
            Assert.IsType<Service1_2>(callSite.Invoke());
        }
    }
}
