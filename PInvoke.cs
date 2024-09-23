using System;
using System.Runtime.InteropServices;

namespace SystemBackdropTypes;

public class PInvoke
{
    public class ParameterTypes
    {
        /*
        [Flags]
        enum DWM_SYSTEMBACKDROP_TYPE
        {
            DWMSBT_MAINWINDOW = 2, // Mica
            DWMSBT_TRANSIENTWINDOW = 3, // Acrylic
            DWMSBT_TABBEDWINDOW = 4 // Tabbed
        }
        */

        [Flags]
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_SYSTEMBACKDROP_TYPE = 38
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;      // width of left border that retains its size
            public int cxRightWidth;     // width of right border that retains its size
            public int cyTopHeight;      // height of top border that retains its size
            public int cyBottomHeight;   // height of bottom border that retains its size
        };
    }

    public static class Methods
    {
        [DllImport("DwmApi.dll")]
        static extern int DwmExtendFrameIntoClientArea(
            IntPtr hwnd,
            ref ParameterTypes.MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        static extern int DwmSetWindowAttribute(IntPtr hwnd, ParameterTypes.DWMWINDOWATTRIBUTE dwAttribute, ref int pvAttribute, int cbAttribute);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        // snap assist

        public static int ExtendFrame(IntPtr hwnd, ParameterTypes.MARGINS margins)
            => DwmExtendFrameIntoClientArea(hwnd, ref margins);

        public static int SetWindowAttribute(IntPtr hwnd, ParameterTypes.DWMWINDOWATTRIBUTE attribute, int parameter)
            => DwmSetWindowAttribute(hwnd, attribute, ref parameter, Marshal.SizeOf<int>());

        public static int SetWindowLongAttribute(IntPtr hwnd, int nIndex, int dwNewLong)
            => SetWindowLong(hwnd, nIndex, dwNewLong);

        public static void Disable_SYSMENU(IntPtr hwnd)
        {
            const int GWL_STYLE = -16;
            const int WS_SYSMENU = 0x80000;
            int style = GetWindowLong(hwnd, GWL_STYLE);
            //SetWindowLongAttribute(hwnd, GWL_STYLE, style & ~WS_SYSMENU);

            // disable the minimize button and the maximize button
            const int GWL_STYLE2 = -20;
            const int WS_MINIMIZEBOX = 0x20000;
            const int WS_MAXIMIZEBOX = 0x10000;
            int style2 = GetWindowLong(hwnd, GWL_STYLE2);
            //SetWindowLongAttribute(hwnd, GWL_STYLE2, style2 & ~WS_MINIMIZEBOX & ~WS_MAXIMIZEBOX);

            // disable the title bar
            const int GWL_STYLE3 = -16;
            const int WS_CAPTION = 0xC00000;
            int style3 = GetWindowLong(hwnd, GWL_STYLE3);
           // SetWindowLongAttribute(hwnd, GWL_STYLE3, style3 & ~WS_CAPTION);

            // disable the border
            const int GWL_STYLE4 = -16;
            const int WS_BORDER = 0x800000;
            int style4 = GetWindowLong(hwnd, GWL_STYLE4);
          //  SetWindowLongAttribute(hwnd, GWL_STYLE4, style4 & ~WS_BORDER);

            // set resize border with to 16px
            const int GWL_STYLE5 = -16;
            const int WS_THICKFRAME = 0x40000;
            int style5 = GetWindowLong(hwnd, GWL_STYLE5);
            SetWindowLongAttribute(hwnd, GWL_STYLE5, style5 & ~WS_THICKFRAME);


        }
    }
}
