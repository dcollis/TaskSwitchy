using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Switchy.Win32
{
    internal static class WindowManagerApi
    {
        private const int SwRestore = 9;
        private const int GclHiconsm = -34;
        private const int GclHicon = -14;
        private const int IconSmall = 0;
        private const int IconBig = 1;
        private const int IconSmall2 = 2;
        private const int WmGeticon = 0x7F;

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        private static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private extern static bool DestroyIcon(IntPtr handle);

        internal static ImageSource GetAppIcon(IntPtr hwnd)
        {
            IntPtr iconHandle = SendMessage(hwnd, WmGeticon, IconBig, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GclHicon);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GclHiconsm);
            if (iconHandle == IntPtr.Zero)
                iconHandle = SendMessage(hwnd, WmGeticon, IconSmall, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = SendMessage(hwnd, WmGeticon, IconSmall2, 0);

            if (iconHandle == IntPtr.Zero)
                return null;

            using(Icon i = Icon.FromHandle(iconHandle))
            {
                 var image = Imaging.CreateBitmapSourceFromHIcon(
                            i.Handle,
                            new Int32Rect(0,0,i.Width, i.Height),
                            BitmapSizeOptions.FromEmptyOptions());
                DestroyIcon(iconHandle);
                return image;
            }
            
        }

        private static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
            {
                return GetClassLongPtr64(hWnd, nIndex);
            }
            return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }

        internal static BitmapSource CaptureWindowIfItIsntMinimised(Window window)
        {
            if (IsIconic(window.WindowPointer)) return null;
            var hWnd = window.WindowPointer;
            Rectangle rctForm;
            using (Graphics grfx = Graphics.FromHdc(GetWindowDC(hWnd)))
            {
                rctForm = Rectangle.Round(grfx.VisibleClipBounds);
            }
            var pImage = new Bitmap(rctForm.Width, rctForm.Height);
            Graphics graphics = Graphics.FromImage(pImage);
            IntPtr hDc = graphics.GetHdc();      
            try
            {
                PrintWindow(hWnd, hDc, 0);
            }
            finally
            {
                graphics.ReleaseHdc(hDc);
            }
            return GetBitmapSource(pImage);
        }

        internal static BitmapSource GetBitmapSource(Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs;
            try
            {
                bs = Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }


        internal static List<Window> GetWindowDetails(IntPtr owningWindow)
        {
            var shellWindow = GetShellWindow();
            var windows = new List<Window>();
            EnumWindows((hWnd, lParam) => 
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;
                if (hWnd == owningWindow) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                var builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                var name = builder.ToString();
                if(name != "Start") windows.Add(new Window(name,hWnd));
                return true;
            }, 0);

            PopulateIcons(windows);
            return windows;
        }

        //Maybe add process info to search
        internal static List<Window> GetProcessWindowDetails()
        {
            return Process.GetProcesses().Where(w => !w.MainWindowHandle.Equals(IntPtr.Zero))
                .Select(w => new Window(w.MainWindowTitle, w.MainWindowHandle)).ToList();
        }

        internal static void ShowWindow(Window window)
        {
            ShowWindow(window.WindowPointer);
        }

        internal static void ShowWindow(IntPtr windowHandle)
        {
            if (IsIconic(windowHandle))
            {
                ShowWindow(windowHandle, SwRestore);
            }
            SetForegroundWindow(windowHandle);
        }

        internal static void PopulateIcons(List<Window> windows)
        {
            foreach (var window in windows)
            {
                window.Icon = GetAppIcon(window.WindowPointer);
            }
        }
    }
}
