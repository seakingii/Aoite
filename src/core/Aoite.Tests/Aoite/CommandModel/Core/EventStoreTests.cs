using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.CommandModel.Core
{
    public class EventStoreTests
    {
        class SimpleCommand : ICommand<int>
        {
            public int Value { get; set; }

            public int ResultValue { get; set; }
        }
        [Fact()]
        public void CtorArgumentNullExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => new EventStore(null));
        }

        [Fact()]
        public void RegisterTest()
        {
            var container = new IocContainer();
            var store = new EventStore(container);
            int x = 1;
            var event1 = new MockEvent<SimpleCommand>((context, command) =>
            {
                x += 1;
                return true;
            }, (context, command, exception) =>
            {
                x += 2;
            });
            var event2 = new MockEvent<SimpleCommand>((context, command) =>
            {
                x += 3;
                return true;
            }, (context, command, exception) =>
            {
                x += 4;
            });
            store.Register<SimpleCommand>(event1);
            store.Register<SimpleCommand>(event2);
            Assert.True(store.RaiseExecuting<SimpleCommand>(null, null));
            Assert.Equal(5, x);
            store.RaiseExecuted<SimpleCommand>(null, null, null);
            Assert.Equal(11, x);
        }


        [Fact()]
        public void UnregisterTest()
        {
            var container = new IocContainer();
            var store = new EventStore(container);
            int x = 1;
            var event1 = new MockEvent<SimpleCommand>((context, command) =>
            {
                x += 1;
                return true;
            }, (context, command, exception) =>
            {
                x += 2;
            });
            var event2 = new MockEvent<SimpleCommand>((context, command) =>
            {
                x += 3;
                return true;
            }, (context, command, exception) =>
            {
                x += 4;
            });
            store.Register<SimpleCommand>(event1);
            store.Register<SimpleCommand>(event2);
            Assert.True(store.RaiseExecuting<SimpleCommand>(null, null));
            Assert.Equal(5, x);
            store.RaiseExecuted<SimpleCommand>(null, null, null);
            Assert.Equal(11, x);

            store.Unregister<SimpleCommand>(event1);
            x = 1;
            Assert.True(store.RaiseExecuting<SimpleCommand>(null, null));
            Assert.Equal(4, x);
            store.RaiseExecuted<SimpleCommand>(null, null, null);
            Assert.Equal(8, x);

        }


        [Fact()]
        public void UnregisterAllTest()
        {
            var container = new IocContainer();
            var store = new EventStore(container);
            int x = 1;
            var event1 = new MockEvent<SimpleCommand>((context, command) =>
            {
                x += 1;
                return true;
            }, (context, command, exception) =>
            {
                x += 2;
            });
            var event2 = new MockEvent<SimpleCommand>((context, command) =>
            {
                x += 3;
                return true;
            }, (context, command, exception) =>
            {
                x += 4;
            });
            store.Register<SimpleCommand>(event1);
            store.Register<SimpleCommand>(event2);
            Assert.True(store.RaiseExecuting<SimpleCommand>(null, null));
            Assert.Equal(5, x);
            store.RaiseExecuted<SimpleCommand>(null, null, null);
            Assert.Equal(11, x);

            store.UnregisterAll<SimpleCommand>();
            x = 1;
            Assert.True(store.RaiseExecuting<SimpleCommand>(null, null));
            Assert.Equal(1, x);
            store.RaiseExecuted<SimpleCommand>(null, null, null);
            Assert.Equal(1, x);

        }
    }
}
