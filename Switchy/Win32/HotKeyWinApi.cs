using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Switchy.Win32
{
    internal static class HotKeyWinApi
    {
        internal const int WmHotKey = 0x0312;

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}