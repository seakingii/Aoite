using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    class RedisObject : RedisCommand<object>
    {
        public RedisObject(string command, params object[] args) : base(command, args) { }

        internal override object Parse(RedisExecutor executor)
        {
            return executor.ReadObject();
        }
    }
}
