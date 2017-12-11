using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira.Core.Commands.General
{
    public class SayCommand : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Echo's back what is supplied ... ex: `say INSERT TEXT HERE`")]
        [Alias("echo", "repeat")]
        public async Task DoAsync([Remainder][Summary("What to say")]string whatToSay)
        {
            await Context.Channel.SendMessageAsync(whatToSay);
        }
    }
}
