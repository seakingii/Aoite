using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace Aoite.CommandModel
{
    [SingletonMapping]
    class CommandMemoryCache : MemoryCache
    {
        public CommandMemoryCache() : base("CommandMemoryCache") { }
    }
}
