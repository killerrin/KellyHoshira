using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira.Core.Commands.General
{
    public class DeveloperCommand : ModuleBase<SocketCommandContext>
    {

        [Command("developer")]
        [Summary("Information about Kelly Hoshira ... ex: `developer`")]
        [Alias("owner", "parent", "father", "master", "credits")]
        public async Task DoAsync()
        {
            await Context.Channel.SendMessageAsync($"{KellyHoshiraBot.APP_NAME} - Version {KellyHoshiraBot.APP_VERSION} \n" +
                $"Created By: @killerrin \n" +
                $"My website is here: {KellyHoshiraBot.APP_WEBSITE} \n" +
                $"View my source code here: {KellyHoshiraBot.APP_SOURCE_CODE}");
        }
    }
}
