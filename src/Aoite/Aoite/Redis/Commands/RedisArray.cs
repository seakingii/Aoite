using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoite.Redis.Commands
{
    static class RedisArray
    {
        public static Generic<T> Create<T>(RedisCommand<T> command, int parseItemCount = 1)
        {
            return new Generic<T>(command, parseItemCount);
        }

        public class Generic<T> : RedisCommand<T[]>
        {
            readonly RedisCommand<T> _command;

            protected RedisCommand<T> Subcommand { get { return _command; } }
            private long _parseItemCount = 1;

            public Generic(RedisCommand<T> command, long parseItemCount = 1)
                : base(command.Command, command.Arguments)
            {
                this._command = command;
                this._parseItemCount = parseItemCount;
            }

            internal override T[] Parse(RedisExecutor executor)
            {
                executor.AssertType(RedisReplyType.MultiBulk);
                long count = executor.ReadInteger(false);
                return Read(count / this._parseItemCount, executor).ToArray();
            }

            protected virtual IEnumerable<T> Read(long count, RedisExecutor stream)
            {
                for(int i = 0; i < count; i++)
                    yield return _command.Parse(stream);
            }
        }

        public class Scan<T> : Generic<T>
        {
            public Scan(RedisCommand<T> command, long parseItemCount = 1) : base(command, parseItemCount) { }

            public long Cursor { get; private set; }

            internal override T[] Parse(RedisExecutor executor)
            {
                executor.AssertType(RedisReplyType.MultiBulk);
                if(executor.ReadInteger(false) != 2) throw new RedisProtocolException("预期返回 2 个项。");
                this.Cursor = Int64.Parse(executor.ReadBulkString());
                return base.Parse(executor);
            }
        }

       
    }
}
