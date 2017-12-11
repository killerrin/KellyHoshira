using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira.Core.Commands.General
{
    public class ByeCommand : ModuleBase<SocketCommandContext>
    {

        [Command("bye")]
        [Summary("Says 'Bye' to a person ... ex: `bye @username`")]
        [Alias("cya", "g2g", "gb", "bb")]
        public async Task DoAsync([Remainder][Summary("Person to say bye to")]string leavingPerson = "")
        {
            if (string.IsNullOrWhiteSpace(leavingPerson))
            {
                await Context.Channel.SendMessageAsync($"See you later");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"See you later, {leavingPerson}");
            }
        }
    }
}
