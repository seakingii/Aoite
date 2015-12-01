using Xunit;
using System;

namespace Aoite.Redis
{
    public class RedisScriptTests
    {
        [Fact()]
        public void EvalTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "*4\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$5\r\nfirst\r\n$6\r\nsecond\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.Eval("return {KEYS[1],KEYS[2],ARGV[1],ARGV[2]}", new RedisDictionary() { { "key1", "first" }, { "key2", "second" } });
                Assert.True(response is object[]);
                Assert.Equal(4, (response as object[]).Length);
                Assert.Equal("key1", (string)(BinaryValue)(response as object[])[0]);
                Assert.Equal("key2", (string)(BinaryValue)(response as object[])[1]);
                Assert.Equal("first", (string)(BinaryValue)(response as object[])[2]);
                Assert.Equal("second", (string)(BinaryValue)(response as object[])[3]);
                Assert.Equal("*7\r\n$4\r\nEVAL\r\n$40\r\nreturn {KEYS[1],KEYS[2],ARGV[1],ARGV[2]}\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$5\r\nfirst\r\n$6\r\nsecond\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void EvalSHATest()
        {
            using(var mock = new MockConnector("localhost", 9999, "*4\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$5\r\nfirst\r\n$6\r\nsecond\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.EvalSHA("checksum", new RedisDictionary() { { "key1", "first" }, { "key2", "second" } });
                Assert.True(response is object[]);
                Assert.Equal(4, (response as object[]).Length);
                Assert.Equal("key1", (string)(BinaryValue)(response as object[])[0]);
                Assert.Equal("key2", (string)(BinaryValue)(response as object[])[1]);
                Assert.Equal("first", (string)(BinaryValue)(response as object[])[2]);
                Assert.Equal("second", (string)(BinaryValue)(response as object[])[3]);
                Assert.Equal("*7\r\n$7\r\nEVALSHA\r\n$8\r\nchecksum\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$5\r\nfirst\r\n$6\r\nsecond\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ScriptExistsTests()
        {
            using(var mock = new MockConnector("localhost", 9999, "*2\r\n:1\r\n:0\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.ScriptExists("checksum1", "checksum2");
                Assert.Equal(2, response.Length);
                Assert.True(response[0]);
                Assert.False(response[1]);

                Assert.Equal("*4\r\n$6\r\nSCRIPT\r\n$6\r\nEXISTS\r\n$9\r\nchecksum1\r\n$9\r\nchecksum2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ScriptFlushTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.ScriptFlush());
                Assert.Equal("*2\r\n$6\r\nSCRIPT\r\n$5\r\nFLUSH\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ScriptKillTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.ScriptKill());
                Assert.Equal("*2\r\n$6\r\nSCRIPT\r\n$4\r\nKILL\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ScriptLoadTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "$8\r\nchecksum\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal("checksum", redis.ScriptLoad("return 1"));
                Assert.Equal("*3\r\n$6\r\nSCRIPT\r\n$4\r\nLOAD\r\n$8\r\nreturn 1\r\n", mock.GetMessage());
            }
        }
    }
}
