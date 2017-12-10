using Discord;
using Discord.Commands;
using FunSharp.Core.Games.Randomized;
using FunSharp.Core.Games.Strawpoll;
using KellyHoshira.Core.Events;
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

        protected DiscordClient m_client;
        public DiscordClient Client { get { return m_client; } protected set { m_client = value; } }

        protected CommandService m_commandService;


        public KellyHoshiraBot(SecretKeys keys)
            : this(keys, new DiscordConfigBuilder()
            {
                AppName = APP_NAME,
                AppUrl = APP_WEBSITE,
                AppVersion = APP_VERSION,
                LogLevel = LogSeverity.Info
            })
        {
        }
        public KellyHoshiraBot(SecretKeys keys, DiscordConfigBuilder config)
        {
            if (config.LogHandler == null)
                config.LogHandler = Log;

            Keys = keys;

            // Set Local Variables
            NetworkStatus = OnlineStatus.Offline;

            // Create the Client
            m_client = new DiscordClient(config);

            // Setup the Usings
            m_client.UsingCommands(x =>
            {
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Public;
            });
            m_commandService = m_client.GetService<CommandService>();
            SetupGeneralCommands();
            SetupFunCommands();

            // Setup Events
            m_client.MessageReceived += MessageReceived;
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
        private void SetupFunCommands()
        {
            m_commandService.CreateCommand("8ball")
                .Description("Checks with the Magic 8 Ball... ex: `8ball {question}` OR `8ball {question} -{positive/neutral/negative}`")
                .Parameter("Question", ParameterType.Unparsed)
                .Do(async e =>
                {
                    var question = e.GetArg("Question");

                    string text = $"{e.User.Mention} asked the 8ball `{question}`";
                    string flag = "";
                    string result = "";

                    var questionLower = question.ToLower();
                    if (questionLower.Contains("-positive"))
                    {
                        flag = "positive";
                        result = $"{Magic8Ball.Instance.RandomPositive()}";
                    }
                    else if (questionLower.Contains("-neutral"))
                    {
                        flag = "neutral";
                        result = $"{Magic8Ball.Instance.RandomNeutral()}";
                    }
                    else if (questionLower.Contains("-negative"))
                    {
                        flag = "negative";
                        result = $"{Magic8Ball.Instance.RandomNegative()}";
                    }
                    else
                    {
                        result = $"{Magic8Ball.Instance.RandomAll()}";
                    }

                    if (!string.IsNullOrWhiteSpace(flag))
                    {
                        text += $" with a flag of `{flag}`";
                    }

                    await e.Channel.SendMessage($"{text}\n{result}");
                });

            m_commandService.CreateCommand("coinToss")
                .Alias(new string[] { "coin", "coinFlip", "flipCoin", "tossCoin" })
                .Description("Tosses a coin ... ex: `coinToss` OR `coinToss -s`")
                .Parameter("Flag", ParameterType.Optional)
                .Do(async e =>
                {
                    string text = $"{e.User.Mention} threw a coin...";
                    var flag = e.GetArg("Flag").ToLower();

                    CoinResult coinFlip;
                    if (flag.Equals("-s"))
                    {
                        coinFlip = Coin.Instance.FlipCoinWithSide();
                        if (coinFlip == CoinResult.Side)
                        {
                            await e.Channel.SendMessage($"{text}\nThe coin landed on its {coinFlip}... wait, that shouldn't have happened");
                            return;
                        }
                    }
                    else
                    {
                        coinFlip = Coin.Instance.FlipCoin();
                    }

                    await e.Channel.SendMessage($"{text}\nThe coin landed on {coinFlip}");
                });

            m_commandService.CreateCommand("roll")
                .Description("Rolls Dice ... ex: `roll 3d20 Fire Damage` OR `roll 100`")
                .Parameter("DieString", ParameterType.Required)
                .Parameter("Text", ParameterType.Unparsed)
                .Do(async e =>
                {
                    var dieString = e.GetArg("DieString");
                    var text = e.GetArg("Text");

                    if (!dieString.ToLower().Contains("d"))
                    {
                        if (int.TryParse(dieString, out int result))
                        {
                            dieString = $"1d{result}";
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (string.IsNullOrWhiteSpace(dieString))
                    {
                        dieString = "1d100";
                    }

                    var dieRolls = Dice.Instance.RollMultiple(dieString);
                    string rolls = string.Join(", ", dieRolls);
                    int total = 0;
                    foreach (var roll in dieRolls)
                    {
                        total += roll;
                    }

                    await e.Channel.SendMessage($"{e.User.Mention} rolled {dieString} {text} \n" +
                                                $"Result: {rolls} | Total: {total} {text}");
                });

            m_commandService.CreateGroup("strawpoll", cgb =>
            {
                async Task PrintStrawPoll(CommandEventArgs e, StrawpollPoll poll)
                {
                    if (poll != null)
                    {
                        var pollString = $"{poll.title} \n" +
                            $"VOTE HERE - {poll.GetPollUrl()} \n";

                        for (int i = 0; i < poll.options.Count; i++)
                        {
                            pollString += string.Format("\t{0,-15}{1}\n", poll.votes[i] + " votes", poll.options[i]);
                        }

                        await e.Channel.SendMessage(pollString);
                    }
                    else
                    {
                        await e.Channel.SendMessage("There was an error retrieving this particular poll. Please try again later.");
                    }
                }

                cgb.CreateCommand("view")
                    .Alias(new string[] { "show", "display" })
                    .Description("Views the current results of a Strawpoll ... ex: `strawpoll view 1`")
                    .Parameter("PollID", ParameterType.Required)
                    .Do(async e =>
                    {
                        try
                        {
                            if (int.TryParse(e.GetArg("PollID"), out int id))
                            {
                                Debug.WriteLine($"Strawpoll View {id}");
                                StrawpollService service = StrawpollService.Instance;
                                StrawpollPoll poll = await service.GetPoll(id);
                                Debug.WriteLine("Get Complete");

                                await PrintStrawPoll(e, poll);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (Debugger.IsAttached)
                                Debugger.Break();
                        }
                    });

                cgb.CreateCommand("create")
                    .Alias(new string[] { "new" })
                    .Description("Creates a new Strawpoll ... ex: `strawpoll create Dogs or Cats? {Dogs;Cats}`")
                    .Parameter("PollString", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        try
                        {
                            string pollString = e.GetArg("PollString");
                            var title = pollString.Substring(0, pollString.IndexOf('{'));
                            var questionString = Regex.Match(pollString, @"\{([^)]*)\}").Groups[1].Value;

                            Debug.WriteLine(pollString);
                            Debug.WriteLine(title);
                            Debug.WriteLine(questionString);

                            StrawpollService service = StrawpollService.Instance;
                            var pollSettings = new StrawpollSettings();
                            pollSettings.Title = title;
                            pollSettings.Options.AddRange(questionString.Split(';'));

                            foreach (var s in pollSettings.Options)
                            {
                                Debug.WriteLine(s);
                            }

                            StrawpollPoll poll = await service.PostPoll(pollSettings);

                            Debug.WriteLine("Post Complete");

                            await PrintStrawPoll(e, poll);
                        }
                        catch (Exception ex)
                        {
                            if (Debugger.IsAttached)
                                Debugger.Break();
                        }
                    });
            });
        }
        #endregion

        #region Connect/Disconnect
        public event NetworkChangedEventHandler NetworkChanged;

        public void Connect()
        {
            m_client.ExecuteAndWait(async () =>
            {
                await m_client.Connect(Keys.UserToken, TokenType.Bot);

                NetworkStatus = OnlineStatus.Online;
                ConnectedTime = DateTime.UtcNow;
                NetworkChanged?.Invoke(this, new NetworkChangedEventArgs(NetworkStatus));
            });
        }
        public void Disconnect()
        {
            m_client.ExecuteAndWait(async () =>
            {
                await m_client.Disconnect();

                NetworkStatus = OnlineStatus.Offline;
                NetworkChanged?.Invoke(this, new NetworkChangedEventArgs(NetworkStatus));
            });
        }

        public async Task ConnectAsync()
        {
            await m_client.Connect(Keys.UserToken, TokenType.Bot);

            NetworkStatus = OnlineStatus.Online;
            ConnectedTime = DateTime.UtcNow;
            NetworkChanged?.Invoke(this, new NetworkChangedEventArgs(NetworkStatus));
        }
        public async Task DisconectAsync()
        {
            await m_client.Disconnect();

            NetworkStatus = OnlineStatus.Offline;
            NetworkChanged?.Invoke(this, new NetworkChangedEventArgs(NetworkStatus));
        }
        #endregion

        #region Events
        private void MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.Message.IsMentioningMe(true))
            {
                Debug.WriteLine(e.Message);
            }
        }

        public event EventHandler<LogMessageEventArgs> LogReceived;
        private void Log(object sender, LogMessageEventArgs e)
        {
            Debug.WriteLine(e.Message);
            LogReceived?.Invoke(sender, e);
        }
        #endregion
    }
}
