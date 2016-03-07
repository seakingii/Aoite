using System;
using System.Collections.Specialized;
using Aoite.Data;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型上下文的工厂。
    /// </summary>
    [SingletonMapping]
    public class ContextFactory : CommandModelContainerProviderBase, IContextFactory
    {
        /// <summary>
        /// 初始化一个 <see cref="ContextFactory"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public ContextFactory(IIocContainer container) : base(container) { }

        /// <summary>
        /// 创建一个命令模型的上下文。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="lazyData">延迟模式的命令模型的其他参数。</param>
        /// <param name="lazyEngine">延迟模式的上下文中的 <see cref="IDbEngine"/> 实例。</param>
        /// <returns>命令模型的上下文。</returns>
        public IContext Create<TCommand>(TCommand command, Lazy<HybridDictionary> lazyData, Lazy<IDbEngine> lazyEngine) where TCommand : ICommand
        {
            return new Context(this.Container, command, lazyData, lazyEngine);
        }
    }
}
