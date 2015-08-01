using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.CommandModel.Core
{
    public class ExecutorFactoryTests
    {
        #region Commands & Executors

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

        class Simple3 : ICommand
        {
            public int Value { get; set; }
        }
        class Simple3Executor : IExecutor<Simple3>
        {
            public void Execute(IContext context, Simple3 command)
            {
            }
        }

        class Simple4<T1, T2> : ICommand
        {
            public T1 T1Property { get; set; }
            public T2 T2Property { get; set; }
        }
        class Simple4Executor<T1, T2> : IExecutor<Simple4<T1, T2>>
        {
            public void Execute(IContext context, Simple4<T1, T2> command) { }
        }

        class Simple5<T1, T2> : ICommand
        {
            public static readonly Type ExecutorType = typeof(Executor);
            public T1 T1Property { get; set; }
            public T2 T2Property { get; set; }
            class Executor : IExecutor<Simple5<T1, T2>>
            {
                public void Execute(IContext context, Simple5<T1, T2> command) { }
            }
        }

        [BindingExecutor(typeof(TestSimple6))]
        class Simple6 : ICommand { }
        class TestSimple6 : IExecutor<Simple6>
        {
            public void Execute(IContext context, Simple6 command) { }
        }
        [BindingExecutor(typeof(TestSimple7<,>))]
        class Simple7<T1, T2> : ICommand { }
        class TestSimple7<T1, T2> : IExecutor<Simple7<T1, T2>>
        {
            public void Execute(IContext context, Simple7<T1, T2> command) { }
        }

        #endregion

        [Fact()]
        public void CtorArgumentNullExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => new ExecutorFactory(null));
        }

        [Fact()]
        public void CallCreateTest()
        {
            ExecutorFactory factory = new ExecutorFactory(new IocContainer());
            //- 普通
            Assert.IsType<Simple1Executor>(factory.Create(new Simple1Command()).Executor);
            Assert.IsType<Simple2Executor>(factory.Create(new Simple2Command()).Executor);
            Assert.IsType<Simple3Executor>(factory.Create(new Simple3()).Executor);
            //- 泛型
            Assert.IsType<Simple4Executor<int, string>>(factory.Create(new Simple4<int, string>()).Executor);
            //- 嵌套
            Assert.IsType(Simple5<int, string>.ExecutorType, factory.Create(new Simple5<int, string>()).Executor);

            //- 特性
            Assert.IsType<TestSimple6>(factory.Create(new Simple6()).Executor);
            //- 特性泛型
            Assert.IsType<TestSimple7<string, double>>(factory.Create(new Simple7<string, double>()).Executor);
        }
    }
}
