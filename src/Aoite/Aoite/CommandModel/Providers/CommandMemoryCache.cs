using System;
using System.Runtime.Caching;

namespace Aoite.CommandModel
{
    [SingletonMapping]
    class CommandMemoryCache : MemoryCache
    {
        public CommandMemoryCache() : base("CommandMemoryCache") { }
    }
}
