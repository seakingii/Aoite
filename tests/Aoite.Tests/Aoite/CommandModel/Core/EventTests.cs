using Xunit;

namespace Aoite.CommandModel.Core
{
    public class EventTests
    {
        class SimpleCommand : ICommand<int>
        {
            public int Value { get; set; }

            public int Result { get; set; }
        }
        [Fact()]
        public void ExecutingTest()
        {
            Event<SimpleCommand> @event = new Event<SimpleCommand>();
            var e3Return = false;
            int x = 1;
            @event.Executing += (context, command) =>
            {
                x = 2;
                return true;
            };
            @event.Executing += (context, command) =>
            {
                x = 3;
                return e3Return;
            };
            @event.Executing += (context, command) =>
            {
                x = 4;
                return true;
            };
            Assert.False(@event.RaiseExecuting(null, null));
            Assert.Equal(3, x);
            e3Return = true;
            Assert.True(@event.RaiseExecuting(null, null));
            Assert.Equal(4, x);
        }

        [Fact()]
        public void ExecutedTest()
        {
            Event<SimpleCommand> @event = new Event<SimpleCommand>();
            int x = 1;
            @event.Executed += (context, command, exception) =>
            {
                x = 2;
            };
            @event.Executed += (context, command, exception) =>
            {
                x = 3;
            };
            @event.Executed += (context, command, exception) =>
            {
                x = 4;
            };
            @event.RaiseExecuted(null, null, null);
            Assert.Equal(4, x);
        }
    }
}
