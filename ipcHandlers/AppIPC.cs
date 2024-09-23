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
    public partial class AppIPC
    {
        // foo void that takes in a string and returns a string

        private MainWindow mainWindow;
        public AppIPC(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            // on window resize event
            mainWindow.SizeChanged += (sender, e) =>
            {
                var ipcMsg = new IPCMessage();
                ipcMsg.channel = "update-window-maximized";
                if (mainWindow.WindowState == System.Windows.WindowState.Maximized)
                {
                    ipcMsg.data = "true";
                    CoreWV.WV.CoreWebView2.ExecuteScriptAsync(@"CiderApp.appState.window.maximized = true");
                }
                else
                {
                    ipcMsg.data = "false";
                    CoreWV.WV.CoreWebView2.ExecuteScriptAsync(@"CiderApp.appState.window.maximized = false");
                }
                MainWindow.webViewRenderer.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(ipcMsg));

            };

            // window in focus
            mainWindow.Activated += (sender, e) =>
            {
                var ipcMsg = new IPCMessage();
                ipcMsg.channel = "window-focused";
                ipcMsg.data = "true";
                MainWindow.webViewRenderer.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(ipcMsg));
            };
            // window out of focus
            mainWindow.Deactivated += (sender, e) =>
            {
                var ipcMsg = new IPCMessage();
                ipcMsg.channel = "window-blurred";
                ipcMsg.data = "false";
                MainWindow.webViewRenderer.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(ipcMsg));
            };
        }

        // Snap Assist, trigger Windows + Z effect.  This shows the snap assist window
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        public void snapAssist()
        {
            keybd_event(0x5B, 0, 0, UIntPtr.Zero);
            keybd_event(0x5A, 0, 0, UIntPtr.Zero);
            keybd_event(0x5A, 0, 2, UIntPtr.Zero);
            keybd_event(0x5B, 0, 2, UIntPtr.Zero);
        }


        public string foo(string bar)
        {
            return bar;
        }

        public string emulate(string channel, string data)
        {
            switch (channel)
            {
                case "maximize":
                    // maximize window
                    if (mainWindow.WindowState == System.Windows.WindowState.Maximized)
                    {
                        mainWindow.WindowState = System.Windows.WindowState.Normal;
                        return channel;
                    }
                    mainWindow.WindowState = System.Windows.WindowState.Maximized;
                    return channel;
                case "minimize":
                    // minimize window
                    mainWindow.WindowState = System.Windows.WindowState.Minimized;
                    return channel;
                case "close":
                    // close window
                    mainWindow.Close();
                    return channel;
                case "get-platform":
                    return "win32";
                default:
                    return channel;
            }

        }
    }
}