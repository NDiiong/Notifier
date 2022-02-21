using System;

namespace Notifier.Runtime
{
    public static class ArrayElement
    {
        public static void Push<T>(ref T[] table, T value)
        {
            Array.Resize(ref table, table.Length + 1);
            table.SetValue(value, table.Length - 1);
        }
    }
}