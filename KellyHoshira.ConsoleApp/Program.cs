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
        static void Main(string[] args)
        {
            // Load the Secret Keys
            SecretKeys keys = SecretKeys.Load("Secret.txt");

            // Create the Bot
            KellyHoshiraBot bot = new KellyHoshiraBot(keys);
            bot.Client.MessageReceived += Client_MessageReceived;
            bot.LogReceived += Bot_LogReceived;
            bot.Connect();
        }

        private static void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.Message.IsMentioningMe(true))
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void Bot_LogReceived(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
