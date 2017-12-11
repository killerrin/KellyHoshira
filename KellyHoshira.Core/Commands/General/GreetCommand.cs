using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira.Core.Commands.General
{
    public class GreetCommand : ModuleBase<SocketCommandContext>
    {
        [Command("greet")]
        [Summary("Greets a person. ... ex: `greet @username`")]
        [Alias("gr", "hi", "hello")]
        public async Task DoAsync([Remainder][Summary("Person to Greet")] string greetedPerson = "")
        {
            if (string.IsNullOrWhiteSpace(greetedPerson))
            {
                await Context.Channel.SendMessageAsync($"Hello!");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Hello, {greetedPerson}");
            }
        }
    }
}
