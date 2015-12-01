using System;
using System.Collections.Generic;
using System.Linq;
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
            Assert.True(RedisManager.Context.Id > 0);
            Assert.True(RedisManager.IsThreadContext);
            Assert.True(RedisManager.Context.Id > 0);
            RedisManager.ResetContext();
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
                    RedisManager.ResetContext();
                }));
            }
            Ajob.WaitAll(jobList);
            Assert.Equal(10, idList.Count);
            Assert.True(idList.Sum() >= 55);
            Assert.True(RedisManager.Context.Id >= 11);
            RedisManager.ResetContext();
        }
    }
}
