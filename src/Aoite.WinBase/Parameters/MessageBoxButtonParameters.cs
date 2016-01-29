namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// 表示消息框按钮的设置参数。
    /// </summary>
    public class MessageBoxButtonParameters : MessageBoxCheckParameters
    {
        #region Constructors

        /// <summary>
        /// 初始化 <see cref="MessageBoxCheckParameters"/> 的新实例。
        /// </summary>
        public MessageBoxButtonParameters()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 获取或设置一个值，表示按钮的返回值。
        /// </summary>
        public DialogResult Result
        {
            get; set;
        }

        #endregion Properties
    }
}