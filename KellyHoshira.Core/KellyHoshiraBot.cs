using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FunSharp.Core.Games.Randomized;
using FunSharp.Core.Games.Strawpoll;
using KellyHoshira.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KellyHoshira.Core
{
    public class KellyHoshiraBot
    {
        #region Consts
        public const string APP_NAME = "Kelly Hoshira";
        public const string APP_BOT_USER_NAME = "KellyHoshira#1789";
        public const string APP_VERSION = "1.0";
        public const string APP_WEBSITE = "https://killerrin.github.io/KellyHoshira/";
        public const string APP_SOURCE_CODE = "https://github.com/killerrin/KellyHoshira";
        #endregion
        SecretKeys Keys { get; set; }

        public OnlineStatus NetworkStatus { get; protected set; }
        public DateTime ConnectedTime { get; protected set; }

        protected DiscordSocketClient m_client;
        public DiscordSocketClient Client { get { return m_client; } protected set { m_client = value; } }

        protected CommandService m_commandService;
        protected IServiceProvider _services;

        public KellyHoshiraBot(SecretKeys keys)
            : this(keys, new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info
            })
        {
            DiscordConfig config = new DiscordConfig();
        }
        public KellyHoshiraBot(SecretKeys keys, DiscordSocketConfig config)
        {
            Keys = keys;

            // Set Local Variables
            NetworkStatus = OnlineStatus.Offline;

            // Create the Client
            m_client = new DiscordSocketClient(config);
            m_client.Log += Log;

            // Setup the Commands
            m_commandService = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(m_client)
                .AddSingleton(m_commandService)
                .BuildServiceProvider();

            // Setup Events
            m_client.MessageReceived += MessageReceived;
            m_commandService.AddModulesAsync(System.Reflection.Assembly.GetEntryAssembly());


            SetupGeneralCommands();
        }

        #region Commands
        private void SetupGeneralCommands()
        {
            m_commandService.CreateCommand("greet")
                .Alias(new string[] { "gr", "hi", "hello" })
                .Description("Greets a person. ... ex: `greet @username`")
                .Parameter("GreetedPerson", ParameterType.Optional)
                .Do(async e =>
                {
                    var greetedPerson = e.GetArg("GreetedPerson");
                    if (string.IsNullOrWhiteSpace(greetedPerson))
                    {
                        await e.Channel.SendMessage($"Hello!");
                    }
                    else
                    {
                        await e.Channel.SendMessage($"Hello, {e.GetArg("GreetedPerson")}");
                    }
                });

            m_commandService.CreateCommand("bye")
                .Alias(new string[] { "cya", "g2g", "gb", "bb" })
                .Description("Says 'Bye' to a person ... ex: `bye @username`")
                .Parameter("LeavingPerson", ParameterType.Optional)
                .Do(async e =>
                {
                    var leavingPerson = e.GetArg("LeavingPerson");
                    if (string.IsNullOrWhiteSpace(leavingPerson))
                    {
                        await e.Channel.SendMessage($"See you later");
                    }
                    else
                    {
                        await e.Channel.SendMessage($"See you later, {e.GetArg("LeavingPerson")}");
                    }
                });

            m_commandService.CreateCommand("say")
                .Alias(new string[] { "echo" })
                .Description("Echo's back what is supplied ... ex: `say INSERT TEXT HERE`")
                .Parameter("WhatToSay", ParameterType.Unparsed)
                .Do(async e =>
                {
                    await e.Channel.SendMessage(e.GetArg("WhatToSay"));
                });

            m_commandService.CreateCommand("speak")
                .Alias(new string[] { "tts" })
                .Description("Speaks through TTS what is supplied ... ex: `speak INSERT TEXT HERE`")
                .Parameter("WhatToSay", ParameterType.Unparsed)
                .Do(async e =>
                {
                    await e.Channel.SendTTSMessage(e.GetArg("WhatToSay"));
                });

            m_commandService.CreateCommand("developer")
                .Alias(new string[] { "owner", "parent", "father", "master", "credits" })
                .Description("Information about Kelly Hoshira ... ex: `developer`")
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"{APP_NAME} - Version {APP_VERSION} \n" +
                        $"Created By: @killerrin \n" +
                        $"My website is here: {APP_WEBSITE} \n" +
                        $"View my source code here: {APP_SOURCE_CODE}");
                });

            m_commandService.CreateCommand("uptime")
                .Description("Gets the current uptime ... ex: `uptime`")
                .Do(async e =>
                {
                    TimeSpan difference = DateTime.UtcNow.Subtract(ConnectedTime);
                    await e.Channel.SendMessage($"The current Uptime is: {difference}");
                });

        }
        #endregion

        #region Connect/Disconnect
        public event NetworkChangedEventHandler NetworkChanged;

        public async Task ConnectAsync()
        {
            await m_client.LoginAsync(TokenType.Bot, Keys.UserToken);

            NetworkStatus = OnlineStatus.Online;
            ConnectedTime = DateTime.UtcNow;
            NetworkChanged?.Invoke(this, new NetworkChangedEventArgs(NetworkStatus));

            await m_client.StartAsync();
        }
        public async Task DisconectAsync()
        {
            await m_client.LogoutAsync();

            NetworkStatus = OnlineStatus.Offline;
            NetworkChanged?.Invoke(this, new NetworkChangedEventArgs(NetworkStatus));

            await m_client.StopAsync();
        }
        #endregion

        #region Events
        private Task MessageReceived(SocketMessage e)
        {
            Debug.WriteLine(e.Content);
            return Task.CompletedTask;
        }

        public event EventHandler<LogMessage> LogReceived;
        private Task Log(LogMessage e)
        {
            Debug.WriteLine(e.Message);
            LogReceived?.Invoke(this, e);

            return Task.CompletedTask;
        }
        #endregion
    }
}
