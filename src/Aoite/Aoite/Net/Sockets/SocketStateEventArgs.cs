using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net.Sockets
{
    /// <summary>
    /// 表示通讯状态发生更改后发生的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void SocketStateEventHandler(object sender, SocketStateEventArgs e);

    /// <summary>
    /// 表示通讯状态发生更改后发生的事件参数。
    /// </summary>
    public class SocketStateEventArgs : EventArgs
    {
        /// <summary>
        /// 获取一个值，表示通讯更改后的状态。
        /// </summary>
        public SocketState State { get; }
        /// <summary>
        /// 获取一个值，表示通讯过程中抛出的异常。
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 提供通讯状态，初始化 <see cref="SocketStateEventArgs"/> 类的新实例。
        /// </summary>
        /// <param name="state">通讯更改后的状态。</param>
        public SocketStateEventArgs(SocketState state)
        {
            if(state == SocketState.Failed) throw new ArgumentException("没有提供异常信息不能将通讯状态设置为 Failed", nameof(state));
            this.State = state;
        }

        /// <summary>
        /// 提供通讯过程中抛出的异常，初始化 <see cref="SocketStateEventArgs"/> 类的新实例。
        /// </summary>
        /// <param name="exception">通讯过程中抛出的异常。</param>
        public SocketStateEventArgs(Exception exception)
        {
            if(exception == null) throw new ArgumentNullException(nameof(exception));

            this.State = SocketState.Failed;
            this.Exception = exception;
        }
    }
}
