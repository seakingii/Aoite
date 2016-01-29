namespace System.Windows.Forms
{
    using System.Drawing;

    /// <summary>
    /// 表示消息框文本的设置参数。
    /// </summary>
    public class MessageBoxTextParameters
    {
        #region Constructors

        /// <summary>
        /// 初始化 <see cref="MessageBoxTextParameters"/> 的新实例。
        /// </summary>
        public MessageBoxTextParameters()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 获取或设置一个值，表示消息框的背景色。
        /// </summary>
        public Color? BackColor
        {
            get; set;
        }

        /// <summary>
        /// 获取或设置一个值，表示消息框的字体。
        /// </summary>
        public Font Font
        {
            get; set;
        }

        /// <summary>
        /// 获取或设置一个值，表示消息框的前景色。
        /// </summary>
        public Color? ForeColor
        {
            get; set;
        }

        /// <summary>
        /// 获取或设置一个值，表示消息框的文本。
        /// </summary>
        public virtual string Text
        {
            get; set;
        }

        #endregion Properties
    }
}