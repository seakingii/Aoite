using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Core
{
    public class LockeableTests
    {
        [Fact]
        public async void LockingTest()
        {
            List<Task> tasks = new List<Task>();
            var x = 0;
            for(int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(100 - i);
                    using(GA.Lock("x"))
                    {
                        if(x < 10) x++;
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Assert.Equal(10, x);

            var lockable = GA.Lock("x");

            await Task.Factory.StartNew(() =>
            {
                   Assert.Throws<TimeoutException>(() => GA.Lock("x", TimeSpan.FromMilliseconds(300)));
            });
            lockable.Dispose();
        }
        [Fact]
        public async void LockingAsyncTest()
        {
            List<Task> tasks = new List<Task>();
            var x = 0;
            for(int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    Thread.Sleep(100 - i);
                    using(await GA.LockAsync("x"))
                    {
                        if(x < 10) x++;
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Assert.Equal(10, x);

            var lockable = GA.Lock("x");

            await Task.Factory.StartNew(() =>
            {
                Assert.Throws<TimeoutException>(() => GA.Lock("x", TimeSpan.FromMilliseconds(300)));
            });
            lockable.Dispose();
        }
    }
}
