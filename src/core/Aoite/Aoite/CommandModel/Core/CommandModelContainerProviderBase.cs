using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                if(value == null) throw new ArgumentNullException("value");
                this._Container = value;
            }
        }

        /// <summary>
        /// 初始化 <see cref="Aoite.CommandModel.CommandModelContainerProviderBase"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public CommandModelContainerProviderBase(IIocContainer container)
        {
            if(container == null) throw new ArgumentNullException("container");
            this._Container = container;
        }
    }
}
