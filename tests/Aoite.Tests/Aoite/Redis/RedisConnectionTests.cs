using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Redis
{
    public class RedisConnectionTests
    {
        [Fact()]
        public void AuthTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n", "+OK\r\n"))
            using(var redis = new RedisClient(mock, "my password"))
            {
                redis._connector.Connect();
                Assert.Equal("*2\r\n$4\r\nAUTH\r\n$11\r\nmy password\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void PingTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+PONG\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.Ping());
                Assert.Equal("*1\r\n$4\r\nPING\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void QuitTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.Quit());
                Assert.Equal("*1\r\n$4\r\nQUIT\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void SelectTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True( redis.Select(2));
                Assert.Equal("*2\r\n$6\r\nSELECT\r\n$1\r\n2\r\n", mock.GetMessage());
            }
        }
    }
}
