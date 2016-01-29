namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// 表示按下全局热键后发生的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void HotkeyDownEventHandler(object sender, HotkeyEventArgs e);

    /// <summary>
    /// 表示注册、注销全局热键的封装。
    /// </summary>
    public class Hotkey : IMessageFilter
    {
        IntPtr _hWnd;
        Dictionary<Keys, int> _keyList = new Dictionary<Keys, int>(11);

        /// <summary>
        /// 初始化 <see cref="System.Windows.Forms.Hotkey"/> 类的新实例。
        /// </summary>
        /// <param name="hWnd">窗口句柄。</param>
        public Hotkey(IntPtr hWnd)
        {
            this._hWnd = hWnd;
            Application.AddMessageFilter(this);
        }

        [Flags]
        private enum HotkeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8
        }

        /// <summary>
        /// 按下全局热键后发生。
        /// </summary>
        public event HotkeyDownEventHandler HotkeyDown;


        /// <summary>
        /// 在调度消息之前将其筛选出来
        /// </summary>
        /// <param name="m">要调度的消息。无法修改此消息。</param>
        [System.Diagnostics.DebuggerHidden]
        public bool PreFilterMessage(ref Message m)
        {
            if(m.Msg == 0x312) /*WM_HOTKEY*/
            {
                int keyID = (int)m.WParam;

                foreach(var item in this._keyList)
                {
                    if(item.Value == keyID)
                    {
                        this.OnHotkeyDown(new HotkeyEventArgs(keyID, item.Key));
                        break;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 注册一个全局热键。
        /// </summary>
        /// <param name="keys">键。</param>
        public int RegisterHotkey(Keys keys)
        {
            int hotkeyid = GlobalAddAtom(Guid.NewGuid().ToString());
            this._keyList.Add(keys, hotkeyid);

            HotkeyModifiers modifiers = HotkeyModifiers.None;
            if((keys & Keys.Control) == Keys.Control)
            {
                modifiers |= HotkeyModifiers.Control;
                keys = keys & ~Keys.Control;
            }
            if((keys & Keys.Alt) == Keys.Alt)
            {
                modifiers |= HotkeyModifiers.Alt;
                keys = keys & ~Keys.Alt;
            }
            if((keys & Keys.Shift) == Keys.Shift)
            {
                modifiers |= HotkeyModifiers.Shift;
                keys = keys & ~Keys.Shift;
            }
            if((keys & Keys.LWin) == Keys.LWin)
            {
                modifiers |= HotkeyModifiers.Windows;
                keys = keys & ~Keys.LWin;
            }

            RegisterHotKey(_hWnd, hotkeyid, (int)modifiers, (int)keys);
            return hotkeyid;
        }

        /// <summary>
        /// 注销一个全局热键。
        /// </summary>
        /// <param name="keys">键。</param>
        public void UnregisterHotkeys(Keys keys)
        {
            if(this._keyList.ContainsKey(keys))
            {
                int keyid = this._keyList[keys];
                UnregisterHotKey(_hWnd, keyid);
                GlobalDeleteAtom(keyid);
                this._keyList.Remove(keys);
            }
        }

        /// <summary>
        /// 注销所有的全局热键。
        /// </summary>
        public void UnregisterHotkeys()
        {
            Application.RemoveMessageFilter(this);
            foreach(int key in this._keyList.Keys)
            {
                UnregisterHotKey(_hWnd, key);
                GlobalDeleteAtom(key);
            }
            this._keyList.Clear();
        }

        /// <summary>
        /// 引发 <see cref="Hotkey"/> 的 <see cref="Hotkey.HotkeyDown"/> 事件。
        /// </summary>
        /// <param name="e">参数。</param>
        protected virtual void OnHotkeyDown(HotkeyEventArgs e)
        {
            if(this.HotkeyDown != null) this.HotkeyDown(this, e);
        }

        #region kernel32 & user32

        [DllImport("kernel32.dll")]
        static extern int GlobalAddAtom(String lpString);

        [DllImport("kernel32.dll")]
        static extern int GlobalDeleteAtom(int nAtom);

        [DllImport("user32.dll")]
        static extern int RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        static extern int UnregisterHotKey(IntPtr hWnd, int id);

        #endregion
    }

    /// <summary>
    /// 表示按下全局热键后发生的事件参数。
    /// </summary>
    public class HotkeyEventArgs : EventArgs
    {
        private Keys _keys;
        private int _keysID;

        /// <summary>
        /// 初始化 <see cref="System.Windows.Forms.HotkeyEventArgs"/> 类的新实例。
        /// </summary>
        /// <param name="keysID">唯一的全局热键编号。</param>
        /// <param name="keys">注册的全局热键。</param>
        public HotkeyEventArgs(int keysID, Keys keys)
        {
            this._keysID = keysID;
            this._keys = keys;
        }

        /// <summary>
        /// 注册的全局热键。
        /// </summary>
        public Keys Keys
        {
            get { return this._keys; }
        }

        /// <summary>
        /// 唯一的全局热键编号。
        /// </summary>
        public int KeysID
        {
            get { return this._keysID; }
        }
    }
}