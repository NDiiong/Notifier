using System;
using System.Runtime.InteropServices;

namespace Notifier.Infrastructures
{
    internal static class Shell32dll
    {
        [DllImport("Shell32.dll")]
        internal static extern IntPtr GetCurrentProcessExplicitAppUserModelID(out IntPtr AppID);

        internal static string GetCurrentProcessExplicitAppUserModelID()
        {
            GetCurrentProcessExplicitAppUserModelID(out var pv);

            if (pv == IntPtr.Zero)
                return null;

            var s = Marshal.PtrToStringAuto(pv);
            Ole32dll.CoTaskMemFree(pv);
            return s;
        }
    }
}