using Discord;
using Discord.Commands;
using FunSharp.Core.Games.Randomized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira
{
    public class KellyHoshiraBot
    {
        public const string APP_NAME = "Kelly Hoshira";
        public const string CLIENT_ID = "294889055663947776";
        public const string APP_BOT_USER_NAME = "KellyHoshira#1789";
        public const string APP_BOT_USER_TOKEN = "Mjk0ODg5MDU1NjYzOTQ3Nzc2.C7dx-Q.BbipBYQMm5ixqoc95v0OoulFxkg";

        private DiscordClient m_client;
        private CommandService m_commandService;

        public KellyHoshiraBot()
        {
            DiscordConfigBuilder config = new DiscordConfigBuilder();
            config.AppName = "Kelly Hoshira";
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
            m_commandService = m_client.GetService<CommandService>();
            SetupGeneralCommands();
            SetupFunCommands();

            // Setup Events
            m_client.MessageReceived += MessageReceived;
        }

        private void SetupGeneralCommands()
        {
            // Standard Commands
            m_commandService.CreateCommand("greet")
                .Alias(new string[] { "gr", "hi", "hello" })
                .Description("Greets a person.")
                .Parameter("GreetedPerson", ParameterType.Required)
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"Hello, {e.GetArg("GreetedPerson")}");
                });

            m_commandService.CreateCommand("bye")
                .Alias(new string[] { "cya", "g2g", "gb", "bb" })
                .Description("Says 'Bye' to a person")
                .Parameter("LeavingPerson", ParameterType.Required)
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"See you later, {e.GetArg("LeavingPerson")}");
                });

            m_commandService.CreateCommand("say")
                .Alias(new string[] { "echo" })
                .Description("Echo's back what is said")
                .Parameter("WhatToSay", ParameterType.Unparsed)
                .Do(async e =>
                {
                    await e.Channel.SendMessage(e.GetArg("WhatToSay"));
                });

            m_commandService.CreateCommand("owner")
                .Alias(new string[] { "master", "credits" })
                .Description("Information about Kelly Hoshira")
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"Created By: @killerrin");
                    await e.Channel.SendMessage($"My website is here: https://killerrin.github.io/KellyHoshira/");
                    await e.Channel.SendMessage($"View my source code here: https://github.com/killerrin/KellyHoshira");
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

                    await e.Channel.SendMessage(text);
                    await e.Channel.SendMessage(result);
                });

            m_commandService.CreateCommand("coinToss")
                .Alias(new string[] { "coin", "coinFlip", "flipCoin", "tossCoin" })
                .Description("Tosses a coin ... ex: `coinToss` OR `coinToss -s`")
                .Parameter("Flag", ParameterType.Optional)
                .Do(async e =>
                {
                    var flag = e.GetArg("Flag").ToLower();

                    await e.Channel.SendMessage($"{e.User.Mention} threw a coin...");

                    CoinResult coinFlip;
                    if (flag.Equals("-s")) {
                        coinFlip = Coin.Instance.FlipCoinWithSide();
                        if (coinFlip == CoinResult.Side)
                        {
                            await e.Channel.SendMessage($"The coin landed on its {coinFlip}... wait, that shouldn't have happened");
                            return;
                        }
                    }
                    else
                    {
                        coinFlip = Coin.Instance.FlipCoin();
                    }

                    await e.Channel.SendMessage($"The coin landed on {coinFlip}");
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

                    await e.Channel.SendMessage($"{e.User.Mention} rolled {dieString} {text}");
                    await e.Channel.SendMessage($"Result: {rolls} | Total: {total} {text}");
                });
        }

        public void Run()
        {
            m_client.ExecuteAndWait(async () =>
            {
                await m_client.Connect(APP_BOT_USER_TOKEN, TokenType.Bot);
            });
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
