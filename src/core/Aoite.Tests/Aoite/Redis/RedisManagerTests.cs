using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Redis
{
    public class RedisManagerTests
    {
        [Fact()]
        public void SimpleTest()
        {
            Assert.False(RedisManager.IsThreadContext);
            Assert.NotNull(RedisManager.Context);
            Assert.Equal(1, RedisManager.Context.Id);
            Assert.True(RedisManager.IsThreadContext);
            Assert.Equal(1, RedisManager.Context.Id);
            RedisManager.Reset();
            Assert.False(RedisManager.IsThreadContext);
        }

        [Fact()]
        public void MultiThreadsTest()
        {
            List<long> idList = new List<long>();
            List<IAsyncJob> jobList = new List<IAsyncJob>();
            for(int i = 0; i < 10; i++)
            {
                jobList.Add(Ajob.Once(job =>
                {
                    Assert.NotNull(RedisManager.Context);
                    idList.Add(RedisManager.Context.Id);
                    RedisManager.Reset();
                }));
            }
            Ajob.WaitAll(jobList);
            Assert.Equal(10, idList.Count);
            Assert.Equal(55, idList.Sum());
            Assert.Equal(11, RedisManager.Context.Id);
            RedisManager.Reset();
        }
    }
}
