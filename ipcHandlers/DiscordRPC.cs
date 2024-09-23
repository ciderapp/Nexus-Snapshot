using C2Windows.CiderDiscord;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using DiscordRPC;
using DiscordRPC.Logging;
using Windows.Media.Protection.PlayReady;

namespace ipcHandlers
{

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public partial class DiscordRPC
    {
        public DiscordRPC() { }
        public void SetActivity(
            string state,
            string details,
            string artwork,
            int start,
            int end,
            string largeImageText
        )
        {
                
            // convert the start to a DateTime
            DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            startDateTime = startDateTime.AddSeconds(start);
            // convert the end to a DateTime
            DateTime endDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            endDateTime = endDateTime.AddSeconds(end);


            Console.WriteLine("Discord Update", state, end, start, details, artwork, largeImageText);
            CiderDiscord.client.SetPresence(new RichPresence()
            {
                Details = details,
                State = state,
                Assets = new Assets()
                {
                    LargeImageKey = artwork,
                    LargeImageText = largeImageText,
                    SmallImageKey = artwork
                },
                Timestamps = new Timestamps()
                {
                    Start = startDateTime,
                    End = endDateTime
                }
            });
        }

        public void TestActivity()
        {
            CiderDiscord.client.SetPresence(new RichPresence()
            {
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow,
                    End = DateTime.UtcNow.AddMinutes(5)
                },
                Details = "C3App",
                State = "State test",
                Assets = new Assets()
                {
                    LargeImageKey = "https://i.scdn.co/image/ab6761610000e5ebc5ceb05f152103b2b70d3b07",
                    LargeImageText = "Discord IPC Library",
                    SmallImageKey = "https://i.scdn.co/image/ab6761610000e5ebc5ceb05f152103b2b70d3b07"
                }
            });
        }

        public void ClearActivity()
        {
            CiderDiscord.client.ClearPresence();
        }
    }
}
