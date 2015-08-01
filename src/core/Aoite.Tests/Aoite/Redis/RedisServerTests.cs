using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Redis
{
    public class ServerTests
    {
        [Fact()]
        public void BgRewriteAofTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.BgRewriteAof());
                Assert.Equal("*1\r\n$12\r\nBGREWRITEAOF\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void BgSaveTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.BgSave());
                Assert.Equal("*1\r\n$6\r\nBGSAVE\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ClientGetNameTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "$6\r\nmyname\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal("myname", redis.ClientGetName());
                Assert.Equal("*2\r\n$6\r\nCLIENT\r\n$7\r\nGETNAME\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ClientKillTest()
        {
            var reply1 = "+OK\r\n";
            var reply2 = ":1\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply1, reply2, reply2, reply2, reply2, reply2))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.ClientKill("1.1.1.1", 9999));
                Assert.Equal("*3\r\n$6\r\nCLIENT\r\n$4\r\nKILL\r\n$12\r\n1.1.1.1:9999\r\n", mock.GetMessage());

                Assert.Equal(1, redis.ClientKill(addr: "1.1.1.1:9999"));
                Assert.Equal("*4\r\n$6\r\nCLIENT\r\n$4\r\nKILL\r\n$4\r\nADDR\r\n$12\r\n1.1.1.1:9999\r\n", mock.GetMessage());

                Assert.Equal(1, redis.ClientKill(id: "123"));
                Assert.Equal("*4\r\n$6\r\nCLIENT\r\n$4\r\nKILL\r\n$2\r\nID\r\n$3\r\n123\r\n", mock.GetMessage());

                Assert.Equal(1, redis.ClientKill(type: "normal"));
                Assert.Equal("*4\r\n$6\r\nCLIENT\r\n$4\r\nKILL\r\n$4\r\nTYPE\r\n$6\r\nnormal\r\n", mock.GetMessage());

                Assert.Equal(1, redis.ClientKill(skipMe: true));
                Assert.Equal("*4\r\n$6\r\nCLIENT\r\n$4\r\nKILL\r\n$6\r\nSKIPME\r\n$3\r\nyes\r\n", mock.GetMessage());

                Assert.Equal(1, redis.ClientKill(skipMe: false));
                Assert.Equal("*4\r\n$6\r\nCLIENT\r\n$4\r\nKILL\r\n$6\r\nSKIPME\r\n$2\r\nno\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ClientListTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "$291\r\nid=3 addr=127.0.0.1:57656 fd=6 name= age=97 idle=81 flags=N db=0 sub=0 psub=0 multi=-1 qbuf=0 qbuf-free=0 obl=0 oll=0 omem=0 events=r cmd=client\nid=4 addr=127.0.0.1:57663 fd=7 name= age=5 idle=0 flags=N db=0 sub=0 psub=0 multi=-1 qbuf=0 qbuf-free=32768 obl=0 oll=0 omem=0 events=r cmd=client\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal("id=3 addr=127.0.0.1:57656 fd=6 name= age=97 idle=81 flags=N db=0 sub=0 psub=0 multi=-1 qbuf=0 qbuf-free=0 obl=0 oll=0 omem=0 events=r cmd=client\nid=4 addr=127.0.0.1:57663 fd=7 name= age=5 idle=0 flags=N db=0 sub=0 psub=0 multi=-1 qbuf=0 qbuf-free=32768 obl=0 oll=0 omem=0 events=r cmd=client", redis.ClientList());
                Assert.Equal("*2\r\n$6\r\nCLIENT\r\n$4\r\nLIST\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ClientSetNameTest()
        {
            string reply = "+OK\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply, reply))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.ClientSetName("myname"));
                Assert.Equal("*3\r\n$6\r\nCLIENT\r\n$7\r\nSETNAME\r\n$6\r\nmyname\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ConfigGetTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "*4\r\n$10\r\nmasterauth\r\n$-1\r\n$7\r\nlogfile\r\n$18\r\n/var/log/redis.log\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.ConfigGet("*");
                Assert.Equal(2, response.Length);
                Assert.Equal("masterauth", response[0].Key);
                Assert.Equal(null, (string)response[0].Value);
                Assert.Equal("logfile", response[1].Key);
                Assert.Equal("/var/log/redis.log", (string)response[1].Value);
                Assert.Equal("*3\r\n$6\r\nCONFIG\r\n$3\r\nGET\r\n$1\r\n*\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ConfigResetStatTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.ConfigResetStat());
                Assert.Equal("*2\r\n$6\r\nCONFIG\r\n$9\r\nRESETSTAT\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ConfigRewriteTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.ConfigRewrite());
                Assert.Equal("*2\r\n$6\r\nCONFIG\r\n$7\r\nREWRITE\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void ConfigSetTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.ConfigSet("param", "value"));
                Assert.Equal("*4\r\n$6\r\nCONFIG\r\n$3\r\nSET\r\n$5\r\nparam\r\n$5\r\nvalue\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void DbSizeTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":5\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(5, redis.DbSize());
                Assert.Equal("*1\r\n$6\r\nDBSIZE\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void FlushAllTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.FlushAll());
                Assert.Equal("*1\r\n$8\r\nFLUSHALL\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void FlushDbTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.FlushDb());
                Assert.Equal("*1\r\n$7\r\nFLUSHDB\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void InfoTest()
        {
            var reply1 = "$1774\r\n# Server\r\nredis_version:2.8.12\r\nredis_git_sha1:00000000\r\nredis_git_dirty:0\r\nredis_build_id:15bba638a3b1acf9\r\nredis_mode:standalone\r\nos:Linux 2.6.32-220.17.1.el6.x86_64 x86_64\r\narch_bits:64\r\nmultiplexing_api:epoll\r\ngcc_version:4.4.6\r\nprocess_id:16533\r\nrun_id:76e33834f46f97e0acf1c2749245bc25758afe41\r\ntcp_port:6379\r\nuptime_in_seconds:183775\r\nuptime_in_days:2\r\nhz:10\r\nlru_clock:12351741\r\nconfig_file:/etc/redis.conf\r\n\r\n# Clients\r\nconnected_clients:2\r\nclient_longest_output_list:0\r\nclient_biggest_input_buf:0\r\nblocked_clients:0\r\n\r\n# Memory\r\nused_memory:365208\r\nused_memory_human:356.65K\r\nused_memory_rss:7626752\r\nused_memory_peak:386120\r\nused_memory_peak_human:377.07K\r\nused_memory_lua:33792\r\nmem_fragmentation_ratio:20.88\r\nmem_allocator:jemalloc-3.6.0\r\n\r\n# Persistence\r\nloading:0\r\nrdb_changes_since_last_save:0\r\nrdb_bgsave_in_progress:0\r\nrdb_last_save_time:1404676894\r\nrdb_last_bgsave_status:ok\r\nrdb_last_bgsave_time_sec:-1\r\nrdb_current_bgsave_time_sec:-1\r\naof_enabled:0\r\naof_rewrite_in_progress:0\r\naof_rewrite_scheduled:0\r\naof_last_rewrite_time_sec:-1\r\naof_current_rewrite_time_sec:-1\r\naof_last_bgrewrite_status:ok\r\naof_last_write_status:ok\r\n\r\n# Stats\r\ntotal_connections_received:9\r\ntotal_commands_processed:23\r\ninstantaneous_ops_per_sec:0\r\nrejected_connections:0\r\nsync_full:0\r\nsync_partial_ok:0\r\nsync_partial_err:0\r\nexpired_keys:0\r\nevicted_keys:0\r\nkeyspace_hits:0\r\nkeyspace_misses:0\r\npubsub_channels:0\r\npubsub_patterns:0\r\nlatest_fork_usec:0\r\n\r\n# Replication\r\nrole:master\r\nconnected_slaves:0\r\nmaster_repl_offset:0\r\nrepl_backlog_active:0\r\nrepl_backlog_size:1048576\r\nrepl_backlog_first_byte_offset:0\r\nrepl_backlog_histlen:0\r\n\r\n# CPU\r\nused_cpu_sys:139.97\r\nused_cpu_user:43.93\r\nused_cpu_sys_children:0.00\r\nused_cpu_user_children:0.00\r\n\r\n# Keyspace\r\ndb0:keys=6,expires=0,avg_ttl=0\r\n";
            var reply2 = "$104\r\n# CPU\r\nused_cpu_sys:140.26\r\nused_cpu_user:43.95\r\nused_cpu_sys_children:0.00\r\nused_cpu_user_children:0.00\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply1, reply2))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal("# Server\r\nredis_version:2.8.12\r\nredis_git_sha1:00000000\r\nredis_git_dirty:0\r\nredis_build_id:15bba638a3b1acf9\r\nredis_mode:standalone\r\nos:Linux 2.6.32-220.17.1.el6.x86_64 x86_64\r\narch_bits:64\r\nmultiplexing_api:epoll\r\ngcc_version:4.4.6\r\nprocess_id:16533\r\nrun_id:76e33834f46f97e0acf1c2749245bc25758afe41\r\ntcp_port:6379\r\nuptime_in_seconds:183775\r\nuptime_in_days:2\r\nhz:10\r\nlru_clock:12351741\r\nconfig_file:/etc/redis.conf\r\n\r\n# Clients\r\nconnected_clients:2\r\nclient_longest_output_list:0\r\nclient_biggest_input_buf:0\r\nblocked_clients:0\r\n\r\n# Memory\r\nused_memory:365208\r\nused_memory_human:356.65K\r\nused_memory_rss:7626752\r\nused_memory_peak:386120\r\nused_memory_peak_human:377.07K\r\nused_memory_lua:33792\r\nmem_fragmentation_ratio:20.88\r\nmem_allocator:jemalloc-3.6.0\r\n\r\n# Persistence\r\nloading:0\r\nrdb_changes_since_last_save:0\r\nrdb_bgsave_in_progress:0\r\nrdb_last_save_time:1404676894\r\nrdb_last_bgsave_status:ok\r\nrdb_last_bgsave_time_sec:-1\r\nrdb_current_bgsave_time_sec:-1\r\naof_enabled:0\r\naof_rewrite_in_progress:0\r\naof_rewrite_scheduled:0\r\naof_last_rewrite_time_sec:-1\r\naof_current_rewrite_time_sec:-1\r\naof_last_bgrewrite_status:ok\r\naof_last_write_status:ok\r\n\r\n# Stats\r\ntotal_connections_received:9\r\ntotal_commands_processed:23\r\ninstantaneous_ops_per_sec:0\r\nrejected_connections:0\r\nsync_full:0\r\nsync_partial_ok:0\r\nsync_partial_err:0\r\nexpired_keys:0\r\nevicted_keys:0\r\nkeyspace_hits:0\r\nkeyspace_misses:0\r\npubsub_channels:0\r\npubsub_patterns:0\r\nlatest_fork_usec:0\r\n\r\n# Replication\r\nrole:master\r\nconnected_slaves:0\r\nmaster_repl_offset:0\r\nrepl_backlog_active:0\r\nrepl_backlog_size:1048576\r\nrepl_backlog_first_byte_offset:0\r\nrepl_backlog_histlen:0\r\n\r\n# CPU\r\nused_cpu_sys:139.97\r\nused_cpu_user:43.93\r\nused_cpu_sys_children:0.00\r\nused_cpu_user_children:0.00\r\n\r\n# Keyspace\r\ndb0:keys=6,expires=0,avg_ttl=0", redis.Info());
                Assert.Equal("*1\r\n$4\r\nINFO\r\n", mock.GetMessage());

                redis.Info("CPU");
                Assert.Equal("*2\r\n$4\r\nINFO\r\n$3\r\nCPU\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void LastSaveTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1404861064\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(new DateTime(2014, 7, 8, 23, 11, 04, DateTimeKind.Utc).ToLocalTime(), redis.LastSave());
                Assert.Equal("*1\r\n$8\r\nLASTSAVE\r\n", mock.GetMessage());
            }
        }

   
        [Fact()]
        public void SaveTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.Save());
                Assert.Equal("*1\r\n$4\r\nSAVE\r\n", mock.GetMessage());
            }
        }
    }
}
