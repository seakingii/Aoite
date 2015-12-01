using System;
using System.Collections.Generic;
using Xunit;

namespace Aoite.Redis
{
    public class RedisTranTests : TestBase
    {
        [Fact()]
        public void MultiTest()
        {
            var okReply = "+OK\r\n";
            using(var mock = new MockConnector("localhost", 9999, okReply, okReply))
            using(var redis = new RedisClient(mock))
            {
                using(var tran = redis.BeginTransaction())
                {
                    Assert.Equal("*1\r\n$5\r\nMULTI\r\n", mock.GetMessage());
                }
            }
        }
        [Fact()]
        public void ExecTests()
        {
            var okReply = "+OK\r\n";
            using(var mock = new MockConnector("localhost", 9999, okReply, "+QUEUED\r\n", "+QUEUED\r\n", "*2\r\n" + okReply + okReply))
            using(var redis = new RedisClient(mock))
            {
                int x = 0;
                using(var tran = redis.BeginTransaction())
                {
                    Assert.Equal("*1\r\n$5\r\nMULTI\r\n", mock.GetMessage());
                    tran.On(tran.Set("key1", "value1"), r =>
                    {
                        Assert.True(r);
                        x += 1;
                    });
                    Assert.Equal("*3\r\n$3\r\nSET\r\n$4\r\nkey1\r\n$6\r\nvalue1\r\n", mock.GetMessage());
                    tran.On(tran.Set("key2", "value2"), r =>
                    {
                        Assert.True(r);
                        Assert.Equal(1, x);
                        x += 2;
                    });
                    Assert.Equal("*3\r\n$3\r\nSET\r\n$4\r\nkey2\r\n$6\r\nvalue2\r\n", mock.GetMessage());
                    tran.Commit();
                    Assert.Equal("*1\r\n$4\r\nEXEC\r\n", mock.GetMessage());
                }
                Assert.Equal(string.Empty, mock.GetMessage());
                Assert.Equal(3, x);
            }

            this.RealCall(redis =>
            {
                int x = 0;
                using(var tran = redis.BeginTransaction())
                {
                    tran.On(tran.Set("key1", "value1"), r =>
                    {
                        Assert.True(r);
                        x += 1;
                    });
                    tran.On(tran.Set("key2", "value2"), r =>
                    {
                        Assert.True(r);
                        Assert.Equal(1, x);
                        x += 2;
                    });
                    tran.Commit();
                }
                Assert.Equal(3, x);

                Assert.Equal("value1", (string)redis.Get("key1"));
                Assert.Equal("value2", (string)redis.Get("key2"));
            });
        }

        [Fact()]
        public void RollbackTests()
        {
            var okReply = "+OK\r\n";
            using(var mock = new MockConnector("localhost", 9999, okReply, "+QUEUED\r\n", "+QUEUED\r\n", "*2\r\n" + okReply + okReply))
            using(var redis = new RedisClient(mock))
            {
                int x = 0;
                using(var tran = redis.BeginTransaction())
                {
                    Assert.Equal("*1\r\n$5\r\nMULTI\r\n", mock.GetMessage());
                    tran.On(tran.Set("key1", "value1"), r =>
                    {
                        Assert.True(r);
                        x += 1;
                    });
                    Assert.Equal("*3\r\n$3\r\nSET\r\n$4\r\nkey1\r\n$6\r\nvalue1\r\n", mock.GetMessage());
                    tran.On(tran.Set("key2", "value2"), r =>
                    {
                        Assert.True(r);
                        Assert.Equal(1, x);
                        x += 2;
                    });
                    Assert.Equal("*3\r\n$3\r\nSET\r\n$4\r\nkey2\r\n$6\r\nvalue2\r\n", mock.GetMessage());
                }
                Assert.Equal("*1\r\n$7\r\nDISCARD\r\n", mock.GetMessage());
                Assert.Equal(0, x);
            }

            this.RealCall(redis =>
            {
                int x = 0;
                using(var tran = redis.BeginTransaction())
                {
                    tran.On(tran.Set("key1", "value1"), r =>
                    {
                        Assert.True(r);
                        x += 1;
                    });
                    tran.On(tran.Set("key2", "value2"), r =>
                    {
                        Assert.True(r);
                        Assert.Equal(1, x);
                        x += 2;
                    });
                }
                Assert.Equal(0, x);

                Assert.Null(redis.Get("key1"));
                Assert.Null(redis.Get("key2"));
            });
        }

        [Fact()]
        public void TranOutErrorTests()
        {
            var okReply = "+OK\r\n";
            using(var mock = new MockConnector("localhost", 9999, okReply, "+QUEUED\r\n", "+QUEUED\r\n", "*2\r\n" + okReply + "-ERR Operation against a key holding the wrong kind of value\r\n", okReply))
            using(var redis = new RedisClient(mock))
            {
                int x = 0;
                using(var tran = redis.BeginTransaction())
                {
                    Assert.Equal("*1\r\n$5\r\nMULTI\r\n", mock.GetMessage());
                    tran.On(tran.Set("key1", "value1"), r =>
                    {
                        Assert.True(r);
                        x += 1;
                    });
                    Assert.Equal("*3\r\n$3\r\nSET\r\n$4\r\nkey1\r\n$6\r\nvalue1\r\n", mock.GetMessage());
                    tran.On(tran.IncrBy("key1"), r =>
                    {
                        x += 2;
                    });
                    Assert.Equal("*2\r\n$4\r\nINCR\r\n$4\r\nkey1\r\n", mock.GetMessage());
                    Assert.Throws<RedisReplyException>(() => tran.Commit());
                    Assert.Equal("*1\r\n$4\r\nEXEC\r\n", mock.GetMessage());
                }
                Assert.Equal("*1\r\n$7\r\nDISCARD\r\n", mock.GetMessage());
                Assert.Equal(0, x);
            }
            this.RealCall(redis =>
            {
                using(var tran = redis.BeginTransaction())
                {
                    tran.On(tran.Set("key1", "value1"), r =>
                    {
                        Assert.True(r);
                    });
                    tran.IncrBy("key1");
                    Assert.Throws<RedisReplyException>(() => tran.Commit());
                }
            });
        }
    }
}
