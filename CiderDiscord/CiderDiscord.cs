using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace C2Windows.CiderDiscord
{
    public class CiderDiscord
    {
        public static DiscordRpcClient client;
        
        public static void InitDiscord()
        {
            Console.WriteLine("Init Discord");

            client = new DiscordRpcClient("911790844204437504");

            //Set the logger
            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            //Subscribe to events
            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            //Connect to the RPC
            client.Initialize();

            //Set the rich presence
            //Call this as many times as you want and anywhere in your code.
            /**
            client.SetPresence(new RichPresence()
            {
                Details = "Example Project",
                State = "csharp example",
                Assets = new Assets()
                {
                    LargeImageKey = "https://i.scdn.co/image/ab6761610000e5ebc5ceb05f152103b2b70d3b07",
                    LargeImageText = "Discord IPC Library",
                    SmallImageKey = "https://i.scdn.co/image/ab6761610000e5ebc5ceb05f152103b2b70d3b07"
                }
            });
            **/
        }

        public static void StopDiscord()
        {
            if (client == null)
            {
                return;
            }


            client.Deinitialize();
            client.Dispose();
        }

    }
}
