using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira
{
    public class KellyHoshiraBot
    {

        private DiscordClient m_client;
        private CommandService m_commandService;

        public KellyHoshiraBot()
        {
            DiscordConfigBuilder config = new DiscordConfigBuilder();
            config.AppName = "KellyHoshira";
            config.AppUrl = "https://github.com/killerrin/KellyHoshira";
            config.AppVersion = "1.0";
            config.LogLevel = LogSeverity.Info;
            config.LogHandler = Log;

            m_client = new DiscordClient(config);

            // Setup the Usings
            m_client.UsingCommands(x => {
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Public;
            });
            m_client.GetService<CommandService>();

            // Setup Events
            m_client.MessageReceived += MessageReceived;
        }

        public void Run()
        {
            m_client.ExecuteAndWait(async () =>
            {
                await m_client.Connect("Mjk0ODg5MDU1NjYzOTQ3Nzc2.C7btfQ.MgaU7lzmOMkCGVaXz_3ijHZm6F0", TokenType.Bot);
            });
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
