using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    class RedisTran
    {
        public class Exec : RedisStatus
        {
            LinkedList<RedisCommand> _tranCommands;
            internal Exec(LinkedList<RedisCommand> tranCommands)
                : base("EXEC", new object[0])
            {
                this._tranCommands = tranCommands;
            }

            internal override Result Parse(RedisExecutor executor)
            {
                executor.AssertType(RedisReplyType.MultiBulk);
                var count = executor.ReadInteger(false);
                if(count != this._tranCommands.Count)
                    throw new RedisProtocolException(String.Format("预期 {0} 事务返回项，实际只有 {1} 个返回项。", this._tranCommands.Count, count));
                var node = this._tranCommands.First;
                Queue<object> values = new Queue<object>((int)count);
                try
                {
                    do
                    {
                        values.Enqueue(node.Value.ObjectParse(executor));
                        node = node.Next;
                    } while(node != null);
                }
                catch(Exception)
                {
                    this._tranCommands.Clear();
                    throw;
                }

                this._tranCommands.Each(command => command.RunCallback(values.Dequeue()));
                this._tranCommands.Clear();
                return Result.Successfully;
            }
        }

        public class Queue : RedisCommand<Result>
        {
            public Queue(RedisCommand command) : base(command.Command, command.Arguments) { }

            internal override Result Parse(RedisExecutor executor)
            {
                return executor.ReadStatus(statusText: "QUEUED");
            }
        }
    }
}
