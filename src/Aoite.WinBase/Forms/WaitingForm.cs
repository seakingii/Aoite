using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
#if DEVEXPRESS
using DevExpress.XtraEditors;
#endif

namespace System.Windows.Forms
{
#if DEVEXPRESS
    using Label2 = LabelControl;
    using Panel2 = PanelControl;
    using Text2 = TextEdit;
    using Check2 = CheckEdit;
    using Picture2 = PictureEdit;
    using Rich2 = MemoEdit;
    using Control2 = BaseControl;
#else
    using Label2 = Label;
    using Button2 = Button;
    using Panel2 = Panel;
    using Text2 = TextBox;
    using Check2 = CheckBox;
    using Picture2 = PictureBox;
    using Rich2 = RichTextBox;
    using Control2 = Control;
#endif
    /// <summary>
    /// 表示显示进度条等待的窗体。
    /// </summary>
    internal class WaitingForm : NoFocusedForm, IWaitingTask
    {
        internal static readonly System.Drawing.Image CloseImage;
        internal static readonly System.Drawing.Image WaitImage;

        readonly object SyncObject = new object();

        private System.ComponentModel.IContainer components = null;
        private Label2 lbl_Content;
        private Panel2 panelControl1;
        private System.Windows.Forms.PictureBox pictureBox1;

        /// <summary>
        /// 表示当前的工作是否完成。
        /// </summary>
        private bool _isCompleted = false;
        private bool _IsShown = false;
        private bool _IsTextChangedClearSeconds;

        /// <summary>
        /// 表示调用委托的工作线程。
        /// </summary>
        private Thread _OwnerThread;
        int _second = 0;
        System.Windows.Forms.Timer _timer;

        static WaitingForm()
        {
            var assembly = typeof(WaitingForm).Assembly;
            WaitImage = System.Drawing.Bitmap.FromStream(assembly.GetManifestResourceStream("System.Windows.Forms.WaitingPanel.gif"));
            CloseImage = System.Drawing.Bitmap.FromStream(assembly.GetManifestResourceStream("System.Windows.Forms.Close.png"));
        }

        public WaitingForm(bool shownCloseButton, Form callerForm)
        {
            this.CallerForm = callerForm;
            InitializeComponent(shownCloseButton);
        }

        /// <summary>
        /// 获取一个值，表示当前窗体是否已经呈现给用户。
        /// </summary>
        public bool IsShown { get { return this._IsShown; } }

        public bool IsTextChangedClearSeconds { get { return this._IsTextChangedClearSeconds; } set { this._IsTextChangedClearSeconds = value; } }

        public Thread OwnerThread { get { return this._OwnerThread; } set { _OwnerThread = value; } }

        /// <summary>
        /// 设置或获取窗体以及进度条的文本。
        /// </summary>
        public override string Text { get { return base.Text; } set { this.SetText(value); } }

        public Control This { get { return this; } }

        public Form CallerForm { get; private set; }



        public void ClearSeconds()
        {
            this._second = 0;
        }

        public void ShowForm(Form form)
        {
            this.ShowDialog(form);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<bool>(this.Dispose), disposing);
                }
                catch (Exception)
                {
                    //this.Dispose(disposing);
                }
            }
            else if (this.IsHandleCreated)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
                if (this._timer != null)
                {
                    this._timer.Stop();
                    this._timer.Dispose();
                }
                this._timer = null;
                this._isCompleted = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (!this.IsHandleCreated) return;

            base.OnClosed(e);
            if (!this._isCompleted && this._OwnerThread != null && this._OwnerThread.IsAlive)
            {
                this._OwnerThread.Abort();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!this.IsHandleCreated) return;

            base.OnClosing(e);
            if (!this._isCompleted)
            {
                e.Cancel = !this.ShowYesNo("强制中断可能会导致无法预料的结果，是否强制中断执行？");
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this._IsShown = true;
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 60000;
            _timer.Tick += new EventHandler(timer_Tick);
            _timer.Start();
        }

        void closeLabel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent(bool shownCloseButton)
        {
            this.lbl_Content = new Label2();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelControl1 = new Panel2();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
#if DEVEXPRESS
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
#endif
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            //
            // lbl_Content
            //
#if DEVEXPRESS
            this.lbl_Content.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lbl_Content.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lbl_Content.AutoSizeMode = LabelAutoSizeMode.None;
#else
            this.lbl_Content.TextAlign = ContentAlignment.MiddleCenter;
            this.lbl_Content.AutoSize = false;
#endif
            this.lbl_Content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_Content.Location = new System.Drawing.Point(3, 75);
            this.lbl_Content.Name = "lbl_Content";
            this.lbl_Content.TabIndex = 0;
            //
            // pictureBox1
            //
            this.pictureBox1.Image = WaitImage; //Properties.Resources.WaitingPanel;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Height = WaitImage.Height + 5;
            this.pictureBox1.Dock = DockStyle.Top;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            //
            // panelControl1
            //
            this.panelControl1.Controls.Add(this.lbl_Content);
            this.panelControl1.Controls.Add(this.pictureBox1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(335, 109);
            this.panelControl1.TabIndex = 1;
            //
            // WaitingForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 75);
            if (shownCloseButton)
            {
                Label closeLabel = new Label();
                closeLabel.AutoSize = false;
                var image = CloseImage;
                closeLabel.Size = image.Size;
                closeLabel.Location = new System.Drawing.Point(this.Width - 30, 1);
                closeLabel.Image = image;
                closeLabel.ImageAlign = ContentAlignment.MiddleCenter;
                closeLabel.Click += new EventHandler(closeLabel_Click);
                this.Controls.Add(closeLabel);
            }
            this.Controls.Add(this.panelControl1);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "WaitingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "正在准备中...";
            // this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
#if DEVEXPRESS
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
#endif
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void SetText(string value)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired) this.Invoke(new Action<string>(this.SetText), value);
            else
            {
                if (this._IsTextChangedClearSeconds)
                {
                    this.ClearSeconds();
                }
                lock (SyncObject)
                {
                    base.Text = value;
                    lbl_Content.Text = value;
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            this._second++;
            lock (SyncObject)
            {
                lbl_Content.Text = base.Text + " ( " + (this._second).ToString() + "分钟 )";
            }

        }

    }
}
