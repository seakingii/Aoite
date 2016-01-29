namespace System.Windows.Forms
{
    #region Enumerations

    /// <summary>
    /// 表示显示在消息框的图标。
    /// </summary>
    public enum MessageBoxIconEx
    {
        /// <summary>
        /// 表示消息框不包含图标。
        /// </summary>
        None = 0,
        /// <summary>
        /// 错误图标 (WIN32: IDI_ERROR)。
        /// </summary>
        Error,
        /// <summary>
        /// 信息图标 (WIN32: IDI_INFORMATION)。
        /// </summary>
        Information,
        /// <summary>
        /// 问号图标 (WIN32: IDI_QUESTION)。
        /// </summary>
        Question,
        /// <summary>
        /// 警告图标 (WIN32: IDI_WARNING)。
        /// </summary>
        Warning,
    }

    #endregion Enumerations
}