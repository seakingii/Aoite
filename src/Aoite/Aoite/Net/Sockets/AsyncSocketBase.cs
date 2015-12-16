using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Net.Sockets
{
    class SocketExceptionEventArgs : ExceptionEventArgs
    {
        /// <summary>
        /// 获取最近使用此上下文对象执行的套接字操作类型。
        /// </summary>
        public SocketAsyncOperation LastOperation { get; }

        /// <summary>
        /// 提供一个错误，初始化一个 <see cref="ExceptionEventArgs "/> 类的新实例。
        /// </summary>
        /// <param name="exception">一个错误。</param>
        public SocketExceptionEventArgs(SocketAsyncOperation lastOperation, Exception exception) : base(exception)
        {
            this.LastOperation = lastOperation;
        }
    }
    class SocketAsyncEventArgsPool : ObjectPool<AdvSocketAsyncEventArgs>
    {
        BlockingBufferManager _bufferManager;
        public SocketAsyncEventArgsPool(BlockingBufferManager bufferManager)
        {
            this._bufferManager = bufferManager;
        }

        protected override AdvSocketAsyncEventArgs OnCreateObject()
        {
            return new AdvSocketAsyncEventArgs();
        }

        public override AdvSocketAsyncEventArgs Acquire()
        {
            var saea = base.Acquire();
            var buffer = this._bufferManager.GetBuffer();
            saea.ArraySegmentBuffer = buffer;
            saea.SetBuffer(buffer.Array, buffer.Offset, buffer.Count);
            return saea;
        }

        private readonly static ArraySegment<byte> Empty = new ArraySegment<byte>(new byte[0], 0, 0);

        public override void Release(AdvSocketAsyncEventArgs obj)
        {
            obj.ArraySegmentBuffer = Empty;
            obj.SetBuffer(null, 0, 0);
            base.Release(obj);
        }
    }
    /// <summary>
    /// 表示一个异步实现 Berkeley 套接字接口的基类。
    /// </summary>
    public abstract class AsyncSocketBase : ObjectDisposableBase
    {
        private long _State = (long)SocketState.Closed;

        /// <summary>
        /// 表示通讯状态发生更改后发生。
        public event SocketStateEventHandler StateChanged;

        /// <summary>
        /// 获取一个值，指示通讯是否正在运行中。
        /// </summary>
        public virtual bool IsRunning
        {
            get
            {
                return GA.LockEquals(ref this._State, (long)SocketState.Opened)
                    || GA.LockEquals(ref this._State, (long)SocketState.Opening);
            }
        }

        /// <summary>
        /// 获取一个值，表示通讯的状态。
        /// </summary>
        public SocketState State
        {
            get
            {
                return (SocketState)GA.LockRead(ref this._State);
            }
        }

        /// <summary>
        /// 打开通讯。
        /// </summary>
        public void Open()
        {
            if(this.IsRunning) return;
            this.SwitchState(SocketState.Opening);
            try
            {
                this.OnOpen();
                this.SwitchState(SocketState.Opened);
            }
            catch(Exception ex)
            {
                this.SwitchState(ex);
                throw;
            }
        }

        /// <summary>
        /// 关闭通讯。
        /// </summary>
        public void Close()
        {
            if(!this.IsRunning) return;
            this.SwitchState(SocketState.Closing);
            try
            {
                this.OnClose();
            }
            catch(Exception ex)
            {
                this.SwitchState(ex);
                throw;
            }
            finally
            {
                this.SwitchState(SocketState.Closed);
            }
        }

        /// <summary>
        /// 打开通讯时发生。
        /// </summary>
        protected abstract void OnOpen();
        /// <summary>
        /// 关闭通讯时发生。
        /// </summary>
        protected abstract void OnClose();

        protected virtual void OnDisconnect(Socket socket, Exception exception)
        {
            //socket.DisconnectAsync(false)
        }

        #region SwitchState

        /// <summary>
        /// 切换通讯状态。
        /// </summary>
        /// <param name="state">通讯的状态。</param>
        protected void SwitchState(SocketState state)
        {
            this.SwitchState(state, null);
        }

        /// <summary>
        /// 切换到异常的通讯状态。
        /// </summary>
        /// <param name="exception">通讯中抛出的异常。</param>
        protected void SwitchState(Exception exception)
        {
            this.SwitchState(SocketState.Failed, exception);
        }

        /// <summary>
        /// 切换通讯状态。
        /// </summary>
        /// <param name="state">通讯的状态。</param>
        /// <param name="exception">通讯中抛出的异常。</param>
        internal protected virtual void SwitchState(SocketState state, Exception exception)
        {
            GA.LockWrite(ref this._State, (long)state);

            var handler = this.StateChanged;
            if(handler != null)
            {
                if(exception != null)
                {
                    handler(this, new SocketStateEventArgs(exception));
                    this.Close();
                }
                else
                {
                    handler(this, new SocketStateEventArgs(state));
                }
            }
        }

        #endregion

        /// <summary>
        /// 套接字异步操作完成的方法。
        /// </summary>
        /// <param name="sender">事件对象。</param>
        /// <param name="e">当前事件参数关联的 <see cref="SocketAsyncEventArgs"/>。</param>
        protected virtual void SocketAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch(e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    this.ProcessAccept(e);
                    break;
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend(e);
                    break;
                case SocketAsyncOperation.Disconnect:
                    this.ProcessDisconnect(e);
                    break;
                default:
                    throw new NotSupportedException("不支持的异步套接字操作的类型。");
            }
        }
        protected virtual void ProcessSend(SocketAsyncEventArgs e) { }

        protected virtual void ProcessDisconnect(SocketAsyncEventArgs e)
        {

        }

        /// <summary>
        /// 开始处理异步接入操作。
        /// </summary>
        /// <param name="e">当前事件参数关联的 <see cref="SocketAsyncEventArgs"/>。</param>
        protected virtual void ProcessAccept(SocketAsyncEventArgs e) { }

        /// <summary>
        /// 开始处理异步接收操作。
        /// </summary>
        /// <param name="e">当前事件参数关联的 <see cref="SocketAsyncEventArgs"/>。</param>
        protected virtual void ProcessReceive(SocketAsyncEventArgs e)
        {
            if(!this.IsRunning) return; //- 服务器已断开

            var acceptSocket = e.AcceptSocket;
            var receiveLength = e.BytesTransferred;
            if(receiveLength == 0) e.SocketError = SocketError.OperationAborted;

            if(e.SocketError != SocketError.Success)
            {
                //var buffer
            }
        }
    }
}
