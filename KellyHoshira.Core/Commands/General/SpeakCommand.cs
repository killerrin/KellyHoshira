using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira.Core.Commands.General
{
    public class SpeakCommand : ModuleBase<SocketCommandContext>
    {

        [Command("speak")]
        [Summary("Speaks through TTS what is supplied ... ex: `speak INSERT TEXT HERE`")]
        [Alias("tts", "voice")]
        public async Task DoAsync([Remainder][Summary("What to say")]string whatToSay)
        {
            await Context.Channel.SendMessageAsync(whatToSay, true);
        }
    }
}
