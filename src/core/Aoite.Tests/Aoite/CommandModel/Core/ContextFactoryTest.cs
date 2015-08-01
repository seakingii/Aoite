using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.CommandModel.Core
{
    public class ContextFactoryTest
    {
        class SimpleCommand : ICommand<int>
        {
            public int Value { get; set; }

            public int ResultValue { get; set; }
        }

        [Fact()]
        public void CtorArgumentNullExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => new ContextFactory(null));
        }

        [Fact()]
        public void CallCreateTest()
        {
            var container = new IocContainer();
            var command = new SimpleCommand();
            var contextFactory = new ContextFactory(container);
            var context = contextFactory.Create(command);
            Assert.Equal(command, context.Command);
            Assert.Equal(container, context.Container);
        }

    }
}
