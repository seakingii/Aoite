using System;
using System.Collections.Specialized;
using Aoite.Data;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令模型上下文的工厂。
    /// </summary>
    public interface IContextFactory : IContainerProvider
    {
        /// <summary>
        /// 创建一个命令模型的上下文。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="lazyData">延迟模式的命令模型的其他参数。</param>
        /// <param name="lazyEngine">延迟模式的上下文中的 <see cref="IDbEngine"/> 实例。</param>
        /// <returns>命令模型的上下文。</returns>
        IContext Create<TCommand>(TCommand command, Lazy<HybridDictionary> lazyData, Lazy<IDbEngine> lazyEngine) where TCommand : ICommand;
    }
}
