namespace System.Windows.Forms
{
    using System.Threading;

    /// <summary>
    /// 表示一个任务等待的接口。
    /// </summary>
    public interface IWaitingTask : IDisposable
    {
        /// <summary>
        /// 获取一个值，表示等待窗体是否已释放。
        /// </summary>
        bool IsDisposed
        {
            get;
        }

        /// <summary>
        /// 获取一个值，表示等待窗体是否已经呈现给用户。
        /// </summary>
        bool IsShown
        {
            get;
        }

        /// <summary>
        /// 获取或设置一个值，表示修改等待窗体的显示内容时，是否清空计时器。
        /// </summary>
        bool IsTextChangedClearSeconds
        {
            get; set;
        }

        /// <summary>
        /// 获取或设置一个值，表示等待窗体的附属线程。
        /// </summary>
        Thread OwnerThread
        {
            get; set;
        }

        /// <summary>
        /// 获取或设置一个值，表示等待窗体的显示内容。
        /// </summary>
        string Text
        {
            get; set;
        }

        /// <summary>
        /// 获取当前窗体。
        /// </summary>
        Control This
        {
            get;
        }
        /// <summary>
        /// 获取当前调用的父窗体。
        /// </summary>
        Form CallerForm { get; }

        /// <summary>
        /// 清空计时器。
        /// </summary>
        void ClearSeconds();

        /// <summary>
        /// 指定一个父窗体，显示等待窗体。
        /// </summary>
        /// <param name="form">父窗体。</param>
        void ShowForm(Form form);
    }
}