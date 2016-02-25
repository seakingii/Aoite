using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个模拟的命令模型执行器工厂。
    /// </summary>
    public sealed class MockExecutorFactory : CommandModelContainerProviderBase, IExecutorFactory
    {
        private Dictionary<Type, IExecutorMetadata> Executors = new Dictionary<Type, IExecutorMetadata>();

        /// <summary>
        /// 初始化一个 <see cref="MockExecutorFactory"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public MockExecutorFactory(IIocContainer container) : base(container) { }

        /// <summary>
        /// 模拟指定命令模型的执行方法。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="mockHandler">模拟的执行器。</param>
        /// <returns>当前执行器工厂。</returns>
        public MockExecutorFactory Mock<TCommand>(CommandExecuteHandler<TCommand> mockHandler) where TCommand : ICommand
        {
            Executors[typeof(TCommand)] = new ExecutorMetadata<TCommand>(new MockExecutor<TCommand>(mockHandler));
            return this;
        }

        IExecutorMetadata<TCommand> IExecutorFactory.Create<TCommand>(TCommand command)
        {
            var executor = Executors.TryGetValue(typeof(TCommand)) as IExecutorMetadata<TCommand>;
            if(executor == null)
            {
                if(_queues.Count == 0) throw new NotSupportedException($"命令{typeof(TCommand).FullName}没有模拟执行器。");
                var handler = _queues.Dequeue();
                if(handler != null)
                {
                    return new ExecutorMetadata<TCommand>(new MockExecutor<TCommand>((cont, cmd) =>
                    {
                        var result = handler(cmd);
                        if(result == null) return;
                        var type = cmd.GetType();
                        foreach(var interfaceType in type.GetInterfaces())
                        {
                            if(interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ICommand<>))
                            {
                                var resultType = interfaceType.GetGenericArguments()[0];
                                if(resultType.IsAnonymous())
                                {
                                    result = new AnonymousTypeObject(resultType).CreateAny(result);
                                }
                                break;
                            }
                        }
                        TypeMapper.Create(type)["Result"].SetValue(cmd, result);
                    }));
                }
                throw new NotSupportedException($"没有模拟实现命令 { typeof(TCommand).FullName } 的执行器。");
            }
            return executor;
        }

        Queue<Func<ICommand, object>> _queues = new Queue<Func<ICommand, object>>();

        /// <summary>
        /// 添加一个按顺序先进先出的模拟的模型执行方法。
        /// </summary>
        /// <param name="mockHandler">模拟的执行器。</param>
        /// <returns>当前执行器工厂。</returns>
        [Obsolete]
        public MockExecutorFactory Enqueue(Func<ICommand, object> mockHandler)
        {
            _queues.Enqueue(mockHandler);
            return this;
        }

        /// <summary>
        /// 弱类型命令模型，添加一个按顺序先进先出的模拟的模型执行方法。
        /// </summary>
        /// <param name="mockHandler">模拟的执行器。</param>
        /// <returns>当前执行器工厂。</returns>
        public MockExecutorFactory Mock(Func<ICommand, object> mockHandler)
        {
            _queues.Enqueue(mockHandler);
            return this;
        }

        /// <summary>
        /// 强类型命令模型，添加一个按顺序先进先出的模拟的模型执行方法。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="mockHandler">模拟的执行器。</param>
        /// <returns>当前执行器工厂。</returns>
        public MockExecutorFactory Mock<TCommand>(Func<TCommand, object> mockHandler) where TCommand : ICommand
        {
            _queues.Enqueue(c => mockHandler((TCommand)c));
            return this;
        }

        class AnonymousTypeObject
        {
            private readonly System.Reflection.ConstructorInfo _ctor;
            public string Fields { get; }

            public AnonymousTypeObject(Type type)
            {
                if(!type.IsAnonymous()) throw new ArgumentException("只支持匿名对象的类型", type.FullName);
                this._ctor = type.GetConstructors().First();
                this.Fields = this._ctor.GetParameters().Join(p => p.Name);
            }

            public object CreateAny(object value)
            {
                if(value == null) return null;
                var mp = TypeMapper.Create(value.GetType());
                var d = this._ctor.GetParameters().ToDictionary(s => s.Name, s => (object)null);
                foreach(var p in mp.Properties)
                {
                    d[p.Name] = p.GetValue(value);
                }
                return DynamicFactory.CreateConstructorHandler(_ctor)(d.Values.ToArray());
            }
        }


    }

}
