using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.CommandModel.Core
{
    public class CommandModelContainerProviderBaseTests
    {
        class SimpleCommandModelContainerProvider : CommandModelContainerProviderBase
        {
            public SimpleCommandModelContainerProvider(IIocContainer container) : base(container) { }
        }

        [Fact()]
        public void CtorArgumentNullExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => new SimpleCommandModelContainerProvider(null));
        }

        [Fact()]
        public void GetContainerTest()
        {
            var container = new IocContainer();
            var s = new SimpleCommandModelContainerProvider(container);
            Assert.NotNull(s.Container);
            Assert.Equal(container, s.Container);
        }
    }
}
