namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

#if DEVEXPRESS
    using Aoite.WinForm.LangPack;
    using DevExpress.XtraEditors;
#endif
    /// <summary>
    /// 窗体控件的扩展方法集合。
    /// </summary>
    public static class EditorsExts
    {

        /// <summary>
        /// 获取应用程序的图标。
        /// </summary>
        public static readonly Icon AppIcon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);

        /// <summary>
        /// 获取或设置一个值，表示进度条窗口的显示内容。
        /// </summary>
        public static string DefaultWaitingText = "正在执行任务...";

        private static Mutex _Mutex;

        /// <summary>
        /// 同步基元。
        /// </summary>
        public static Mutex Mutex
        {
            get { return EditorsExts._Mutex; }
        }

        #region Methods

        /// <summary>
        /// 检查控件文本是否为空，增加控件的错误信息提示。
        /// </summary>
        /// <param name="edit">指定的控件。</param>
        /// <returns>如果控件文本为空返回 false，否则返回 true。</returns>
        public static bool DoCheck(this Control edit)
        {
            if (edit == null) return false;
            return DoCheck(edit, "不能为空！");
        }

        /// <summary>
        /// 检查控件文本是否为空，增加控件的错误信息提示。
        /// </summary>
        /// <param name="edit">指定的控件。</param>
        /// <param name="errorText">提示的内容。</param>
        /// <returns>如果控件文本为空返回 false，否则返回 true。</returns>
        public static bool DoCheck(this Control edit, string errorText)
        {
            if (edit == null) return false;
            return DoCheck(edit, string.IsNullOrEmpty(edit.Text = edit.Text.Trim()), errorText);
        }

        /// <summary>
        /// 根据指定条件，增加控件的错误信息提示。
        /// </summary>
        /// <param name="edit">指定的控件。</param>
        /// <param name="isError">指示是否错误。</param>
        /// <param name="errorText">提示的内容。</param>
        /// <returns>检查不通过返回 false，否则返回 true。</returns>
        public static bool DoCheck(this Control edit, bool isError, string errorText)
        {
            if (edit == null) return false;
            if (isError)
            {
                edit.ShowError(errorText);
                edit.Focus();
            }
            return !isError;
        }

        /// <summary>
        /// 线程安全调用指定的委托。
        /// </summary>
        /// <param name="control">线程的控件。</param>
        /// <param name="action">线程的委托。</param>
        public static void Saletime(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else action();
        }

        /// <summary>
        /// 从后台执行一个委托。
        /// </summary>
        /// <param name="owner">控件。</param>
        /// <param name="method">执行的委托。</param>
        /// <param name="args">参数集合。</param>
        public static void Backwork(this Control owner, Delegate method, params object[] args)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                var old = Control.CheckForIllegalCrossThreadCalls;
                Control.CheckForIllegalCrossThreadCalls = false;
                method.DynamicInvoke(args);
                Control.CheckForIllegalCrossThreadCalls = old;
            });
        }

        /// <summary>
        /// 从后台执行一个委托。
        /// </summary>
        /// <param name="owner">控件。</param>
        /// <param name="action">执行的委托。</param>
        public static void Backwork(this Control owner, Action action)
        {
            Backwork(owner, action, null);
        }

        /// <summary>
        /// 从后台执行一个任务。
        /// </summary>
        /// <typeparam name="TArg">委托参数的类型。</typeparam>
        /// <param name="owner">控件。</param>
        /// <param name="action">执行的委托。</param>
        /// <param name="arg">传送的参数值。</param>
        public static void Backwork<TArg>(this Control owner, Action<TArg> action
            , TArg arg)
        {
            Backwork(owner, action, new object[1] { arg });
        }

        /// <summary>
        /// 从后台执行一个任务。
        /// </summary>
        /// <typeparam name="TArg1">第一个参数类型。</typeparam>
        /// <typeparam name="TArg2">第二个参数类型。</typeparam>
        /// <param name="owner">控件。</param>
        /// <param name="action">执行的委托。</param>
        /// <param name="arg1">第一个参数值。</param>
        /// <param name="arg2">第二个参数值。</param>
        public static void Backwork<TArg1, TArg2>(this Control owner, Action<TArg1, TArg2> action
            , TArg1 arg1, TArg2 arg2)
        {
            Backwork(owner, action, new object[2] { arg1, arg2 });
        }

        /// <summary>
        /// 从后台执行一个任务。
        /// </summary>
        /// <typeparam name="TArg1">第一个参数类型。</typeparam>
        /// <typeparam name="TArg2">第二个参数类型。</typeparam>
        /// <typeparam name="TArg3">第三个参数类型。</typeparam>
        /// <param name="owner">控件。</param>
        /// <param name="action">执行的委托。</param>
        /// <param name="arg1">第一个参数值。</param>
        /// <param name="arg2">第二个参数值。</param>
        /// <param name="arg3">第三个参数值。</param>
        public static void Backwork<TArg1, TArg2, TArg3>(this Control owner, Action<TArg1, TArg2, TArg3> action
            , TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            Backwork(owner, action, new object[3] { arg1, arg2, arg3 });
        }

        /// <summary>
        /// 从后台执行一个任务。
        /// </summary>
        /// <typeparam name="TArg1">第一个参数类型。</typeparam>
        /// <typeparam name="TArg2">第二个参数类型。</typeparam>
        /// <typeparam name="TArg3">第三个参数类型。</typeparam>
        /// <typeparam name="TArg4">第四个参数类型。</typeparam>
        /// <param name="owner">控件。</param>
        /// <param name="action">执行的委托。</param>
        /// <param name="arg1">第一个参数值。</param>
        /// <param name="arg2">第二个参数值。</param>
        /// <param name="arg3">第三个参数值。</param>
        /// <param name="arg4">第四个参数值。</param>
        public static void Backwork<TArg1, TArg2, TArg3, TArg4>(this Control owner, Action<TArg1, TArg2, TArg3, TArg4> action
            , TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            Backwork(owner, action, new object[4] { arg1, arg2, arg3, arg4 });
        }

        /// <summary>
        /// 在新线程启动等待窗体。并在 <see cref="System.IDisposable"/> 后关闭等待窗体。
        /// </summary>
        /// <param name="owner">控件。</param>
        /// <param name="centerScreen">表示是否居中于桌面。</param>
        public static Form BeginRun(this Control owner, bool centerScreen = false)
        {
            return EditorsExts.BeginRun(owner, EditorsExts.DefaultWaitingText, centerScreen);
        }

        /// <summary>
        /// 在新线程启动等待窗体。并在 <see cref="System.IDisposable"/> 后关闭等待窗体。
        /// </summary>
        /// <param name="owner">控件。</param>
        /// <param name="text">等待窗体的内容。</param>
        /// <param name="centerScreen">表示是否居中于桌面。</param>
        public static Form BeginRun(this Control owner, string text, bool centerScreen = false)
        {
            WaitingForm fw = null;
            Thread t = null;

            t = new Thread(() =>
            {
                var ownerForm = owner.FindForm();
                fw = new WaitingForm(false, ownerForm);

                fw.OwnerThread = t;
                if (centerScreen) fw.StartPosition = FormStartPosition.CenterScreen;
                else
                {
                    int x = owner.Location.X + (owner.Width - fw.Width) / 2;
                    int y = owner.Location.Y + (owner.Height - fw.Height) / 2 - SystemInformation.CaptionHeight;
                    fw.Location = new Point(x, y);
                    fw.StartPosition = FormStartPosition.Manual;
                }
                fw.Text = text;
                EventHandler act = (ss, ee) =>
                {
                    if (fw.IsHandleCreated)
                        fw.BeginInvoke(new Action(fw.Activate));
                };
                if (ownerForm != null)
                {
                    ownerForm.Activated += act;

                }
                try
                {
                    fw.ShowDialog();
                }
                catch (ThreadAbortException) { }
                catch (Exception) { }

                if (ownerForm != null) ownerForm.Activated -= act;
            });

            t.Start();
            while (fw == null || !fw.IsShown) Application.DoEvents();

            return fw;
        }

        /// <summary>
        /// 判断当前应用程序是否唯一实例。倘若不是，显示已存在的实例。
        /// </summary>
        /// <param name="name">同步基元的名称。</param>
        public static bool CreatedNew(string name)
        {
#if DEBUG
            return true;
#endif
            bool createdNew = false;
            EditorsExts._Mutex = new Mutex(false, name, out createdNew);
            return createdNew;

        }

        const int SW_HIDE = 0;
        const int SW_SHOWNORMAL = 1;
        const int SW_SHOWMINIMIZED = 2;
        const int SW_SHOWMAXIMIZED = 3;
        const int SW_SHOWNOACTIVATE = 4;
        const int SW_SHOWACTIVATE = 5;
        const int SW_SHOWRESTORE = 9;
        const int SW_SHOWDEFAULT = 10;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// 显示一个不抢占当前窗体焦点的窗体。
        /// </summary>
        /// <param name="form">显示的窗体。</param>
        public static void ShowWithNoActived(this Form form)
        {
            ShowWindow(form.Handle, SW_SHOWNOACTIVATE);
        }

        /// <summary>
        /// 启动多线程等待。action 委托是在多线程中调用。并执行后续操作。
        /// </summary>
        /// <param name="owner">控件。</param>
        /// <param name="action">在多线程中运行的委托。</param>
        public static void RunTask(this Control owner, Action<Form> action)
        {
            EditorsExts.RunTask(owner, EditorsExts.DefaultWaitingText, action);
        }

        /// <summary>
        /// 启动多线程等待。action 委托是在多线程中调用。并执行后续操作。
        /// </summary>
        /// <param name="owner">控件。</param>
        /// <param name="text">等待窗体的内容。</param>
        /// <param name="action">在多线程中运行的委托。</param>
        public static void RunTask(this Control owner, string text, Action<Form> action)
        {
            RunTask(owner, text, (fw) => { action(fw); return true; });
        }

        /// <summary>
        /// 启动多线程等待。action 委托是在多线程中调用。并执行后续操作。
        /// </summary>
        /// <typeparam name="TResult">返回值数据类型。</typeparam>
        /// <param name="owner">控件。</param>
        /// <param name="func">在多线程中运行的委托，这个委托带有一个返回值。</param>
        public static TResult RunTask<TResult>(this Control owner, Func<Form, TResult> func)
        {
            return EditorsExts.RunTask(owner, EditorsExts.DefaultWaitingText, func);
        }

        /// <summary>
        /// 启动多线程等待。action 委托是在多线程中调用。并执行后续操作。
        /// </summary>
        /// <typeparam name="TResult">返回值数据类型。</typeparam>
        /// <param name="owner">控件。</param>
        /// <param name="text">等待窗体的内容。</param>
        /// <param name="func">在多线程中运行的委托，这个委托带有一个返回值。</param>
        public static TResult RunTask<TResult>(this Control owner, string text, Func<Form, TResult> func)
        {
            var waitTask = owner as IWaitingTask;
            return Run(owner, text, func, new WaitingControl(waitTask == null ? owner.FindForm() : waitTask.CallerForm));
        }

        /// <summary>
        /// 启动多线程等待。action 委托是在多线程中调用。并执行后续操作（Wait是可以强制中断的任务）。
        /// </summary>
        /// <param name="owner">控件。</param>
        /// <param name="action">在多线程中运行的委托。</param>
        public static void RunWait(this Control owner, Action<Form> action)
        {
            EditorsExts.RunWait(owner, EditorsExts.DefaultWaitingText, action);
        }

        /// <summary>
        /// 启动多线程等待。action 委托是在多线程中调用。并执行后续操作（Wait是可以强制中断的任务）。
        /// </summary>
        /// <param name="owner">控件。</param>
        /// <param name="text">等待窗体的内容。</param>
        /// <param name="action">在多线程中运行的委托。</param>
        public static void RunWait(this Control owner, string text, Action<Form> action)
        {
            RunWait(owner, text, (fw) => { action(fw); return true; });
        }

        /// <summary>
        /// 启动多线程等待。action 委托是在多线程中调用。并执行后续操作（Wait是可以强制中断的任务）。
        /// </summary>
        /// <typeparam name="TResult">返回值数据类型。</typeparam>
        /// <param name="owner">控件。</param>
        /// <param name="func">在多线程中运行的委托，这个委托带有一个返回值。</param>
        public static TResult RunWait<TResult>(this Control owner, Func<Form, TResult> func)
        {
            return EditorsExts.RunWait(owner, EditorsExts.DefaultWaitingText, func);
        }

        /// <summary>
        /// 启动多线程等待。action 委托是在多线程中调用。并执行后续操作（Wait是可以强制中断的任务）。
        /// </summary>
        /// <typeparam name="TResult">返回值数据类型。</typeparam>
        /// <param name="owner">控件。</param>
        /// <param name="text">等待窗体的内容。</param>
        /// <param name="func">在多线程中运行的委托，这个委托带有一个返回值。</param>
        public static TResult RunWait<TResult>(this Control owner, string text, Func<Form, TResult> func)
        {
            return Run(owner, text, func, new WaitingForm(true, owner.FindForm()));
        }

        private static class IconImages
        {
            static IconImages()
            {
                Information = SystemIcons.Information.ToBitmap();
                Question = SystemIcons.Question.ToBitmap();
                Warning = SystemIcons.Warning.ToBitmap();
                Error = SystemIcons.Error.ToBitmap();
            }
            public readonly static Bitmap Information;
            public readonly static Bitmap Question;
            public readonly static Bitmap Warning;
            public readonly static Bitmap Error;
        }

        /// <summary>
        /// 将指定 <see cref="System.Windows.Forms.MessageBoxIconEx"/> 转换为图标。
        /// </summary>
        /// <param name="value"><see cref="System.Windows.Forms.MessageBoxIconEx"/> 值。</param>
        /// <returns>返回一个图标。</returns>
        public static Bitmap ToBitmap(this MessageBoxIconEx value)
        {
            switch (value)
            {
                case MessageBoxIconEx.Information:
                    return IconImages.Information;

                case MessageBoxIconEx.Question:
                    return IconImages.Question;

                case MessageBoxIconEx.Warning:
                    return IconImages.Warning;
                case MessageBoxIconEx.Error:
                    return IconImages.Error;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 启动多线程等待。action 委托是在多线程中调用。并执行后续操作。
        /// </summary>
        /// <typeparam name="TResult">返回值数据类型。</typeparam>
        /// <param name="owner">控件。</param>
        /// <param name="text">等待窗体的内容。</param>
        /// <param name="func">在多线程中运行的委托，这个委托带有一个返回值。</param>
        /// <param name="taksControl">等待窗体。</param>
        private static TResult Run<TResult>(this Control owner, string text, Func<Form, TResult> func, IWaitingTask taksControl)
        {
            TResult result = default(TResult);
            Thread t = new Thread((p) =>
            {
                var old = Control.CheckForIllegalCrossThreadCalls;
                Control.CheckForIllegalCrossThreadCalls = false;
                var waitingForm = p as IWaitingTask;
                try
                {
                    while (!waitingForm.IsShown) Application.DoEvents();
                    result = func(p as Form);

                    waitingForm.Dispose();
                }
                catch (ThreadAbortException) { }
                Control.CheckForIllegalCrossThreadCalls = old;

            });
            t.IsBackground = true;
            taksControl.OwnerThread = t;
            taksControl.Text = text;
            var ownerForm = owner.FindForm();
            ownerForm.Activate();
            ownerForm.Focus();
            t.Start(taksControl);
            try
            {
                taksControl.ShowForm(ownerForm);
            }
            catch (InvalidOperationException)
            {
                if (!taksControl.IsDisposed) taksControl.Dispose();
            }
            catch (Exception ex)
            {
                ownerForm.ShowError(ex.Message);
            }
            finally
            {
                ownerForm.Activate();
            }
            return result;
        }

        #endregion Methods

        #region CueText

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg,
            int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hwnd, int msg, int wParam, StringBuilder lParam);
        [DllImport("user32.dll")]
        private static extern bool GetComboBoxInfo(IntPtr hwnd, ref COMBOBOXINFO pcbi);

        [StructLayout(LayoutKind.Sequential)]
        private struct COMBOBOXINFO
        {
            public int cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public IntPtr stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndItem;
            public IntPtr hwndList;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private const int EM_SETCUEBANNER = 0x1501;
        private const int EM_GETCUEBANNER = 0x1502;

        /// <summary>
        /// 设置控件的暗示信息。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <param name="text">暗示信息。</param>
        public static void SetCueText(this Control owner, string text)
        {
            if (owner is ComboBox)
            {
                COMBOBOXINFO info = GetComboBoxInfo(owner);
                SendMessage(info.hwndItem, EM_SETCUEBANNER, 0, text);
            }
            else
            {
                SendMessage(owner.Handle, EM_SETCUEBANNER, 0, text);
            }
        }

        private static COMBOBOXINFO GetComboBoxInfo(Control owner)
        {
            COMBOBOXINFO info = new COMBOBOXINFO();
            //a combobox is made up of three controls, a button, a list and textbox; 
            //we want the textbox 
            info.cbSize = Marshal.SizeOf(info);
            GetComboBoxInfo(owner.Handle, ref info);
            return info;
        }

        /// <summary>
        /// 获取控件的暗示信息。
        /// </summary>
        /// <param name="owner">控件拥有者。</param>
        /// <returns>暗示信息。</returns>
        public static string GetCueText(this Control owner)
        {
            StringBuilder builder = new StringBuilder();
            if (owner is ComboBox)
            {
                COMBOBOXINFO info = new COMBOBOXINFO();
                //a combobox is made up of two controls, a list and textbox; 
                //we want the textbox 
                info.cbSize = Marshal.SizeOf(info);
                GetComboBoxInfo(owner.Handle, ref info);
                SendMessage(info.hwndItem, EM_GETCUEBANNER, 0, builder);
            }
            else
            {
                SendMessage(owner.Handle, EM_GETCUEBANNER, 0, builder);
            }
            return builder.ToString();
        }


        #endregion
    }
}