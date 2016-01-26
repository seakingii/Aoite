using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.DI
{
    public class ServiceBuilderTests
    {
        [Fact]
        public void Create_Test()
        {
            var locator = new IocContainer();
            using(var builder = new ServiceBuilder(locator))
            {
                builder.Promote();
                Assert.True(builder.IsPromote);
            }
        }

        [Fact]
        public void Add_Test()
        {
            var locator = new IocContainer();
            using(var builder = new ServiceBuilder(locator))
            {
                var binder = builder.UseRange(Types.String);
                Assert.IsType<TypeServiceBinder>(binder);
                Assert.False(binder.Overwrite);
                Assert.Equal(Types.String, binder.ExpectType);
                Assert.Equal(binder, builder.TypeBinders.Value[0]);
            }
        }

        [Fact]
        public void Set_Test()
        {
            var locator = new IocContainer();
            using(var builder = new ServiceBuilder(locator))
            {
                var binder = builder.Use(Types.String);
                Assert.IsType<TypeServiceBinder>(binder);
                Assert.True(binder.Overwrite);
                Assert.Equal(Types.String, binder.ExpectType);
                Assert.Equal(binder, builder.TypeBinders.Value[0]);

                var binder2 = builder.Use("abc");
                Assert.IsType<ValueServiceBinder>(binder2);
                Assert.Equal("abc", binder2.Name);
                Assert.Null(binder2.ExpectType);
                Assert.Equal(binder2, builder.ValueBinders.Value[0]);


                var binder3 = builder.Use(Types.String, "abc");
                Assert.IsType<ValueServiceBinder>(binder3);
                Assert.Equal("abc", binder3.Name);
                Assert.Equal(Types.String, binder3.ExpectType);
                Assert.Equal(binder3, builder.ValueBinders.Value[1]);
            }
        }
    }
}
