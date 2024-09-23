using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace C2Windows
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class EventForwarder
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        // dragable border regions
        public const int HTLEFT = 10;
        public const int HTRIGHT = 11;
        public const int HTTOP = 12;
        public const int HTTOPLEFT = 13;
        public const int HTTOPRIGHT = 14;
        public const int HTBOTTOM = 15;
        public const int HTBOTTOMLEFT = 16;
        public const int HTBOTTOMRIGHT = 17;


        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        readonly IntPtr target;

        public EventForwarder(IntPtr target)
        {
            this.target = target;
        }

        public void MouseDownDrag()
        {
            ReleaseCapture();
            SendMessage(target, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        /// <summary>
        /// Resize Drag
        /// 
        /// 8 directions.
        /// Top Left, Top , Top Right, Right, Bottom Right, Bottom, Bottom Left, Left
        /// 
        /// </summary>
        /// <param name="direction"></param>
        public void MouseDownResizeDrag(int direction)
        {
            ReleaseCapture();
            SendMessage(target, WM_NCLBUTTONDOWN, direction, 0);
        }

        public void SetViewportMargins(bool state = false)
        {
            if(state)
            {
                MainWindow.webViewRenderer.Margin = new System.Windows.Thickness(0, 0, 0, 0);
            }
            else
            {
                MainWindow.webViewRenderer.Margin = new System.Windows.Thickness(2.5, 2.5, 2.5, 2.5);            
            }
        }
    }
}
