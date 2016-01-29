namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    /// <summary>
    /// 表示输入框的参数。
    /// </summary>
    public class InputBoxParameters
    {
        #region Fields

        internal Form _InputBoxForm;

        private string _DisplayInfo;
        private List<Control> _Editors;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 初始化 System.Windows.Forms.InputBoxParameters 的新实例。
        /// </summary>
        public InputBoxParameters()
        {
        }

        /// <summary>
        /// 初始化 System.Windows.Forms.InputBoxParameters 的新实例。
        /// </summary>
        /// <param name="displayInfo">显示的消息。</param>
        /// <param name="editCaptions">一系列的文本输入框标题。</param>
        public InputBoxParameters(string displayInfo, params string[] editCaptions)
        {
            this._DisplayInfo = displayInfo;
            if(editCaptions != null && editCaptions.Length > 0)
            {
                foreach(var editCaption in editCaptions)
                {
                    this.AddTextEdit(editCaption);
                }
            }
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// 取消内容后发生。
        /// </summary>
        public event EventHandler Cancel;

        /// <summary>
        /// 取消内容前发生。
        /// </summary>
        public event CancelEventHandler Canceling;

        /// <summary>
        /// 提交内容后发生。
        /// </summary>
        public event EventHandler Submit;

        /// <summary>
        /// 提交内容前发生。
        /// </summary>
        public event CancelEventHandler Submitting;

        #endregion Events

        #region Properties

        /// <summary>
        /// 获取或设置一个值，表示输入框的显示内容。
        /// </summary>
        public string DisplayInfo
        {
            get
            {
                return this._DisplayInfo;
            }
            set
            {
                this._DisplayInfo = value;
            }
        }

        /// <summary>
        /// 获取一个值，表示输入框的可输入控件集合。
        /// </summary>
        public List<Control> Editors
        {
            get
            {
                if(this._Editors == null) this._Editors = new List<Control>();
                return this._Editors;
            }
        }

        /// <summary>
        /// 获取一个值，表示输入框的窗体。
        /// </summary>
        public Form InputBoxForm
        {
            get
            {
                return this._InputBoxForm;
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// 指定索引获取一个值，表示索引处可输入控件的文本。
        /// </summary>
        /// <param name="index">索引。</param>
        /// <returns>返回索引处的可输入控件的文本。</returns>
        public string this[int index]
        {
            get
            {
                return this.GetText(index);
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// 创建并添加一个指定标题的可输入框控件。
        /// </summary>
        /// <typeparam name="T">可输入框控件的类型。</typeparam>
        /// <param name="title">可输入框控件的标题。</param>
        /// <param name="defValue">默认值。</param>
        /// <returns>返回创建的可输入框控件。</returns>
        public T AddEditor<T>(string title, string defValue = null)
            where T : Control, new()
        {
            return AddControl<T>(new T(), title, defValue);
        }

        /// <summary>
        /// 添加一个指定标题的可输入框控件。
        /// </summary>
        /// <typeparam name="T">可输入框控件的类型。</typeparam>
        /// <param name="control">控件。</param>
        /// <param name="title">可输入框控件的标题。</param>
        /// <param name="defValue">默认值。</param>
        /// <returns>返回控件。</returns>
        public T AddControl<T>(T control, string title, string defValue = null)
            where T : Control
        {
            control.Tag = title;
            if(defValue != null)
            {
                control.Text = defValue;
            }
            this.Editors.Add(control);
            return control;
        }

        /// <summary>
        /// 创建并添加一个指定标题的文本输入框。
        /// </summary>
        /// <param name="title">输入框的标题。</param>
        /// <param name="defValue">默认值。</param>
        /// <returns>返回创建的输入框控件。</returns>
        public TextBox AddTextEdit(string title, string defValue = null)
        {
            return this.AddEditor<TextBox>(title, defValue);
        }

        /// <summary>
        /// 获取指定索引可输入控件。
        /// </summary>
        /// <param name="index">索引。</param>
        /// <returns>返回索引处的可输入控件。</returns>
        public Control GetEditor(int index)
        {
            return this._Editors[index];
        }

        /// <summary>
        /// 获取指定索引可输入控件的 Text 值。
        /// </summary>
        /// <param name="index">索引。</param>
        /// <returns>返回索引处的可输入控件的 Text 值。</returns>
        public string GetText(int index)
        {
            return this._Editors[index].Text;
        }

        internal void OnCancel()
        {
            if(this.Cancel != null) this.Cancel(this, EventArgs.Empty);
        }

        internal void OnCanceling(CancelEventArgs e)
        {
            if(this.Canceling != null) this.Canceling(this, e);
        }

        internal void OnSubmit()
        {
            if(this.Submit != null) this.Submit(this, EventArgs.Empty);
        }

        internal void OnSubmitting(CancelEventArgs e)
        {
            if(this.Submitting != null) this.Submitting(this, e);
        }

        #endregion Methods
    }

    /// <summary>
    /// 表示输入框的返回值。
    /// </summary>
    public class InputBoxResult : IDisposable
    {
        #region Fields

        private List<Control> _Editors;
        private InputBoxForm _Form;
        private bool _IsDisposed;
        private bool _IsSubmit;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 初始化 System.Windows.Forms.InputBoxResult 的新实例。
        /// </summary>
        /// <param name="editors">可输入控件的集合。</param>
        /// <param name="form">输入框的窗体。</param>
        internal InputBoxResult(List<Control> editors, InputBoxForm form)
        {
            this._Editors = editors;
            this._Form = form;
        }

        /// <summary>
        /// 析构方法。
        /// </summary>
        ~InputBoxResult()
        {
            this.Dispose();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 获取一个值，表示输入框的可输入控件集合。
        /// </summary>
        public IEnumerable<Control> Editors
        {
            get
            {
                return this._Editors;
            }
        }

        /// <summary>
        /// 获取一个值，表示输入框的窗体。
        /// </summary>
        public InputBoxForm Form
        {
            get
            {
                return this._Form;
            }
        }

        /// <summary>
        /// 获取一个值，表示当前查询结果是否已释放资源。
        /// </summary>
        public bool IsDisposed
        {
            get { return this._IsDisposed; }
        }

        /// <summary>
        /// 获取一个值，表示用户是否确认并提交了所有内容。
        /// </summary>
        public bool IsSubmit
        {
            get
            {
                return this._IsSubmit;
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// 指定索引获取一个值，表示索引处可输入控件的 EditValue 值。
        /// </summary>
        /// <param name="index">索引。</param>
        /// <returns>返回索引处的可输入控件的 EditValue 值。</returns>
        public object this[int index]
        {
            get
            {
                return this.GetText(index);
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// 释放由 System.Windows.Forms.InputBoxResult 使用的所有资源。
        /// </summary>
        public void Dispose()
        {
            if(this._IsDisposed) return;
            this._IsDisposed = true;

            if(this._Form != null && !this._Form.IsDisposed)
            {
                try
                {
                    this._Form.Dispose();
                }
                catch(Exception)
                {

                }
            }

            this._Form = null;

            if(this._Editors != null) this._Editors.Clear();

            this._Editors = null;
        }

        /// <summary>
        /// 获取指定索引可输入控件。
        /// </summary>
        /// <param name="index">索引。</param>
        /// <returns>返回索引处的可输入控件。</returns>
        public Control GetEditor(int index)
        {
            return this._Editors[index];
        }

        /// <summary>
        /// 获取指定索引可输入控件的 Text 值。
        /// </summary>
        /// <param name="index">索引。</param>
        /// <returns>返回索引处的可输入控件的 Text 值。</returns>
        public string GetText(int index)
        {
            return this._Editors[index].Text;
        }

        /// <summary>
        /// 弹出窗体。
        /// </summary>
        /// <returns>返回用户是否选择了“确定”、“提交”或“是”等确认操作。</returns>
        public bool ShowDialog()
        {
            return this._IsSubmit = this._Form.ShowDialog() == DialogResult.OK;
        }

        #endregion Methods
    }
}