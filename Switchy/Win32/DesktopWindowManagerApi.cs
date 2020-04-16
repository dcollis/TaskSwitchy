using System;
using System.Runtime.InteropServices;

namespace Switchy.Win32
{
    internal static class DesktopWindowManagerApi
    {
        private const int DwmTnpVisible = 0x8;
        private const int DwmTnpOpacity = 0x4;
        private const int DwmTnpRectdestination = 0x1;

        [StructLayout(LayoutKind.Sequential)]
        private struct Psize
        {
            private readonly int x;
            private readonly int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DwmThumbnailProperties
        {
            internal int dwFlags;
            internal Rect rcDestination;
            private readonly Rect rcSource;
            internal byte opacity;
            internal bool fVisible;
            private readonly bool fSourceClientAreaOnly;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rect
        {
            internal Rect(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }


        [DllImport("dwmapi.dll")]
        private static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport("dwmapi.dll")]
        private static extern int DwmUnregisterThumbnail(IntPtr thumb);

        [DllImport("dwmapi.dll")]
        private static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out Psize size);

        [DllImport("dwmapi.dll")]
        private static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DwmThumbnailProperties props);


        internal static IntPtr ShowThumb(IntPtr targetWindow, Window thumbWindow, Rect rect)
        {
            IntPtr thumb;
            int i = DwmRegisterThumbnail(targetWindow, thumbWindow.WindowPointer, out thumb);

            if (i == 0)
            {

                if (thumb != IntPtr.Zero)
                {
                    Psize size;
                    DwmQueryThumbnailSourceSize(thumb, out size);
                    var props = new DwmThumbnailProperties
                    {
                        fVisible = true,
                        dwFlags = DwmTnpVisible | DwmTnpRectdestination | DwmTnpOpacity,
                        opacity = 255,
                        rcDestination = rect
                    };
                    DwmUpdateThumbnailProperties(thumb, ref props);
                }
            }

            return thumb;
        }

        internal static void HideThumb(IntPtr thumb)
        {
            if (thumb != IntPtr.Zero)
                DwmUnregisterThumbnail(thumb);
        }

    }
}
