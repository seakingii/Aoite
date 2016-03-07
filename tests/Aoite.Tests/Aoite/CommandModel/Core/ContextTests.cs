using System;
using Xunit;

namespace Aoite.CommandModel.Core
{
    public class ContextTests
    {
        class SimpleCommand : ICommand<int>
        {
            public int Value { get; set; }

            public int Result { get; set; }
        }
        private Context CreateContext(IIocContainer container, ICommand command)
        {
            return new Context(container, command, new Lazy<System.Collections.Specialized.HybridDictionary>(), new Lazy<Data.IDbEngine>(() => Db.Engine));
        }

        [Fact()]
        public void CtorArgumentNullExceptionTest()
        {
            var command = new SimpleCommand();
            Assert.Throws<ArgumentNullException>(() => CreateContext(null, command));
            var container = new IocContainer();
            Assert.Throws<ArgumentNullException>(() => CreateContext(container, null));
        }

        [Fact()]
        public void GetUserTest()
        {
            var container = new IocContainer();
            var command = new SimpleCommand();
            var context = CreateContext(container, command);
            Assert.Null(context.User);
            var username = "user";
            container.Add<IUserFactory>(new UserFactory(ioc => username));
            Assert.Equal(username, context.User);
        }

        [Fact()]
        public void GetDataTest()
        {
            var container = new IocContainer();
            var command = new SimpleCommand();
            var context = CreateContext(container, command);
            Assert.Equal(0, context.Data.Count);
            context["a"] = 2;
            Assert.Equal(1, context.Data.Count);
            Assert.Equal(2, context["a"]);
            Assert.Equal(2, context.Data["a"]);
        }

        [Fact()]
        public void GetCommandTest()
        {
            var container = new IocContainer();
            var command = new SimpleCommand();
            var context =  CreateContext(container, command );
            Assert.Equal(command, context.Command);
        }
        [Fact()]
        public void GetEngineTest()
        {
            var container = new IocContainer();
            var command = new SimpleCommand();
            var context =  CreateContext(container, command);
            Assert.Null(context.Engine);
        }
    }
}
