using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型的执行器工厂。
    /// </summary>
    [SingletonMapping]
    public class ExecutorFactory : CommandModelContainerProviderBase, IExecutorFactory
    {
        private const string CommandNameSuffix = "Command";
        private readonly System.Collections.Concurrent.ConcurrentDictionary<Type, IExecutorMetadata>
            Executors = new System.Collections.Concurrent.ConcurrentDictionary<Type, IExecutorMetadata>();

        /// <summary>
        /// 初始化 <see cref="Aoite.CommandModel.ExecutorFactory"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public ExecutorFactory(IIocContainer container) : base(container) { }

        /// <summary>
        /// 创建一个命令模型的执行器元数据。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <returns>返回命令模型的执行器元数据。</returns>
        public virtual IExecutorMetadata<TCommand> Create<TCommand>(TCommand command) where TCommand : ICommand
        {
            if(command == null) throw new ArgumentNullException("command");
            return Singleton<TCommand>.Instance;
        }

        static class Singleton<TCommand> where TCommand : ICommand
        {
            private const string ExecutorNameSuffix = "Executor";
            public readonly static IExecutorMetadata<TCommand> Instance = CreateExecutor();

            static Type CreateCommandExecutorType(Type commandType)
            {
                var bea = commandType.GetAttribute<BindingExecutorAttribute>();
                Type type = null;
                if(bea != null) type = bea.Type;
                else
                {
                    type = commandType.GetNestedType(ExecutorNameSuffix
                       , System.Reflection.BindingFlags.Public
                       | System.Reflection.BindingFlags.NonPublic);
                }
                if(commandType.IsGenericType)
                {
                    var gs = commandType.GetGenericArguments();
                    if(type == null)
                    {
                        var name = commandType.GetGenericTypeDefinition().FullName;
                        var gsLength = gs.Length;
                        var gsSuffix = "`" + gsLength;
                        var commandSuffix = CommandNameSuffix + gsSuffix;
                        if(name.EndsWith(commandSuffix)) name = name.RemoveEnds(commandSuffix.Length);
                        else if(name.EndsWith(gsSuffix)) name = name.RemoveEnds(gsSuffix.Length);

                        type = ObjectFactory.GetType(name + ExecutorNameSuffix + gsSuffix);
                        if(type == null) return null;
                        if(!type.IsGenericType || !type.IsGenericTypeDefinition || type.GetGenericArguments().Length != gsLength)
                            throw new ArgumentException("泛型命令模型 {0} 的执行器 {1}，匹配条件失败（非泛型或参数数量不匹配）。");
                    }
                    type = type.MakeGenericType(gs);
                }
                else if(type == null)
                {
                    var fullName = commandType.FullName;
                    if(fullName.EndsWith(CommandNameSuffix)) fullName = fullName.RemoveEnds(CommandNameSuffix.Length);
                    fullName += ExecutorNameSuffix;
                    type = ObjectFactory.GetType(fullName);
                }
                return type;
            }

            static IExecutorMetadata<TCommand> CreateExecutor()
            {
                var commandType = typeof(TCommand);
                var type = CreateCommandExecutorType(commandType);
                if(type == null) throw new NotSupportedException("命令模型 {0} 找不到需要关联的执行器（可能1：没有实现命令执行；可能2：命令和执行器的命名大小写不一致）。".Fmt(commandType.FullName));
                var executor = Activator.CreateInstance(type) as IExecutor<TCommand>;
                if(executor == null) throw new NotSupportedException("命令模型 {0} 的关联到了非法的执行器（{1}） 。".Fmt(commandType.FullName, type.FullName));

                return new ExecutorMetadata<TCommand>(executor);
            }
        }
    }
}
