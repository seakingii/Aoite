using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个执行命令模型的上下文。
    /// </summary>
    public class Context : CommandModelContainerProviderBase, IContext
    {
        /// <summary>
        /// 获取执行命令模型的用户。该属性可能返回 null 值。
        /// </summary>
        public virtual dynamic User
        {
            get
            {
                return this.Container.GetUser();
            }
        }

        private HybridDictionary _Data;
        /// <summary>
        /// 获取执行命令模型的其他参数，参数名称若为字符串则不区分大小写的序号字符串比较。
        /// </summary>
        public virtual HybridDictionary Data { get { return _Data ?? (_Data = new HybridDictionary(true)); } }

        private ICommand _Command;
        /// <summary>
        /// 获取正在执行的命令模型。
        /// </summary>
        public ICommand Command { get { return _Command; } }

        private Lazy<IDbEngine> _LazyEngine;
        /// <summary>
        /// 获取上下文中的 <see cref="System.IDbEngine"/> 实例。理论上来说，返回值不应为空。
        /// </summary>
        public IDbEngine Engine { get { return _LazyEngine.Value; } }

        /// <summary>
        /// 初始化 <see cref="Aoite.CommandModel.Context"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        /// <param name="command">命令模型。</param>
        public Context(IIocContainer container, ICommand command)
            : base(container)
        {
            if(command == null) throw new ArgumentNullException("command");
            this._Command = command;
            this._LazyEngine = new Lazy<IDbEngine>(this.InitializeEngine);
        }

        private IDbEngine InitializeEngine()
        {
            return this.Container.GetFixedService<IDbEngine>() ?? Db.Context;
        }

        /// <summary>
        /// 获取或设置键的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回一个值。</returns>
        public object this[object key]
        {
            get
            {
                return Data[key];
            }
            set
            {
                Data[key] = value;
            }
        }
    }
}
