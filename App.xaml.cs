using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace C2Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            //Console.Title = "C3App";
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "/dev")
                {
                    Console.WriteLine("[DNC] Starting in Dev Mode");
                    AppGlobals.HostURL = "http://localhost:9000";
                    AppGlobals.DevMode = true;
                }
            }

            System.Windows.Forms.NotifyIcon icon = new()
            {
                Visible = true,
                Text = "C3App",
            };

            icon.Click += new EventHandler((s, e) =>
            {
                System.Windows.MessageBox.Show("C3App is running in the background.");
            });



            //C3Agent.StartAgent();
            CoreWV.InitWV();
            CiderDiscord.CiderDiscord.InitDiscord();
            AppGlobals.mainWindow.Show();
            //var sw = new SettingsWindow();
            //sw.Show();
        }
    }
}
