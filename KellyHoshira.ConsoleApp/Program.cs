using Discord;
using Discord.WebSocket;
using KellyHoshira.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira.ConsoleApp
{
    public class Program
    {
        private KellyHoshiraBot Bot;
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            // Load the Secret Keys
            SecretKeys keys = SecretKeys.Load("Secret.txt");

            // Create the Bot
            Bot = new KellyHoshiraBot(keys);
            await Bot.InitializeAsync();
            Bot.OnMessageReceived += Bot_OnMessageReceived;
            Bot.OnLogReceived += Bot_OnLogReceived;
            await Bot.ConnectAsync();

            await Task.Delay(-1);
        }

        private void Bot_OnMessageReceived(object sender, SocketUserMessage e)
        {
            Console.WriteLine(e.Content);
        }

        private void Bot_OnLogReceived(object sender, LogMessage e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
