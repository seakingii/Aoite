namespace System.Windows.Forms
{
    /// <summary>
    /// 表示显示框的设置参数。
    /// </summary>
    public class MessageParametersBase
    {
        #region Fields

        private MessageBoxTextParameters _Content;
        private MessageBoxTextParameters _Form;
        private MessageBoxIconParameters _Icon;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 获取一个值，表示消息框的内容设置。
        /// </summary>
        public MessageBoxTextParameters Content
        {
            get
            {
                if(this._Content == null) this._Content = new MessageBoxTextParameters();
                return this._Content;
            }
        }

        /// <summary>
        /// 获取一个值，表示消息框的窗体设置。
        /// </summary>
        public MessageBoxTextParameters Form
        {
            get
            {
                if(this._Form == null) this._Form = new MessageBoxTextParameters()
                {
                    Text = Msg.DefaultMsgFormText
                };
                return this._Form;
            }
        }

        /// <summary>
        /// 获取一个值，表示消息框的图标设置。
        /// </summary>
        public MessageBoxIconParameters Icon
        {
            get
            {
                if(this._Icon == null) this._Icon = new MessageBoxIconParameters();
                return this._Icon;
            }
        }

        #endregion Properties
    }
}