using Discord;
using Discord.Commands;
using Discord.WebSocket;
using KellyHoshira.Core.Commands.Fun;
using KellyHoshira.Core.Commands.General;
using KellyHoshira.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public SecretKeys Keys { get; set; }
        public bool Initialized { get; private set; }

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
            Initialized = false;

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
        }

        public async Task InitializeAsync()
        {
            if (Initialized) return;
            
            // Add all Modules
            //await m_commandService.AddModulesAsync(System.Reflection.Assembly.GetEntryAssembly());

            // General Commands
            await m_commandService.AddModuleAsync<ByeCommand>();
            await m_commandService.AddModuleAsync<DeveloperCommand>();
            await m_commandService.AddModuleAsync<GreetCommand>();
            await m_commandService.AddModuleAsync<SayCommand>();
            await m_commandService.AddModuleAsync<SpeakCommand>();
            await m_commandService.AddModuleAsync<UptimeCommand>();

            // Fun Commands
            await m_commandService.AddModuleAsync<CoinTossCommand>();
            await m_commandService.AddModuleAsync<DiceRollCommand>();
            await m_commandService.AddModuleAsync<EightBallCommand>();
            await m_commandService.AddModuleAsync<StrawPollCommand>();

            Debug.WriteLine("Modules");
            foreach (var item in m_commandService.Modules)
            {
                Debug.WriteLine(item.Name + "|" + item.Summary);
            }
            Debug.WriteLine("Commands");
            foreach (var item in m_commandService.Commands)
            {
                Debug.WriteLine(item.Name + "|" + item.Summary);
            }

            Initialized = true;
        }

        #region Connect/Disconnect
        public event NetworkChangedEventHandler NetworkChanged;

        public async Task ConnectAsync()
        {
            if (!Initialized) await InitializeAsync();

            await m_client.LoginAsync(TokenType.Bot, Keys.UserToken);

            NetworkStatus = OnlineStatus.Online;
            ConnectedTime = DateTime.UtcNow;
            NetworkChanged?.Invoke(this, new NetworkChangedEventArgs(NetworkStatus));

            await m_client.StartAsync();
        }
        public async Task DisconectAsync()
        {
            await m_client.LogoutAsync();
            await m_client.StopAsync();

            NetworkStatus = OnlineStatus.Offline;
            NetworkChanged?.Invoke(this, new NetworkChangedEventArgs(NetworkStatus));
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
