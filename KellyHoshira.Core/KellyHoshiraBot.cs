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
        public static KellyHoshiraBot Instance { get; set; }

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
        }
        public KellyHoshiraBot(SecretKeys keys, DiscordSocketConfig config)
        {
            // Set the Instance
            Instance = this;

            // Set the Secret Keys
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
            
        }

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
        public event EventHandler<SocketUserMessage> OnMessageReceived;
        private async Task MessageReceived(SocketMessage e)
        {
            var message = e as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command, based on if it starts with a mention prefix
            if (!message.HasMentionPrefix(m_client.CurrentUser, ref argPos)) return;
            OnMessageReceived?.Invoke(this, message);
            Debug.WriteLine(message.Content);

            // Create a Command Context
            var context = new SocketCommandContext(m_client, message);
            
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await m_commandService.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        public event EventHandler<LogMessage> OnLogReceived;
        private async Task Log(LogMessage e)
        {
            Debug.WriteLine(e.Message);
            OnLogReceived?.Invoke(this, e);
        }
        #endregion
    }
}
