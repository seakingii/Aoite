namespace System.Windows.Forms
{
    using System.Drawing;

    /// <summary>
    /// 表示消息框可选框的设置参数。
    /// </summary>
    public class MessageBoxIconParameters
    {
        #region Fields

        private Bitmap _Image;
        private MessageBoxIconEx _MessageBoxIcon;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="MessageBoxIconParameters"/> 的新实例。
        /// </summary>
        public MessageBoxIconParameters()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 获取或设置一个值，表示消息框的图标。
        /// </summary>
        public Bitmap Image
        {
            get { return this._Image ?? (this._Image = this._MessageBoxIcon.ToBitmap()); } set { this._Image = value; }
        }

        /// <summary>
        /// 获取或设置一个值，表示消息框的图标。
        /// </summary>
        public MessageBoxIconEx MessageBoxIcon
        {
            get { return this._MessageBoxIcon; }
            set { this._MessageBoxIcon = value; }
        }

        #endregion Properties

    }
}