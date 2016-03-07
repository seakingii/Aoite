using System;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个包含服务容器的命令模型基类。
    /// </summary>
    public abstract class CommandModelContainerProviderBase : IContainerProvider
    {
        private IIocContainer _Container;
        /// <summary>
        /// 获取或设置命令模型的服务容器。
        /// </summary>
        public virtual IIocContainer Container
        {
            get { return _Container; }
            set
            {
                if(value == null) throw new ArgumentNullException(nameof(value));
                this._Container = value;
            }
        }

        /// <summary>
        /// 初始化一个 <see cref="CommandModelContainerProviderBase"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public CommandModelContainerProviderBase(IIocContainer container)
        {
            if(container == null) throw new ArgumentNullException(nameof(container));
            this._Container = container;
        }
    }
}
