namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;

    /// <summary>
    /// 表示 <see cref="System.Windows.Forms.Msg.ShowingMsg"/> 事件的委托。
    /// </summary>
    /// <param name="sender">事件拥有者。</param>
    /// <param name="e">事件的参数。</param>
    public delegate void ShowingMsgEventHandler(object sender, ShowingMsgEventArgs e);

    /// <summary>
    /// 显示可包含文本、按钮和符号（通知并指示用户）的消息框。
    /// </summary>
    public static class Msg
    {
        #region Global Fields

        /// <summary>
        /// 获取或设置一个值，表示消息窗口的异常标题。
        /// </summary>
        public static string DefaultErrorCaption = "系统发出一个错误";

        /// <summary>
        /// 获取或设置一个值，表示消息窗口的信息标题。
        /// </summary>
        public static string DefaultInformationCaption = "系统发出一个信息";

        /// <summary>
        /// 获取或设置一个值，表示消息窗口的窗体文本。
        /// </summary>
        public static string DefaultMsgFormText = "系统提醒";

        /// <summary>
        /// 获取或设置一个值，表示消息窗口的请求标题。
        /// </summary>
        public static string DefaultQuestionCaption = "系统发出一个请求";

        /// <summary>
        /// 获取或设置一个值，表示消息窗口的重试标题。
        /// </summary>
        public static string DefaultRetryCaption = "系统发出一个重试";

        /// <summary>
        /// 获取或设置一个值，表示进度条窗口的显示内容。
        /// </summary>
        public static string DefaultWaitText = "系统正在执行操作...";

        /// <summary>
        /// 获取或设置一个值，表示消息窗口的警告标题。
        /// </summary>
        public static string DefaultWarnCaption = "系统发出一个警告";

        /// <summary>
        /// 显示消息时发生。
        /// </summary>
        public static event ShowingMsgEventHandler ShowingMsg;

        #endregion

        #region InputBox

        /// <summary>
        /// 显示一个输入框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="displayInfo">显示的消息。</param>
        /// <param name="editCaptions">一系列的文本输入框标题。</param>
        /// <returns>返回输入框的值。</returns>
        public static InputBoxResult InputBox(this Control owner, string displayInfo, params string[] editCaptions)
        {
            var parameters = new InputBoxParameters(displayInfo, editCaptions);

            return Msg.InputBox(owner, parameters);
        }

        /// <summary>
        /// 显示一个输入框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="parameters">输入框参数。</param>
        /// <returns>返回输入框的值。</returns>
        public static InputBoxResult InputBox(this Control owner, InputBoxParameters parameters)
        {
            if (owner != null && owner.InvokeRequired)
            {
                return (InputBoxResult)owner.Invoke(new Func<Control, InputBoxParameters, InputBoxResult>(InputBox), owner, parameters);
            }
            else
            {
                var form = new InputBoxForm(owner, parameters);
                if (owner == null)
                {
                    return new InputBoxResult(parameters.Editors, form);
                }
                var ownerForm = owner.FindForm();
                ownerForm.Activate();
                var r = new InputBoxResult(parameters.Editors, form);
                ownerForm.Activate();
                return r;
            }
        }

        #endregion

        #region IsSuccess

        /// <summary>
        /// 判断一个返回结果是否成功。如果失败，将会弹出异常信息。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="result">返回结果。</param>
        /// <returns>返回结果的状态。</returns>
        public static bool IsSuccess(this Control owner, IResult result)
        {
            if (result != null)
            {
                if (result.IsSucceed) return true;
                Msg.ShowError(result.ToString());
            }
            return false;
        }

        /// <summary>
        /// 判断一个返回结果是否成功。如果失败，将会弹出异常信息。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="result">返回结果。</param>
        /// <param name="prefix">前缀内容。</param>
        /// <param name="suffix">后缀内容。</param>
        /// <returns>返回结果的状态。</returns>
        public static bool IsSuccess(this Control owner, IResult result, string prefix, string suffix = null)
        {
            if (result != null)
            {
                if (result.IsSucceed) return true;
                Msg.ShowError(owner, prefix + result.ToString() + suffix);
            }
            return false;
        }

        #endregion

        #region ShowError

        /// <summary>
        /// 显示包含“确定”的错误消息框。
        /// </summary>
        /// <param name="content">消息内容。</param>
        public static void ShowError(string content)
        {
            Msg.ShowError(null, content);
        }

        /// <summary>
        /// 显示包含“确定”的错误消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        public static void ShowError(this Control owner, string content)
        {
            Msg.ShowMsg(owner, MessageBoxParameters.CreateErrorParameters(content));
        }

        /// <summary>
        /// 显示包含“确定”的错误消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        public static void ShowError(this Control owner, string content, string caption)
        {
            Msg.ShowMsg(owner, MessageBoxParameters.CreateErrorParameters(content, caption));
        }

        #endregion

        #region ShowInfo

        /// <summary>
        /// 显示包含“确定”的信息消息框。
        /// </summary>
        /// <param name="content">消息内容。</param>
        public static void ShowInfo(string content)
        {
            Msg.ShowInfo(null, content);
        }

        /// <summary>
        /// 显示包含“确定”的信息消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        public static void ShowInfo(this Control owner, string content)
        {
            Msg.ShowMsg(owner, MessageBoxParameters.CreateInfoParameters(content));
        }

        /// <summary>
        /// 显示包含“确定”的信息消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        public static void ShowInfo(this Control owner, string content, string caption)
        {
            Msg.ShowMsg(owner, MessageBoxParameters.CreateInfoParameters(content, caption));
        }

        #endregion

        #region ShowYesNo

        /// <summary>
        /// 显示包含“是”和“否”的请求消息框。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回一个值，表示是否是。</returns>
        public static bool ShowYesNo(string content)
        {
            return Msg.ShowYesNo(null, content);
        }

        /// <summary>
        /// 显示包含“是”和“否”的请求消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>返回一个值，表示是否是。</returns>
        public static bool ShowYesNo(this Control owner, string content)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateYesNoParameters(content)) == DialogResult.Yes;
        }

        /// <summary>
        /// 显示包含“是”和“否”的请求消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回一个值，表示是否是。</returns>
        public static bool ShowYesNo(this Control owner, string content, string caption)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateYesNoParameters(content, caption)) == DialogResult.Yes;
        }

        #endregion

        #region ShowRetry

        /// <summary>
        /// 显示包含“重试”和“取消”的请求消息框。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回一个值，表示是否重试。</returns>
        public static bool ShowRetry(string content)
        {
            return Msg.ShowRetry(null, content);
        }

        /// <summary>
        /// 显示包含“重试”和“取消”的请求消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>返回一个值，表示是否重试。</returns>
        public static bool ShowRetry(this Control owner, string content)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateRetryParameters(content)) == DialogResult.Retry;
        }

        /// <summary>
        /// 显示包含“重试”和“取消”的请求消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回一个值，表示是否重试。</returns>
        public static bool ShowRetry(this Control owner, string content, string caption)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateRetryParameters(content, caption)) == DialogResult.Retry;
        }

        #endregion

        #region ShowWarn

        /// <summary>
        /// 显示包含“继续”和“取消”的请求消息框。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回一个值，表示是否确定。</returns>
        public static bool ShowWarn(string content)
        {
            return Msg.ShowWarn(null, content);
        }

        /// <summary>
        /// 显示包含“继续”和“取消”的请求消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>返回一个值，表示是否确定。</returns>
        public static bool ShowWarn(this Control owner, string content)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateWarnParameters(content)) == DialogResult.OK;
        }

        /// <summary>
        /// 显示包含“继续”和“取消”的请求消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回一个值，表示是否确定。</returns>
        public static bool ShowWarn(this Control owner, string content, string caption)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateWarnParameters(content, caption)) == DialogResult.OK;
        }

        #endregion

        #region ShowYesNoCancel

        /// <summary>
        /// 显示包含“是”、“否”和“取消”的请求消息框。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回一个值，表示是否是。</returns>
        public static DialogResult ShowYesNoCancel(string content)
        {
            return Msg.ShowYesNoCancel(null, content);
        }

        /// <summary>
        /// 显示包含“是”、“否”和“取消”的请求消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>返回一个值，表示是否是。</returns>
        public static DialogResult ShowYesNoCancel(this Control owner, string content)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateYesNoCancelParameters(content));
        }

        /// <summary>
        /// 显示包含“是”、“否”和“取消”的请求消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回一个值，表示是否是。</returns>
        public static DialogResult ShowYesNoCancel(this Control owner, string content, string caption)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateYesNoCancelParameters(content, caption));
        }

        #endregion

        #region ShowYesNoCancel

        /// <summary>
        /// 显示包含“中止”、“重试”和“忽略”的请求消息框。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <returns>返回一个值，表示是否是。</returns>
        public static DialogResult ShowAbortRetryIgnore(string content)
        {
            return Msg.ShowAbortRetryIgnore(null, content);
        }

        /// <summary>
        /// 显示包含“中止”、“重试”和“忽略”的请求消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>返回一个值，表示是否是。</returns>
        public static DialogResult ShowAbortRetryIgnore(this Control owner, string content)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateAbortRetryIgnoreParameters(content));
        }

        /// <summary>
        /// 显示包含“中止”、“重试”和“忽略”的请求消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回一个值，表示是否是。</returns>
        public static DialogResult ShowAbortRetryIgnore(this Control owner, string content, string caption)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateAbortRetryIgnoreParameters(content, caption));
        }

        #endregion

        #region ShowMsg

        /// <summary>
        /// 显示一个消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>返回窗体的操作结果。</returns>
        public static DialogResult ShowMsg(this Control owner, string content)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateParameters(content));
        }

        /// <summary>
        /// 显示一个消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <returns>返回窗体的操作结果。</returns>
        public static DialogResult ShowMsg(this Control owner, string content, string caption)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateParameters(content, caption));
        }

        /// <summary>
        /// 显示一个消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <param name="buttons">消息按钮。</param>
        /// <returns>返回窗体的操作结果。</returns>
        public static DialogResult ShowMsg(this Control owner, string content, string caption, MessageBoxButtons buttons)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateParameters(content, caption, buttons));
        }

        /// <summary>
        /// 显示一个消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <param name="buttons">消息按钮。</param>
        /// <param name="messageBoxIcon">消息图标。</param>
        /// <returns>返回窗体的操作结果。</returns>
        public static DialogResult ShowMsg(this Control owner, string content, string caption, MessageBoxButtons buttons, MessageBoxIconEx messageBoxIcon)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateParameters(content, caption, buttons, messageBoxIcon));
        }

        /// <summary>
        /// 显示一个消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="caption">消息标题。</param>
        /// <param name="buttons">消息按钮。</param>
        /// <param name="messageBoxIcon">消息图标。</param>
        /// <param name="defaultButton">默认按钮。</param>
        /// <returns>返回窗体的操作结果。</returns>
        public static DialogResult ShowMsg(this Control owner, string content, string caption, MessageBoxButtons buttons, MessageBoxIconEx messageBoxIcon, MessageBoxDefaultButton defaultButton)
        {
            return Msg.ShowMsg(owner, MessageBoxParameters.CreateParameters(content, caption, buttons, messageBoxIcon, defaultButton));
        }

        /// <summary>
        /// 显示一个消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="parameters">设置参数。</param>
        /// <returns>返回窗体的操作结果。</returns>
        public static DialogResult ShowMsg(this Control owner, MessageBoxParameters parameters)
        {
            if (owner == null)
                owner = Form.ActiveForm;
            if (owner != null && owner.InvokeRequired)
            {
                return (DialogResult)owner.Invoke(new Func<Control, MessageBoxParameters, DialogResult>(ShowMsg), owner, parameters);
            }
            else
            {
                if (ShowingMsg != null)
                {
                    var e = new ShowingMsgEventArgs(owner, parameters);
                    ShowingMsg(owner, e);
                    if (e.Result != DialogResult.OK)
                        return e.Result;
                }
                if (owner == null)
                {
                    var f = new MessageBoxForm(owner, parameters);
                    var r = f.ShowDialog();
                    parameters.CheckedResult = f.CheckedResult;
                    return r;
                }
                else
                {
                    var ownerForm = owner.FindForm();

                    ownerForm.Activate();
                    var f = new MessageBoxForm(owner, parameters);
                    var r = f.ShowDialog(owner);
                    ownerForm.Activate();
                    parameters.CheckedResult = f.CheckedResult;
                    return r;
                }
            }
        }


        #endregion

        #region ShowLabel


        /// <summary>
        /// 显示一个定时标签消息框。
        /// </summary>
        /// <param name="content">消息内容。</param>
        public static void ShowLabel(string content)
        {
            Msg.ShowLabel(null, content);
        }

        /// <summary>
        /// 显示一个定时标签消息框。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <param name="messageBoxIcon">显示的图标。</param>
        public static void ShowLabel(string content, MessageBoxIconEx messageBoxIcon)
        {
            Msg.ShowLabel(null, content, messageBoxIcon);
        }

        /// <summary>
        /// 显示一个定时标签消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        public static void ShowLabel(this Control owner, string content)
        {
            var parameters = new MessageLabelParameters();
            parameters.Content.Text = content;
            Msg.ShowLabel(owner, parameters);
        }

        /// <summary>
        /// 显示一个定时标签消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="alignment">对齐方式。</param>
        /// <param name="isAlignmentScreen">是否对齐于桌面。</param>
        public static void ShowLabel(this Control owner, string content, ContentAlignment alignment, bool isAlignmentScreen = false)
        {
            var parameters = new MessageLabelParameters();
            parameters.Content.Text = content;
            parameters.Alignment = alignment;
            parameters.IsAlignmentScreen = isAlignmentScreen;
            Msg.ShowLabel(owner, parameters);
        }

        /// <summary>
        /// 显示一个定时标签消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="seconds">定时的秒数。</param>
        public static void ShowLabel(this Control owner, string content, int seconds)
        {
            var parameters = new MessageLabelParameters();
            parameters.Content.Text = content;
            parameters.CloseMillisecond = seconds * 1000;
            Msg.ShowLabel(owner, parameters);
        }

        /// <summary>
        /// 显示一个定时标签消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="seconds">定时的秒数。</param>
        /// <param name="messageBoxIcon">显示的图标。</param>
        public static void ShowLabel(this Control owner, string content, int seconds, MessageBoxIconEx messageBoxIcon)
        {
            var parameters = new MessageLabelParameters();
            parameters.Content.Text = content;
            parameters.CloseMillisecond = seconds * 1000;
            parameters.Icon.MessageBoxIcon = messageBoxIcon;
            Msg.ShowLabel(owner, parameters);
        }

        /// <summary>
        /// 显示一个定时标签消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="messageBoxIcon">显示的图标。</param>
        public static void ShowLabel(this Control owner, string content, MessageBoxIconEx messageBoxIcon)
        {
            var parameters = new MessageLabelParameters();
            parameters.Content.Text = content;
            parameters.Icon.MessageBoxIcon = messageBoxIcon;
            Msg.ShowLabel(owner, parameters);
        }

        /// <summary>
        /// 显示一个定时标签消息框。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="parameters">设置参数。</param>
        public static void ShowLabel(this Control owner, MessageLabelParameters parameters)
        {
            if (owner != null && owner.InvokeRequired)
            {
                owner.Invoke(new Action<Control, MessageLabelParameters>(ShowLabel), owner, parameters);
            }
            else
            {
                if (owner == null)
                {
                    new MessageLabelForm(null, parameters).Show();
                    return;
                }
                var waitingTask = owner as IWaitingTask;
                var ownerForm = waitingTask == null ? owner.FindForm() : waitingTask.CallerForm;
                if (ownerForm == null)
                    return;

                if (ownerForm.InvokeRequired) new MessageLabelForm(null, parameters).Show();
                else
                {
                    ownerForm.Activate();
                    new MessageLabelForm(owner, parameters).Show(ownerForm);
                    ownerForm.Activate();
                }
            }
        }


        #endregion
    }

    /// <summary>
    /// 表示 <see cref="System.Windows.Forms.Msg.ShowingMsg"/> 事件的参数。
    /// </summary>
    public class ShowingMsgEventArgs : EventArgs
    {
        private Control _Owner;
        private MessageBoxParameters _Parameters;
        private DialogResult _Result = DialogResult.OK;

        /// <summary>
        /// 初始化 <see cref="System.Windows.Forms.ShowingMsgEventArgs"/> 的新实例。
        /// </summary>
        /// <param name="owner">消息的拥有者。</param>
        /// <param name="parameters">消息的参数。</param>
        public ShowingMsgEventArgs(Control owner, MessageBoxParameters parameters)
        {
            this._Owner = owner;
            this._Parameters = parameters;
        }

        /// <summary>
        /// 获取消息的拥有者。
        /// </summary>
        public Control Owner
        {
            get
            {
                return this._Owner;
            }
        }

        /// <summary>
        /// 获取消息的参数。
        /// </summary>
        public MessageBoxParameters Parameters
        {
            get
            {
                return this._Parameters;
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示是否中止消息。如果为 <see cref="System.Windows.Forms.DialogResult.OK"/> 则表示允许消息继续，默认为 <see cref="System.Windows.Forms.DialogResult.OK"/>。
        /// </summary>
        public DialogResult Result
        {
            get
            {
                return this._Result;
            }
            set
            {
                this._Result = value;
            }
        }
    }
}