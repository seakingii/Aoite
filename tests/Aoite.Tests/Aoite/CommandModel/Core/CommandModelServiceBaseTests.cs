using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.CommandModel.Core
{
    public class CommandModelServiceBaseTests
    {
        class SimpleCommandModelService : CommandModelServiceBase
        {
            public SimpleCommandModelService(IIocContainer container) : base(container) { }

            public int ExecuteTest(int value)
            {
                return this.Execute(new SimpleCommand() { Value = value }).ResultValue;
            }
            public Task<int> ExecuteTestAsync(int value)
            {
                return this.ExecuteAsync(new SimpleCommand() { Value = value })
                           .ContinueWith(t => t.Result.ResultValue);
            }

            private int _lockValue = 1;
            public int LockTest()
            {
                using(this.AcquireLock<SimpleCommandModelService>())
                {
                    return this._lockValue++;
                }
            }
            public long IncrementTest1(long increment = 1)
            {
                return this.Increment<SimpleCommandModelService>(increment);
            }
            public long IncrementTest2(long increment = 1)
            {
                return this.Increment("Key2", increment);
            }

            public ITransaction BeginTransactionTest()
            {
                return this.BeginTransaction();
            }
        }

        class SimpleCommand : ICommand<int>
        {
            public int Value { get; set; }

            public int ResultValue { get; set; }
        }

        [Fact()]
        public void CtorArgumentNullExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => new SimpleCommandModelService(null));
        }

        [Fact()]
        public void GetContainerTest()
        {
            var container = new IocContainer();
            var s = new SimpleCommandModelService(container);
            Assert.NotNull(s.Container);
            Assert.Equal(container, s.Container);
        }

        [Fact()]
        public void GetUserTest()
        {
            var container = new IocContainer();
            var username = "user";
            var s = new SimpleCommandModelService(container);
            Assert.Null(s.User);
            container.AddService<IUserFactory>(new UserFactory(ioc => username));
            Assert.Equal(username, s.User);
        }

        [Fact()]
        public void ExecuteTest()
        {
            var container = ServiceFactory.CreateContainer(null, f => f.Mock<SimpleCommand>((context, command) => command.ResultValue = command.Value * 2));
            var s = new SimpleCommandModelService(container);
            Assert.Equal(10, s.ExecuteTest(5));
        }

        [Fact()]
        public void ExecuteAsyncTest()
        {
            var container = ServiceFactory.CreateContainer(null, f => f.Mock<SimpleCommand>((context, command) => command.ResultValue = command.Value * 2));
            var s = new SimpleCommandModelService(container);
            Assert.Equal(10, s.ExecuteTestAsync(5).Result);
        }

        [Fact()]
        public void LockTest()
        {
            var container = new IocContainer();
            var s = new SimpleCommandModelService(container);
            System.Collections.Concurrent.ConcurrentBag<int> bag = new System.Collections.Concurrent.ConcurrentBag<int>();
            List<Task> tasks = new List<Task>();
            for(int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    bag.Add(s.LockTest());
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Assert.Equal(55, bag.Sum());
        }

        [Fact()]
        public void IncrementTest()
        {
            var container = new IocContainer();
            var s = new SimpleCommandModelService(container);

            List<Task> tasks = new List<Task>();
            for(int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    s.IncrementTest1();
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Assert.Equal(11, s.IncrementTest1());
            Assert.Equal(1, s.IncrementTest2());
        }

        [Fact()]
        public void BeginTransactionTest()
        {
            var container = new IocContainer();
            var s = new SimpleCommandModelService(container);
            using(var tran = s.BeginTransactionTest())
            {
                Assert.IsType<DefaultTransaction>(tran);
            }
        }
    }
}
