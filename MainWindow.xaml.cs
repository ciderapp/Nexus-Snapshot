using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using static SystemBackdropTypes.PInvoke.ParameterTypes;
using static SystemBackdropTypes.PInvoke.Methods;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows.Shell;
using System.ComponentModel;

using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Management;
using System.Windows.Threading;
using Wpf.Ui.Controls;

namespace C2Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow, IAudioSessionEventsHandler
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        public static Microsoft.Web.WebView2.Wpf.WebView2? webViewRenderer;

        private bool _ExtendViewIntoTitleBar = true;

        public bool ExtendViewIntoTitleBar
        {
            get { return _ExtendViewIntoTitleBar; }
            set
            {
                _ExtendViewIntoTitleBar = value;
            }
        }

        private bool _UseModernWindowStyle = true;

        public bool UseModernWindowStyle
        {
            get { return _UseModernWindowStyle; }
            set
            {
                _UseModernWindowStyle = value;
            }
        }
        private void RefreshFrame()
        {

            IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
            HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            MARGINS margins = new MARGINS();
            int margin = 16;
            margins.cxLeftWidth = margin;
            margins.cxRightWidth = margin;
            margins.cyTopHeight = margin;
            margins.cyBottomHeight = margin;
            // WindowExtensions.HideAll(this);
            // IconHelper.RemoveIcon(this);
            //ExtendFrame(mainWindowSrc.Handle, margins);
            // Disable_SYSMENU(mainWindowSrc.Handle);
        }

        private void RefreshDarkMode()
        {
            /** var isDark = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;
             int flag = isDark ? 1 : 0;
             SetWindowAttribute(
                 new WindowInteropHelper(this).Handle,
                 DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                 flag); **/
        }
        private MMDevice? mainDevice;
        private AudioSessionControl? currentSession;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            try
            {
                var sess = this.currentSession;
                this.currentSession = null;
                this.UnregisterFromSession(sess);
                if (mainDevice != null)
                {
                    mainDevice.AudioSessionManager.OnSessionCreated -= this.AudioSessionManager_OnSessionCreated;
                    mainDevice.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        // protected override void OnSourceInitialized(EventArgs e)
        // {
        //     IconHelper.RemoveIcon(this);
        // }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            int flag = int.Parse((string)((RadioButton)sender).Tag);
            SetWindowAttribute(
                new WindowInteropHelper(this).Handle,
                DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
                flag);
        }

        void WebView_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs arg)
        {
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {

            // add AppGlobals.webView to WV.Children (Grid)
            WV.Children.Add(CoreWV.WV);
            webViewRenderer = CoreWV.WV;


            // int style = GetWindowLong(new WindowInteropHelper(this).Handle, -16);
            // SetWindowLong(new WindowInteropHelper(this).Handle, -16, (style & ~0x00C00000) | 0x00040000);
            // SetWindowPos(new WindowInteropHelper(this).Handle, IntPtr.Zero, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0040);
            RefreshFrame();
            RefreshDarkMode();
            //ThemeManager.Current.ActualApplicationThemeChanged += (_, _) => RefreshDarkMode();
            if (AppGlobals.IsWindows11())
            {
                SetWindowAttribute(new WindowInteropHelper(this).Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, 2);
            }
            else
            {
                SetWindowAttribute(new WindowInteropHelper(this).Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, 3);
            }


            // Audio Sesssion
            try
            {
                //Connect to audio device and get all audio session and try to change the text and icon.
                var etor = new MMDeviceEnumerator();
                this.mainDevice = etor.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                if (mainDevice is null)
                {
                    return;
                }

                var sessions = mainDevice.AudioSessionManager.Sessions;
                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    ChangeTextAndIcon(session);
                }

                //Listen for the creation of a new audio session, because likely there's none yet 
                //for our app. As soon as we are playing sound the event is fired and we can then change the text and icon.
                mainDevice.AudioSessionManager.OnSessionCreated += AudioSessionManager_OnSessionCreated;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                System.Windows.MessageBox.Show(this, ex.ToString(), this.Title);
            }
        }

        private bool fullScreen = false;
        [DefaultValue(false)]
        public bool FullScreen
        {
            get { return fullScreen; }
            set
            {
                fullScreen = value;
                if (value)
                {
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;
                    ResizeMode = ResizeMode.NoResize;
                }
                else
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Normal;
                    ResizeMode = ResizeMode.CanResize;
                }
            }
        }

        private void TBPlayPause_Click(object sender, EventArgs e)
        {
            CoreWV.WV.CoreWebView2.ExecuteScriptAsync(@"CiderApp.RPC.togglePlayPause()");
        }

        private void TBPrev_Click(object sender, EventArgs e)
        {
            CoreWV.WV.CoreWebView2.ExecuteScriptAsync(@"CiderApp.RPC.previous()");
        }

        private void TBNext_Click(object sender, EventArgs e)
        {
            CoreWV.WV.CoreWebView2.ExecuteScriptAsync(@"CiderApp.RPC.next()");
        }

        private void TBStar_Click(object sender, EventArgs e)
        {
            CoreWV.WV.CoreWebView2.ExecuteScriptAsync(@"CiderApp.RPC.toggleLoved()");

        }

        private void TBShuffle_Click(object sender, EventArgs e)
        {
            CoreWV.WV.CoreWebView2.ExecuteScriptAsync(@"CiderApp.RPC.toggleShuffle()");

        }

        private void TBAdd_Click(object sender, EventArgs e)
        {
            CoreWV.WV.CoreWebView2.ExecuteScriptAsync(@"CiderApp.RPC.toggleAdd()");

        }

        private void TBRepeat_Click(object sender, EventArgs e)
        {
            CoreWV.WV.CoreWebView2.ExecuteScriptAsync(@"CiderApp.RPC.toggleRepeat()");

        }


        /// <summary>
		/// Changes the text and icon of our application in the volume mixer (sndvol.exe).
		/// Note that this method can be called from a different thread than the UI thread.
		/// </summary>
		/// <param name="session">Any audio session control</param>
		private void ChangeTextAndIcon(AudioSessionControl session)
        {
            try
            {
                //If it's the sys sound session we can discard it right away
                if (session.IsSystemSoundsSession) return;
                var proc = Process.GetProcessById((int)session.GetProcessID);
                if (proc == null)
                {
                    return;
                }

                //Get the process that creates the audio session. 
                //WebView2 starts as a subprocess of our process, therefore
                //we ensure that the audio session really belongs to our app.
                var currentProc = Process.GetCurrentProcess();
                Process? parentProc = proc;
                bool isSubProc = false;
                while (parentProc != null && parentProc.Id != 0)
                {
                    parentProc = GetParentProcess(parentProc);
                    if (parentProc != null && parentProc.Id == currentProc.Id)
                    {
                        isSubProc = true;
                        break;
                    }
                }
                if (!isSubProc)
                {
                    return;
                }

                //Change text
                session.DisplayName = "Cider Audio Playback";
                //Change location. This change is not reflected in an opened volume mixer. 
                //It's only visible after the volume mixer has been (re-)opened
                session.IconPath = GetType().Assembly.Location;
                this.currentSession = session;
                //Subscribe to events of the session in order to eventually 
                this.currentSession.RegisterEventClient(this);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                //Since we could have entered from an audio event we may be on a diferent thread.
                _ = this.Dispatcher.InvokeAsync(() =>
                {
                    System.Windows.MessageBox.Show(this, ex.ToString(), this.Title);
                });
            }
        }

        private void AudioSessionManager_OnSessionCreated(object sender, IAudioSessionControl newSession)
        {
            AudioSessionControl managedControl = new AudioSessionControl(newSession);
            ChangeTextAndIcon(managedControl);
        }

        /// <summary>
        /// Gets the parent proccess for a given <paramref name="process"/>.
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private Process? GetParentProcess(Process process)
        {
            try
            {
                using (var query = new ManagementObjectSearcher(
                  "SELECT * " +
                  "FROM Win32_Process " +
                  "WHERE ProcessId=" + process.Id))
                {
                    using (var collection = query.Get())
                    {
                        var mo = collection.OfType<ManagementObject>().FirstOrDefault();
                        if (mo != null)
                        {
                            using (mo)
                            {
                                var p = Process.GetProcessById((int)(uint)mo["ParentProcessId"]);
                                return p;
                            }
                        }

                    }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        void IAudioSessionEventsHandler.OnVolumeChanged(float volume, bool isMuted) { }
        void IAudioSessionEventsHandler.OnDisplayNameChanged(string displayName) { }
        void IAudioSessionEventsHandler.OnChannelVolumeChanged(uint channelCount, nint newVolumes, uint channelIndex) { }
        void IAudioSessionEventsHandler.OnGroupingParamChanged(ref Guid groupingId) { }

        void IAudioSessionEventsHandler.OnIconPathChanged(string iconPath)
        {
            Debug.WriteLine("OnIconPathChanged: " + iconPath);
        }

        void IAudioSessionEventsHandler.OnStateChanged(AudioSessionState state)
        {
            if (state == AudioSessionState.AudioSessionStateExpired)
            {
                //I never observed that this occured. Therefore I am not sure if it is
                //the right way to relase our session. 
                this.ReleaseSessionDelayed(this.currentSession);
            }
        }

        void IAudioSessionEventsHandler.OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
        {
            //I never observed that this occured. Therefore I am not sure if it is
            //the right way to relase our session. 
            this.ReleaseSessionDelayed(this.currentSession);
        }

        /// <summary>
        /// Release our audio session, but don't do it just now, since we enter this
        /// from an event of <see cref="IAudioSessionEventsHandler"/> and the SDSK
        /// states that be must never unregister from the session from within such an event.
        /// </summary>
        /// <param name="session"></param>
        private void ReleaseSessionDelayed(AudioSessionControl? session)
        {
            //Marshal it to the UI thread.
            _ = this.Dispatcher.InvokeAsync(() =>
            {
                //Kind of paranoid delay of the actual releasing.
                DispatcherTimer timer = new DispatcherTimer();
                void TimerTick(object? sender, EventArgs e)
                {
                    timer.Stop();
                    timer.Tick -= TimerTick;
                    UnregisterFromSession(session);
                };
                timer.Tick += TimerTick;
                timer.Interval = TimeSpan.FromMilliseconds(100);
            });
        }

        private void UnregisterFromSession(AudioSessionControl? session)
        {
            try
            {
                if (session != null)
                {
                    session.UnRegisterEventClient(this);
                    session.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
