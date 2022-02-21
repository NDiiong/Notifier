using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Notifier.Infrastructures
{
    internal static class User32dll
    {
        [DllImport("user32.dll")]
        internal static extern int ShowWindow(IntPtr hWnd, uint Msg);

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref Windowplacement lpwndpl);

        internal struct Windowplacement
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }
    }
}