namespace System.Windows.Forms
{
    /// <summary>
    /// 表示消息框可选框的设置参数。
    /// </summary>
    public class MessageBoxCheckParameters : MessageBoxTextParameters
    {
        #region Constructors

        /// <summary>
        /// 初始化 <see cref="MessageBoxCheckParameters"/> 的新实例。
        /// </summary>
        public MessageBoxCheckParameters()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 获取或设置一个值，表示可选框的文本。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                this.Visible = !string.IsNullOrEmpty(value);
                base.Text = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示是否显示可选框。
        /// </summary>
        public bool Visible
        {
            get; set;
        }

        #endregion Properties
    }
}