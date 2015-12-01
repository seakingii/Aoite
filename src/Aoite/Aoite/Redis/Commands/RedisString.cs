using System;

namespace Aoite.Redis.Commands
{
    class RedisString : RedisCommand<String>
    {
        public RedisString(string command, params object[] args) : base(command, args) { }

        internal override string Parse(RedisExecutor executor)
        {
            return executor.ReadBulkString();
        }
    }
}
