using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义一个事务。
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// 提交事务。
        /// </summary>
        void Commit();
    }
}
