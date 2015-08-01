using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令模型的基础服务。
    /// </summary>
    public interface ICommandModelService : IObjectDisposable, IContainerProvider
    {
        /// <summary>
        /// 获取执行命令模型的用户。该属性可能返回 null 值。
        /// </summary>
        dynamic User { get; }
    }
}
