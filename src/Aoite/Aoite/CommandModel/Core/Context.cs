using Aoite.Data;
using System;
using System.Collections.Specialized;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个执行命令模型的上下文。
    /// </summary>
    public class Context : CommandModelContainerProviderBase, IContext
    {
        /// <summary>
        /// 获取正在执行的命令模型。
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// 获取执行命令模型的用户。该属性可能返回 null 值。
        /// </summary>
        public virtual dynamic User { get { return this.Container.GetUser(); } }

        private Lazy<HybridDictionary> _Data;
        /// <summary>
        /// 获取执行命令模型的其他参数，参数名称若为字符串则不区分大小写的序号字符串比较。
        /// </summary>
        public virtual HybridDictionary Data { get { return _Data.Value; } }

        private Lazy<IDbEngine> _LazyEngine;
        /// <summary>
        /// 获取上下文中的 <see cref="IDbEngine"/> 实例。该实例应不为 null 值，且线程唯一。
        /// <para>* 不应在执行器中开启事务。</para>
        /// </summary>
        public IDbEngine Engine { get { return _LazyEngine.Value; } }

        /// <summary>
        /// 初始化 <see cref="Context"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        /// <param name="command">命令模型。</param>
        public Context(IIocContainer container, ICommand command)
            : base(container)
        {
            if(command == null) throw new ArgumentNullException(nameof(command));
            this.Command = command;
            this._Data = new Lazy<HybridDictionary>(() => new HybridDictionary(true));
            this._LazyEngine = new Lazy<IDbEngine>(() => this.GetDbEngine());
        }

        /// <summary>
        /// 获取或设置键的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>一个值。</returns>
        public object this[object key] { get { return Data[key]; } set { Data[key] = value; } }
    }
}
