using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.CommandModel.Core
{
    public class ExecutorFactoryTests
    {
        class Simple1Command : ICommand
        {
            public int Value { get; set; }
        }
        class Simple1Executor : IExecutor<Simple1Command>
        {
            public void Execute(IContext context, Simple1Command command)
            {
            }
        }

        class Simple2Command : ICommand
        {
            public int Value { get; set; }
        }
        class Simple2Executor : IExecutor<Simple2Command>
        {
            public void Execute(IContext context, Simple2Command command)
            {
            }
        }

        [Fact()]
        public void CtorArgumentNullExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => new ExecutorFactory(null));
        }
        [Fact()]
        public void CallCreateTest()
        {
            ExecutorFactory factory = new ExecutorFactory(new IocContainer());
            Assert.IsType<Simple1Executor>(factory.Create(new Simple1Command()).Executor);
            Assert.IsType<Simple2Executor>(factory.Create(new Simple2Command()).Executor);
        }
    }
}
