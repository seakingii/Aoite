﻿using System;
using Xunit;

namespace Aoite.CommandModel.Core
{
    public class ContextFactoryTest
    {
        class SimpleCommand : ICommand<int>
        {
            public int Value { get; set; }

            public int Result { get; set; }
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
            var context = contextFactory.Create(command, new Lazy<System.Collections.Specialized.HybridDictionary>(), new Lazy<Data.IDbEngine>(() => Db.Engine));
            Assert.Equal(command, context.Command);
            Assert.Equal(container, context.Container);
        }

    }
}
