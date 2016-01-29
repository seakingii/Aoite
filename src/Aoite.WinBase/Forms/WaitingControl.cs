using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
#if DEVEXPRESS
using DevExpress.XtraEditors;
#endif

namespace System.Windows.Forms
{
#if DEVEXPRESS
    using Label2 = LabelControl;
    using Panel2 = PanelControl;
#else
    using Label2 = Label;
    using Panel2 = Panel;
#endif

    internal class WaitingControl : Panel2, IWaitingTask
    {
        readonly object SyncObject = new object();

        Label closeLabel;
        List<Control> disableControls = new List<Control>();
        Label2 lbl_Content;
        Form owneForm;
        PictureBox pictureBox_Loading;
        private bool _IsShown = false;
        private bool _IsTextChangedClearSeconds;
        private Thread _OwnerThread;
        int _second = 0;
        System.Windows.Forms.Timer _timer;

        public WaitingControl(Form callerForm)
        {
            this.CallerForm = callerForm;
            this.InitializeComponent();
        }

        /// <summary>
        /// 获取一个值，表示当前窗体是否已经呈现给用户。
        /// </summary>
        public bool IsShown
        {
            get
            {
                return this._IsShown;
            }
        }

        public bool IsTextChangedClearSeconds
        {
            get
            {
                return this._IsTextChangedClearSeconds;
            }
            set
            {
                this._IsTextChangedClearSeconds = value;
            }
        }

        public Thread OwnerThread
        {
            get
            {
                return this._OwnerThread;
            }
            set
            {
                _OwnerThread = value;
            }
        }

        /// <summary>
        /// 设置或获取窗体以及进度条的文本。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                this.SetText(value);
            }
        }

        public Control This
        {
            get { return this; }
        }

        public Form CallerForm { get; private set; }

        public void ClearSeconds()
        {
            this._second = 0;
        }

        public void ShowForm(Form form)
        {
            owneForm = form;
            foreach (Control item in form.Controls)
            {
                if (item.Visible)
                {
                    this.disableControls.Add(item);
                    item.Visible = false;
                }
            }
            lbl_Content.Location = new Point(pictureBox_Loading.Location.X - lbl_Content.Width, pictureBox_Loading.Location.Y + pictureBox_Loading.Height + 10);
            closeLabel.Location = new System.Drawing.Point(this.Width - closeLabel.Width - 5, 3);
            this.Dock = DockStyle.Fill;
            form.Controls.Add(this);

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(timer_Tick);
            _timer.Start();
            this._IsShown = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.Created)
            {
                owneForm.BeginInvoke(new Action(() =>
                {
                    owneForm.Controls.Remove(this);
                    foreach (Control item in this.disableControls)
                    {
                        item.Visible = true;
                    }
                    base.Dispose(disposing);
                }));
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            lbl_Content.Height = this.Height - (pictureBox_Loading.Location.Y + pictureBox_Loading.Size.Height + 10);
        }

        void closeLabel_Click(object sender, EventArgs e)
        {
            if (this.ShowYesNo("强制中断可能会导致无法预料的结果，是否强制中断执行？"))
            {
                this.Dispose();
            }
        }

        private void InitializeComponent()
        {
            base.CreateControl();
            lbl_Content = new Label2();
#if DEVEXPRESS
            lbl_Content.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lbl_Content.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            lbl_Content.AutoSizeMode = LabelAutoSizeMode.None;
#else
            lbl_Content.TextAlign = ContentAlignment.MiddleCenter;
            lbl_Content.AutoSize = false;
#endif
            lbl_Content.Dock = System.Windows.Forms.DockStyle.Bottom;
            lbl_Content.Name = "lbl_Content";
            lbl_Content.TabIndex = 0;
            lbl_Content.BackColor = Color.Transparent;

            pictureBox_Loading = new PictureBox();
            pictureBox_Loading.Name = "pictureBox_Loading";
            Image image = WaitingForm.WaitImage;
            pictureBox_Loading.Size = new System.Drawing.Size(image.Width + 5, image.Height + 5);
            pictureBox_Loading.Image = image;

            pictureBox_Loading.Anchor = AnchorStyles.None;
            pictureBox_Loading.Location = new Point((this.Width - pictureBox_Loading.Width) / 2, (this.Height - pictureBox_Loading.Height) / 2 - 20);

            image = WaitingForm.CloseImage;

            closeLabel = new Label();
            closeLabel.AutoSize = false;
            closeLabel.Size = image.Size;
            closeLabel.Image = image;
            closeLabel.ImageAlign = ContentAlignment.MiddleCenter;

            closeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            closeLabel.Click += new EventHandler(closeLabel_Click);

            this.Text = "正在进行中...";

            this.Controls.Add(lbl_Content);
            this.Controls.Add(pictureBox_Loading);
            this.Controls.Add(closeLabel);
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
            if (this._second > 4)
            {
                lock (SyncObject)
                {
                    lbl_Content.Text = base.Text + " ( " + (this._second).ToString() + "秒 )";
                }
            }
        }
    }
}