using System;
using System.Runtime.InteropServices;

namespace Notifier.Infrastructures
{
    internal static class Ole32dll
    {
        [DllImport("ole32.dll")]
        public static extern void CoTaskMemFree(IntPtr pv);
    }
}