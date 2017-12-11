using Discord;
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
            Bot.Client.MessageReceived += Client_MessageReceived;
            Bot.LogReceived += Bot_LogReceived;
            await Bot.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task Client_MessageReceived(Discord.WebSocket.SocketMessage arg)
        {
            Console.WriteLine(arg.Content);
            return Task.CompletedTask;
        }

        private void Bot_LogReceived(object sender, LogMessage e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
