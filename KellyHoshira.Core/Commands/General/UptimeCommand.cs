using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira.Core.Commands.General
{
    public class UptimeCommand : ModuleBase<SocketCommandContext>
    {
        [Command("uptime")]
        [Summary("Gets the current uptime...ex: `uptime`")]
        //[Alias()]
        public async Task DoAsync()
        {
            TimeSpan difference = DateTime.UtcNow.Subtract(KellyHoshiraBot.Instance.ConnectedTime);
            await Context.Channel.SendMessageAsync($"The current Uptime is: {difference}");
        }
    }
}
