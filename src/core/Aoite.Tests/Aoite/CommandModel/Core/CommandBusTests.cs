using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.CommandModel.Core
{
    public class CommandBusTests
    {
        class SimpleCommand : ICommand<int>
        {
            public int Value { get; set; }

            public int ResultValue { get; set; }
        }


        [Fact()]
        public void ExecuteTest()
        {
            var bus = new CommandBus(ServiceFactory.CreateContainer(null
                , f => f.Mock<SimpleCommand>((context, command) => command.ResultValue = command.Value * 2)));

            Assert.Equal(10, bus.Execute(new SimpleCommand() { Value = 5 }).ResultValue);
            Assert.Equal(0, bus.Execute(new SimpleCommand() { Value = 5 }, (context, command) => false).ResultValue);
        }
        [Fact()]
        public void ExecuteExceptionTest()
        {
            var bus = new CommandBus(ServiceFactory.CreateContainer(null
                , f => f.Mock<SimpleCommand>((context, command) =>
                {
                    throw new NotSupportedException();
                })));
            Exception catchException = null;
            Assert.Throws<NotSupportedException>(() => bus.Execute(new SimpleCommand() { Value = 5 }, null, (context, command, exception) =>
            {
                catchException = exception;
            }));
            Assert.IsType<NotSupportedException>(catchException);
        }
        [Fact()]
        public void ExecuteAsyncTest()
        {
            var bus = new CommandBus(ServiceFactory.CreateContainer(null
                , f => f.Mock<SimpleCommand>((context, command) => command.ResultValue = command.Value * 2)));
            Assert.Equal(10, bus.ExecuteAsync(new SimpleCommand() { Value = 5 }).Result.ResultValue);
            Assert.Equal(0, bus.ExecuteAsync(new SimpleCommand() { Value = 5 }, (context, command) => false).Result.ResultValue);
        }

        [Fact()]
        public void ExecuteAsyncExceptionTest()
        {
            var bus = new CommandBus(ServiceFactory.CreateContainer(null
                , f => f.Mock<SimpleCommand>((context, command) =>
                {
                    throw new NotSupportedException();
                })));
            Exception catchException = null;
            Assert.Throws<AggregateException>(() => bus.ExecuteAsync(new SimpleCommand() { Value = 5 }, null, (context, command, exception) =>
            {
                catchException = exception;
            }).Result);
            Assert.IsType<NotSupportedException>(catchException);
        }

    }
}
