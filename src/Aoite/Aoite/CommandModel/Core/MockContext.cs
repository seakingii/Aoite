using System;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个模拟执行的命令模型的上下文。
    /// </summary>
    public class MockContext : Context
    {
        /// <summary>
        /// 获取执行命令模型的用户。该属性可能返回 null 值。
        /// </summary>
        public override dynamic User { get; }

        /// <summary>
        /// 初始化一个 <see cref="Context"/> 类的新实例。
        /// </summary>
        /// <param name="user">模拟的用户。</param>
        /// <param name="command">命令模型。</param>
        /// <param name="container">服务容器。</param>
        /// <param name="lazyEngine">延迟模式的上下文中的 <see cref="IDbEngine"/> 实例。</param>
        public MockContext(object user, IIocContainer container, ICommand command, Lazy<Data.IDbEngine> lazyEngine)
            : base(container, command, new Lazy<System.Collections.Specialized.HybridDictionary>(), lazyEngine)
        {
            this.User = user;
        }
    }
}
