using System;
using Xunit;

namespace Aoite.CommandModel.Core
{
    public class CommandBusTests
    {
        class SimpleCommand : ICommand<int>
        {
            public int Value { get; set; }

            public int Result { get; set; }
        }


        [Fact()]
        public void ExecuteTest()
        {
            var bus = new CommandBus(ServiceFactory.CreateContainer(null
                , f => f.Mock<SimpleCommand>((context, command) => command.Result = command.Value * 2)));

            Assert.Equal(10, bus.Execute(new SimpleCommand() { Value = 5 }).Result);
            Assert.Equal(0, bus.Execute(new SimpleCommand() { Value = 5 }, (context, command) => false).Result);
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
                , f => f.Mock<SimpleCommand>((context, command) => command.Result = command.Value * 2)));
            Assert.Equal(10, bus.ExecuteAsync(new SimpleCommand() { Value = 5 }).Result.Result);
            Assert.Equal(0, bus.ExecuteAsync(new SimpleCommand() { Value = 5 }, (context, command) => false).Result.Result);
        }

        [Fact()]
        public async void ExecuteAsyncExceptionTest()
        {
            var bus = new CommandBus(ServiceFactory.CreateContainer(null
                , f => f.Mock<SimpleCommand>((context, command) =>
                {
                    throw new NotSupportedException();
                })));
            await Assert.ThrowsAsync<NotSupportedException>(() => bus.ExecuteAsync(new SimpleCommand() { Value = 5 }, null, (context, command, exception) =>
            {
                Assert.IsType<NotSupportedException>((exception as AggregateException).InnerException);
            }));
        }

    }
}
