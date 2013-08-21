using System;
using System.Runtime.InteropServices;

namespace Yuhan.WPF.CustomWindow
{
    public static class WinApi
    {
        // Nearest monitor to window
        public const int MONITOR_DEFAULTTONEAREST = 2;

        // Get a handle to the specified monitor
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        // Get the working area of the specified monitor
        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MonitorInfoEx monitorInfo);

        // Get Vista system color
        [DllImport("dwmapi.dll")]
        public static extern void DwmGetColorizationColor(out uint ColorizationColor, out bool ColorizationOpaqueBlend); 

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MonitorInfoEx
        {
            public int cbSize;
            public Rect rcMonitor;                      // Total area
            public Rect rcWork;                         // Working area
            public int dwFlags;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public char[] szDevice;
        }
    }
}
