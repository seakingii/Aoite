namespace System.Windows.Forms
{
    /// <summary>
    /// 表示消息框的设置参数。
    /// </summary>
    public class MessageBoxParameters : MessageParametersBase
    {
        #region Fields

        private MessageBoxButtonParameters _Button1;
        private MessageBoxButtonParameters _Button2;
        private MessageBoxButtonParameters _Button3;
        private MessageBoxButtons _Buttons;
        private MessageBoxTextParameters _Caption;
        private MessageBoxCheckParameters _Check;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 获取一个值，表示消息框的按钮 1 设置。
        /// </summary>
        public MessageBoxButtonParameters Button1
        {
            get
            {
                if(this._Button1 == null) this._Button1 = new MessageBoxButtonParameters();
                return this._Button1;
            }
        }

        /// <summary>
        /// 获取一个值，表示消息框的按钮 2 设置。
        /// </summary>
        public MessageBoxButtonParameters Button2
        {
            get
            {
                if(this._Button2 == null) this._Button2 = new MessageBoxButtonParameters();
                return this._Button2;
            }
        }

        /// <summary>
        /// 获取一个值，表示消息框的按钮 3 设置。
        /// </summary>
        public MessageBoxButtonParameters Button3
        {
            get
            {
                if(this._Button3 == null) this._Button3 = new MessageBoxButtonParameters();
                return this._Button3;
            }
        }

        /// <summary>
        /// 获取一个值，表示消息框的按钮设置。
        /// </summary>
        public MessageBoxButtons Buttons
        {
            get { return this._Buttons; }
            set
            {
                switch(value)
                {
                    case MessageBoxButtons.AbortRetryIgnore:
                        this.Button1.Text = "中止(&A)"; this.Button1.Result = DialogResult.Abort;
                        this.Button2.Text = "重试(&R)"; this.Button2.Result = DialogResult.Retry;
                        this.Button3.Text = "忽略(&I)"; this.Button3.Result = DialogResult.Ignore;
                        this.EscResult = DialogResult.Abort;
                        break;
                    case MessageBoxButtons.OK:
                        this.Button1.Text = "确定(&O)"; this.Button1.Result = DialogResult.OK;
                        this.EscResult = DialogResult.OK;
                        break;
                    case MessageBoxButtons.OKCancel:
                        this.Button1.Text = "确定(&O)"; this.Button1.Result = DialogResult.OK;
                        this.Button2.Text = "取消(&C)"; this.Button2.Result = DialogResult.Cancel;
                        this.EscResult = DialogResult.Cancel;
                        break;
                    case MessageBoxButtons.RetryCancel:
                        this.Button1.Text = "重试(&R)"; this.Button1.Result = DialogResult.Retry;
                        this.Button2.Text = "取消(&C)"; this.Button2.Result = DialogResult.Cancel;
                        this.EscResult = DialogResult.Cancel;
                        break;
                    case MessageBoxButtons.YesNo:
                        this.Button1.Text = "是(&Y)"; this.Button1.Result = DialogResult.Yes;
                        this.Button2.Text = "否(&N)"; this.Button2.Result = DialogResult.No;
                        this.EscResult = DialogResult.No;
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        this.Button1.Text = "是(&Y)"; this.Button1.Result = DialogResult.Yes;
                        this.Button2.Text = "否(&N)"; this.Button2.Result = DialogResult.No;
                        this.Button3.Text = "取消(&C)"; this.Button3.Result = DialogResult.Cancel;
                        this.EscResult = DialogResult.Cancel;
                        break;
                    default:
                        break;
                }
                this._Buttons = value;
            }
        }

        /// <summary>
        /// 获取一个值，表示消息框的标题设置。
        /// </summary>
        public MessageBoxTextParameters Caption
        {
            get
            {
                if(this._Caption == null) this._Caption = new MessageBoxTextParameters() { Text = "温馨提示" };
                return this._Caption;
            }
        }

        /// <summary>
        /// 获取一个值，表示消息框的可选框设置。
        /// </summary>
        public MessageBoxCheckParameters Check
        {
            get
            {
                if(this._Check == null)
                {
                    this._Check = new MessageBoxCheckParameters() { Text = "不再显示", Visible = false };
                }
                return this._Check;
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示消息框的默认按钮。
        /// </summary>
        public MessageBoxDefaultButton DefaultButton { get; set; }

        /// <summary>
        /// 获取或设置一个值，表示当按了 Esc 时的返回值。
        /// </summary>
        public DialogResult EscResult { get; set; }

        /// <summary>
        /// 表示可选框的返回值。
        /// </summary>
        public bool CheckedResult { get; set; }

        #endregion Properties

        /// <summary>
        /// 初始化 <see cref="System.Windows.Forms.MessageBoxParameters"/> 的新实例。
        /// </summary>
        public MessageBoxParameters() { }

        #region CreateParameters

        /// <summary>
        /// 创建一个消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateParameters(string content)
        {
            var parameters = new MessageBoxParameters();
            parameters.Content.Text = content;
            return parameters;
        }

        /// <summary>
        /// 创建一个消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateParameters(string content, string caption)
        {
            var parameters = new MessageBoxParameters();
            parameters.Content.Text = content;
            parameters.Caption.Text = caption;
            return parameters;
        }

        /// <summary>
        /// 创建一个消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <param name="buttons">消息按钮。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateParameters(string content, string caption, MessageBoxButtons buttons)
        {
            var parameters = new MessageBoxParameters();
            parameters.Content.Text = content;
            parameters.Caption.Text = caption;
            parameters.Buttons = buttons;
            return parameters;
        }

        /// <summary>
        /// 创建一个消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <param name="buttons">消息按钮。</param>
        /// <param name="messageBoxIcon">消息图标。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateParameters(string content, string caption, MessageBoxButtons buttons, MessageBoxIconEx messageBoxIcon)
        {
            var parameters = new MessageBoxParameters();
            parameters.Content.Text = content;
            parameters.Caption.Text = caption;
            parameters.Buttons = buttons;
            parameters.Icon.MessageBoxIcon = messageBoxIcon;
            return parameters;
        }

        /// <summary>
        /// 创建一个消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <param name="buttons">消息按钮。</param>
        /// <param name="messageBoxIcon">消息图标。</param>
        /// <param name="defaultButton">默认按钮。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateParameters(string content, string caption, MessageBoxButtons buttons, MessageBoxIconEx messageBoxIcon, MessageBoxDefaultButton defaultButton)
        {
            var parameters = new MessageBoxParameters();
            parameters.Content.Text = content;
            parameters.Caption.Text = caption;
            parameters.Buttons = buttons;
            parameters.Icon.MessageBoxIcon = messageBoxIcon;
            parameters.DefaultButton = defaultButton;
            return parameters;
        }

        #endregion

        #region CreateErrorParameters

        /// <summary>
        /// 创建一个包含“确定”的错误消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateErrorParameters(string content)
        {
            return CreateErrorParameters(content, Msg.DefaultErrorCaption);
        }

        /// <summary>
        /// 创建一个包含“确定”的错误消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateErrorParameters(string content, string caption)
        {
            return CreateParameters(content, caption, MessageBoxButtons.OK, MessageBoxIconEx.Error);
        }

        #endregion

        #region CreateInfoParameters

        /// <summary>
        /// 创建一个包含“确定”的信息消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateInfoParameters(string content)
        {
            return CreateInfoParameters(content, Msg.DefaultInformationCaption);
        }

        /// <summary>
        /// 创建一个包含“确定”的信息消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateInfoParameters(string content, string caption)
        {
            return CreateParameters(content, caption, MessageBoxButtons.OK, MessageBoxIconEx.Information);
        }

        #endregion

        #region CreateYesNoParameters

        /// <summary>
        /// 创建一个包含“是”和“否”的请求消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateYesNoParameters(string content)
        {
            return CreateYesNoParameters(content, Msg.DefaultQuestionCaption);
        }

        /// <summary>
        /// 创建一个包含“是”和“否”的请求消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateYesNoParameters(string content, string caption)
        {
            return CreateParameters(content, caption, MessageBoxButtons.YesNo, MessageBoxIconEx.Question, MessageBoxDefaultButton.Button2);
        }

        #endregion

        #region CreateRetryParameters

        /// <summary>
        /// 创建一个包含“重试”和“取消”的请求消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateRetryParameters(string content)
        {
            return CreateRetryParameters(content, Msg.DefaultQuestionCaption);
        }

        /// <summary>
        /// 创建一个包含“重试”和“取消”的请求消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateRetryParameters(string content, string caption)
        {
            return CreateParameters(content, caption, MessageBoxButtons.RetryCancel, MessageBoxIconEx.Error, MessageBoxDefaultButton.Button2);
        }

        #endregion

        #region CreateWarnParameters

        /// <summary>
        /// 创建一个包含“继续”和“取消”的请求消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateWarnParameters(string content)
        {
            return CreateWarnParameters(content, Msg.DefaultQuestionCaption);
        }

        /// <summary>
        /// 创建一个包含“继续”和“取消”的请求消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateWarnParameters(string content, string caption)
        {
            var parameters = new MessageBoxParameters();
            parameters.Content.Text = content;
            parameters.Caption.Text = caption;
            parameters.Icon.MessageBoxIcon = MessageBoxIconEx.Warning;
            parameters.Buttons = MessageBoxButtons.OKCancel;
            parameters.Button1.Text = "继续(&G)";
            parameters.DefaultButton = MessageBoxDefaultButton.Button2;

            return parameters;
        }

        #endregion

        #region CreateYesNoCancelParameters

        /// <summary>
        /// 创建一个包含“是”、“否”和“取消”的请求消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateYesNoCancelParameters(string content)
        {
            return CreateYesNoCancelParameters(content, Msg.DefaultQuestionCaption);
        }

        /// <summary>
        /// 创建一个包含“是”、“否”和“取消”的请求消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateYesNoCancelParameters(string content, string caption)
        {
            return CreateParameters(content, caption, MessageBoxButtons.YesNoCancel, MessageBoxIconEx.Question, MessageBoxDefaultButton.Button3);
        }

        #endregion
        #region CreateAbortRetryIgnoreParameters

        /// <summary>
        /// 创建一个包含“中止”、“重试”和“忽略”的请求消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateAbortRetryIgnoreParameters(string content)
        {
            return CreateAbortRetryIgnoreParameters(content, Msg.DefaultQuestionCaption);
        }

        /// <summary>
        /// 创建一个包含“中止”、“重试”和“忽略”的请求消息框参数。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回消息框参数的实例。</returns>
        public static MessageBoxParameters CreateAbortRetryIgnoreParameters(string content, string caption)
        {
            return CreateParameters(content, caption, MessageBoxButtons.AbortRetryIgnore, MessageBoxIconEx.Error, MessageBoxDefaultButton.Button2);
        }

        #endregion
    }
}