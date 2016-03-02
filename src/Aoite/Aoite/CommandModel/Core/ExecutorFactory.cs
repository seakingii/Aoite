using System;
using System.Reflection;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型的执行器工厂。
    /// </summary>
    [SingletonMapping]
    public class ExecutorFactory : CommandModelContainerProviderBase, IExecutorFactory
    {
        private const string CommandNameSuffix = "Command";
        // private readonly System.Collections.Concurrent.ConcurrentDictionary<Type, IExecutorMetadata>
        //    Executors = new System.Collections.Concurrent.ConcurrentDictionary<Type, IExecutorMetadata>();

        /// <summary>
        /// 初始化 <see cref="ExecutorFactory"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public ExecutorFactory(IIocContainer container) : base(container) { }

        /// <summary>
        /// 创建一个命令模型的执行器元数据。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <returns>命令模型的执行器元数据。</returns>
        public virtual IExecutorMetadata<TCommand> Create<TCommand>(TCommand command) where TCommand : ICommand
        {
            if(command == null) throw new ArgumentNullException(nameof(command));

            try
            {
                return Singleton<TCommand>.Instance;
            }
            catch(TypeInitializationException ex)
            {
                if(ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        static class Singleton<TCommand> where TCommand : ICommand
        {
            private const string ExecutorNameSuffix = "Executor";
            public readonly static IExecutorMetadata<TCommand> Instance = CreateExecutor();

            static Type CreateCommandExecutorType(Type commandType)
            {
                var bea = commandType.GetAttribute<BindingExecutorAttribute>();
                var isNestedType = bea == null;
                var type = isNestedType
                    ? commandType.GetNestedType(ExecutorNameSuffix, BindingFlags.Public | BindingFlags.NonPublic) //- 从当前命令的内部类中找到 Executor 类型。
                    : bea.Type; //- 命令通过特性指定了执行器类型

                if(commandType.IsGenericType)
                {
                    var gs = commandType.GetGenericArguments();
                    var gsLength = gs.Length;
                    if(type == null)
                    {
                        isNestedType = false;
                        //- 没有通过特性指定了执行器类型，并且没有内部类“Executor”，那么通过“同命名空间”去寻找
                        var name = commandType.GetGenericTypeDefinition().FullName;
                        var gsSuffix = "`" + gsLength;
                        var commandSuffix = CommandNameSuffix + gsSuffix;
                        if(name.EndsWith(commandSuffix)) name = name.RemoveEnds(commandSuffix.Length);
                        else if(name.EndsWith(gsSuffix)) name = name.RemoveEnds(gsSuffix.Length);

                        //-名字为 xxx.xxx.xxx.TestCommand`n 调整为  xxx.xxx.xxx.TestExecutor`n 
                        type = ObjectFactory.GetType(name + ExecutorNameSuffix + gsSuffix);
                        if(type == null) return null;

                    }
                    /*
                        public class CC1234<T1, T2>  //- 泛型，参数2
                        {
                            public class C1<T3> { }  //- 泛型，参数3
                            public class C2 { }      //- 泛型，参数2
                        }
                    */
                    if(!type.IsGenericType || !type.IsGenericTypeDefinition || type.GetGenericArguments().Length != gsLength)
                        throw new NotSupportedException($"泛型命令模型 { commandType.FullName } 的执行器 { type.FullName }，匹配条件失败（非泛型或参数数量不匹配）。");

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
