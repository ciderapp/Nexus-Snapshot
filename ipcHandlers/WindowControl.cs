using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using Wpf.Ui.Interop.WinDef;

namespace C2Windows
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public partial class WindowControl
    {
        private MainWindow mainWindow;
        public WindowControl(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void setSize(int width, int height)
        {
            mainWindow.Width = width;
            mainWindow.Height = height;
        }

        public void setPosition(int x, int y)
        {
            mainWindow.Left = x;
            mainWindow.Top = y;
        }

        public void setCenter()
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            mainWindow.Left = (desktopWorkingArea.Right - mainWindow.Width) / 2;
            mainWindow.Top = (desktopWorkingArea.Bottom - mainWindow.Height) / 2;
        }

        public void setResizable(bool resizable)
        {
            mainWindow.ResizeMode = resizable ? System.Windows.ResizeMode.CanResize : System.Windows.ResizeMode.NoResize;
        }

        public void setFullscreen(bool fullScreen)
        {
            mainWindow.FullScreen = fullScreen;
        }

        public void toggleFullscreen()
        {
            mainWindow.FullScreen = !mainWindow.FullScreen;
        }

        public void setZoomScale(double zoomScale)
        {
            CoreWV.WV.ZoomFactor = zoomScale;
        }

        public bool isFullscreen()
        {
            return mainWindow.FullScreen;
        }

        public bool isMaximized()
        {
            return mainWindow.WindowState == System.Windows.WindowState.Maximized;
        }

        public void isMinimized()
        {
            mainWindow.WindowState = System.Windows.WindowState.Minimized;
        }

        public string foo(string bar)
        {
            return bar;
        }

    }
}