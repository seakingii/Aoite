using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.CommandModel
{
    public class CacheAttributeTests
    {
        [Fact]
        public void CtorTest()
        {
            Assert.Equal("group", Assert.Throws<ArgumentNullException>(() => new CacheAttribute(null)).ParamName);
        }

        [Fact]
        public void get_GroupTest()
        {
            Assert.Equal("group", new CacheAttribute("group").Group);
        }

        [Fact]
        public void RaiseExecutedTest()
        {
            var executorAttr = new CacheAttribute("group") as IExecutorAttribute;
            executorAttr.RaiseExecuted(null, null, new Exception());
        }

        class Command1 : ICommand { }

        class Command2 : ICommand, ICommandCache
        {
            public bool NullStrategy { get; set; }
            class CommandCacheStrategy : ICommandCacheStrategy
            {
                public DateTimeOffset AbsoluteExpiration
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                }

                public string Key
                {
                    get
                    {
                        return null;
                    }
                }

                public TimeSpan SlidingExpiration
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                }

                public object GetCache(string group)
                {
                    throw new NotImplementedException();
                }

                public void SetCache(string group, object value)
                {
                    throw new NotImplementedException();
                }
            }
            public ICommandCacheStrategy CreateStrategy(IContext context)
            {
                return NullStrategy ? null : new CommandCacheStrategy();
            }

            public object GetCacheValue()
            {
                throw new NotImplementedException();
            }

            public bool SetCacheValue(object value)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void RaiseExecutingTest()
        {
            var executorAttr = new CacheAttribute("group") as IExecutorAttribute;
            Assert.True(Assert.Throws<NotSupportedException>(() => executorAttr.RaiseExecuting(null, new Command1())).Message.EndsWith("：命令模型没有实现缓存接口。"));
            Assert.True(Assert.Throws<NotSupportedException>(() => executorAttr.RaiseExecuting(null, new Command2() { NullStrategy = true })).Message.EndsWith("：命令模型返回了无效的策略信息。"));
            Assert.True(Assert.Throws<NotSupportedException>(() => executorAttr.RaiseExecuting(null, new Command2() { NullStrategy = false })).Message.EndsWith("：命令模型返回了无效的策略信息。"));
        }
    }
}
