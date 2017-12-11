using Discord.Commands;
using FunSharp.Core.Games.Randomized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira.Core.Commands.Fun
{
    public class EightBallCommand : ModuleBase<SocketCommandContext>
    {
        [Command("8ball")]
        [Summary("Checks with the Magic 8 Ball... ex: `8ball {question}` OR `8ball {question} -{positive/neutral/negative}`")]
        public async Task DoAsync([Remainder][Summary("The question to ask")] string question)
        {
            string text = $"{Context.User.Mention} asked the 8ball `{question}`";
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

            await Context.Channel.SendMessageAsync($"{text}\n{result}");
        }
    }
}
