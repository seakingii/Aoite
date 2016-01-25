using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.DependencyInjection
{
    public class CallSitesTests
    {
        [Fact]
        public void TransientCallSite_Test()
        {
            int x = 1;
            var callSite = new TransientCallSite(() => lmps => x++);

            var value1 = callSite.Invoke();
            Assert.Equal(1, value1);
            var value2 = callSite.Invoke();
            Assert.Equal(2, value2);
            Assert.Equal(3, x);
            Assert.Equal(ServiceLifetime.Transient, callSite.Lifetime);
        }

        [Fact]
        public void ScopedCallSite_Test()
        {
            int x = 1;
            var callSite = new ScopedCallSite(() => lmps => x++);

            var t = new Threading.Thread(() =>
            {
                Assert.Equal(1, callSite.Invoke());
                Assert.Equal(1, callSite.Invoke());
            });
            t.Start();
            t.Join();
            var t2 = new Threading.Thread(() =>
            {
                Assert.Equal(2, callSite.Invoke());
                Assert.Equal(2, callSite.Invoke());
            });
            t2.Start();
            t2.Join();

            Assert.Equal(3, x);
            Assert.Equal(ServiceLifetime.Scoped, callSite.Lifetime);
        }

        [Fact]
        public void SingletonCallSite_Test()
        {
            int x = 1;
            var callSite = new SingletonCallSite(() => lmps => x++);

            var t = new Threading.Thread(() =>
            {
                Assert.Equal(1, callSite.Invoke());
                Assert.Equal(1, callSite.Invoke());
            });
            t.Start();
            t.Join();
            var t2 = new Threading.Thread(() =>
            {
                Assert.Equal(1, callSite.Invoke());
                Assert.Equal(1, callSite.Invoke());
            });
            t2.Start();
            t2.Join();

            Assert.Equal(2, x);
            Assert.Equal(ServiceLifetime.Singleton, callSite.Lifetime);
        }

        class LMCS_Class
        {
            public LMCS_Class(string name) { }
        }
        [Fact]
        public void LastMappingCallSite_Test()
        {
            var callSite = new LastMappingCallSite(null, null, 1);
            Assert.Equal(2, callSite.Invoke(new object[] { "1", 2 }));
            Assert.Equal(ServiceLifetime.LastMapping, callSite.Lifetime);

            var t = typeof(LMCS_Class);
            callSite = new LastMappingCallSite(t, t.GetConstructors()[0].GetParameters()[0], 1);

            Assert.Throws<ArgumentException>(() => callSite.Invoke(null));
            Assert.Throws<ArgumentException>(() => callSite.Invoke(new object[] { "1" }));
        }
    }
}
