using System.Runtime.InteropServices;

namespace Yuhan.WPF.CustomWindow
{
    internal class VistaApi
    {
        [DllImport("dwmapi.dll")] 
        internal static extern void DwmIsCompositionEnabled(ref bool pfEnabled);

        [DllImport("dwmapi.dll")]
        internal static extern int
            DwmExtendFrameIntoClientArea(System.IntPtr hWnd, ref Margins pMargins);

        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            int left, right, top, bottom;

            public void Set(int left, int right, int top, int bottom)
            {
                this.left = left;
                this.right = right;
                this.top = top;
                this.bottom = bottom;
            }
        }
    }
}