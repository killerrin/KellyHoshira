using Discord.Commands;
using FunSharp.Core.Games.Randomized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira.Core.Commands.Fun
{
    public class DiceRollCommand : ModuleBase<SocketCommandContext>
    {
        [Command("roll")]
        [Summary("Rolls Dice ... ex: `roll 3d20 Fire Damage` OR `roll 100`")]
        [Alias("dice")]
        public async Task DoAsync(
            [Summary("The Die string... ex: `2d4` OR `100`")] string dieString,
            [Remainder][Summary("Text")] string text = "")
        {
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

            await Context.Channel.SendMessageAsync($"{Context.User.Mention} rolled {dieString} {text} \n" +
                                        $"Result: {rolls} | Total: {total} {text}");
        }
    }
}
