using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Yuhan.WPF.CustomWindow
{
    public static class Graphics
    {
        /// <summary>
        /// Start aero in the given window
        /// </summary>
        /// <param name="window">Window</param>
        /// <param name="captionHeight">Window caption height</param>
        /// <returns>Return true if aero enabled; otherwise return false</returns>
        public static bool InicializeAero(Window window, int captionHeight)
        {
            bool aeroEnabled = false;

            // XP is version 5, Vista is version 6
            if (Environment.OSVersion.Version.Major >= 6)
            {
                try
                {
                    VistaApi.DwmIsCompositionEnabled(ref aeroEnabled);

                    if (aeroEnabled)
                    {
                        IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
                        HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                        mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

                        // extend glass effect
                        VistaApi.Margins margins = new VistaApi.Margins();
                        margins.Set(0, 0, captionHeight, 0);

                        // inicialize aero
                        VistaApi.DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                    }
                }
                // glass effect is not supported
                catch (DllNotFoundException)
                {
                    aeroEnabled = false;
                }
            }

            return aeroEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static Point GetMonitorSize(Window window)
        {
            // Get handle for nearest monitor to this window
            WindowInteropHelper wih = new WindowInteropHelper(window);
            IntPtr hMonitor = WinApi.MonitorFromWindow(wih.Handle, WinApi.MONITOR_DEFAULTTONEAREST);

            // Get monitor info
            WinApi.MonitorInfoEx monitorInfo = new WinApi.MonitorInfoEx();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            WinApi.GetMonitorInfo(new HandleRef(window, hMonitor), monitorInfo);

            // Create working area dimensions, converted to DPI-independent values
            HwndSource source = HwndSource.FromHwnd(wih.Handle);

            if (source == null) return new Point(); // Should never be null

            if (source.CompositionTarget == null) return new Point(); // Should never be null

            Matrix matrix = source.CompositionTarget.TransformFromDevice;
            WinApi.Rect workingArea = monitorInfo.rcWork;

            Point dpiIndependentSize = matrix.Transform(new Point(workingArea.Right - workingArea.Left, workingArea.Bottom - workingArea.Top));

            return dpiIndependentSize;
        }
    }
}
