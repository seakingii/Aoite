namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class NoFocusedForm
#if DEVEXPRESS
 : DevExpress.XtraEditors.XtraForm
#else
 : Form
#endif
    {
        private const int MA_NOACTIVATE = 3;
        private const int WA_INACTIVE = 0;
        private const int WM_ACTIVATE = 0x006;
        private const int WM_ACTIVATEAPP = 0x01C;
        private const int WM_MOUSEACTIVATE = 0x21;
        private const int WM_NCACTIVATE = 0x086;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = new IntPtr(MA_NOACTIVATE);
                return;
            }
            else if (m.Msg == WM_NCACTIVATE)
            {
                if (((int)m.WParam & 0xFFFF) != WA_INACTIVE)
                {
                    if (m.LParam != IntPtr.Zero)
                    {
                        SetActiveWindow(m.LParam);
                    }
                    else
                    {
                        SetActiveWindow(IntPtr.Zero);
                    }
                }
            }
            base.WndProc(ref m);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr handle);
    }
}