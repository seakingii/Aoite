using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.DI
{
    public class ValueServiceBinderTests
    {
        readonly IocContainer _locator;
        readonly ServiceBuilder _builder;
        public ValueServiceBinderTests()
        {
            this._locator = new IocContainer();
            this._builder = new ServiceBuilder(this._locator);
        }

        private ValueServiceBinder Create(Type expectType, string name)
        {
            return new ValueServiceBinder(this._locator, this._builder, expectType, name);
        }

        [Fact]
        public void Create_Test()
        {
            var binder = this.Create(Types.String, "abc");
            Assert.Equal(Types.String, binder.ExpectType);
            Assert.Equal("abc", binder.Name);
        }


        [Fact]
        public void Scoped_Test()
        {
            var binder = this.Create(typeof(IService1), "abc");

            binder.Scoped(lms => 99);
            var callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Scoped, callSite.Lifetime);
            Assert.Equal(99, callSite.Invoke());
        }

        [Fact]
        public void Transient_Test()
        {
            var binder = this.Create(typeof(IService1), "abc");

            binder.Transient(lms => 99);
            var callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Transient, callSite.Lifetime);
            Assert.Equal(99, callSite.Invoke());
        }

        [Fact]
        public void Singleton_Test()
        {
            var binder = this.Create(typeof(IService1), "abc");

            binder.Singleton(lms => 99);
            var callSite = binder.CallSite;
            Assert.Equal(ServiceLifetime.Singleton, callSite.Lifetime);
            Assert.Equal(99, callSite.Invoke());
        }

        [Fact]
        public void Instance_Test()
        {
            var binder = this.Create(typeof(IService1), "abc");
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
