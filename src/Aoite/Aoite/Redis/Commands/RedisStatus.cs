using System;

namespace Aoite.Redis.Commands
{
    class RedisStatus : RedisCommand<Result>
    {
        public RedisStatus(string command, params object[] args) : base(command, args) { }

        internal override Result Parse(RedisExecutor executor)
        {
            return executor.ReadStatus();
        }
        public class MultiBulk : RedisCommand<bool>
        {
            public MultiBulk(string command, params object[] args) : base(command, args) { }

            internal override bool Parse(RedisExecutor executor)
            {
                var type = executor.ReadType();
                if(type == RedisReplyType.Status)
                {
                    if(executor.ReadStatus(false).IsSucceed) return true;
                    return false;
                }
                object[] result = executor.ReadMultiBulk(false);
                if(result != null) throw new RedisProtocolException("预期空的多条批量回复，实际出现未知回复。 ");

                return false;
            }
        }
        public class Pong : RedisStatus
        {
            public Pong(string command, params object[] args) : base(command, args) { }
            internal override Result Parse(RedisExecutor executor)
            {
                return executor.ReadStatus(statusText: "PONG");
            }
        }
    }
}
