namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;

    /// <summary>
    /// 表示定时标签消息框的设置参数。
    /// </summary>
    public class MessageLabelParameters : MessageParametersBase
    {
        #region Fields

        private ContentAlignment _Alignment = ContentAlignment.MiddleCenter;
        private bool _ClickClosed = true;
        private int _CloseMillisecond = 3000;
        private Point? _Point;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 获取或设置一个值，表示消息框的显示位置。 默认 ContentAlignment.MiddleCenter，表示居中显示。
        /// </summary>
        public ContentAlignment Alignment
        {
            get { return this._Alignment; } set { this._Alignment = value; }
        }

        /// <summary>
        /// 获取或设置一个值，表示单击消息框后是否关闭。默认 true，表示单击窗体时关闭窗体。
        /// </summary>
        public bool ClickClosed
        {
            get
            {
                return this._ClickClosed;
            }
            set
            {
                this._ClickClosed = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示消息框的显示时间。 默认 3000 毫秒。
        /// </summary>
        public int CloseMillisecond
        {
            get { return this._CloseMillisecond; }
            set
            {
                if(value < 50) value = 50;
                this._CloseMillisecond = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示消息框的显示位置是基于桌面，还是基于窗体。 默认 false，表示基于窗体。
        /// </summary>
        public bool IsAlignmentScreen
        {
            get; set;
        }

        /// <summary>
        /// 获取或设置一个值，表示消息框的显示坐标。
        /// </summary>
        public Point? Point
        {
            get { return this._Point; } set { this._Point = value; }
        }

        #endregion Properties
    }
}