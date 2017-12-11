using Discord.Commands;
using FunSharp.Core.Games.Randomized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira.Core.Commands.Fun
{
    public class CoinTossCommand : ModuleBase<SocketCommandContext>
    {
        [Command("coinToss")]
        [Summary("Tosses a coin ... ex: `coinToss` OR `coinToss -s`")]
        [Alias("coin", "coinFlip", "flipCoin", "tossCoin")]
        public async Task DoAsync([Remainder][Summary("Flag")] string flag = "")
        {
            string text = $"{Context.User.Mention} threw a coin...";
            flag = flag.ToLower();

            CoinResult coinFlip;
            if (flag.Equals("-s"))
            {
                coinFlip = Coin.Instance.FlipCoinWithSide();
                if (coinFlip == CoinResult.Side)
                {
                    await Context.Channel.SendMessageAsync($"{text}\nThe coin landed on its {coinFlip}... wait, that shouldn't have happened");
                    return;
                }
            }
            else
            {
                coinFlip = Coin.Instance.FlipCoin();
            }

            await Context.Channel.SendMessageAsync($"{text}\nThe coin landed on {coinFlip}");
        }
    }
}
