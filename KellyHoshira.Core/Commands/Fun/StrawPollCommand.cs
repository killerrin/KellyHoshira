using Discord.Commands;
using FunSharp.Core.Games.Strawpoll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KellyHoshira.Core.Commands.Fun
{
    [Group("Strawpoll")]
    public class StrawPollCommand : ModuleBase<SocketCommandContext>
    {
        [Command("view")]
        [Summary("Views the current results of a Strawpoll ... ex: `strawpoll view 1`")]
        [Alias("show", "display")]
        public async Task ViewAsync([Remainder][Summary("Poll ID")] string pollID)
        {
            try
            {
                if (int.TryParse(pollID, out int id))
                {
                    Debug.WriteLine($"Strawpoll View {id}");
                    StrawpollService service = StrawpollService.Instance;
                    StrawpollPoll poll = await service.GetPoll(id);
                    Debug.WriteLine("Get Complete");

                    await Context.Channel.SendMessageAsync(PrintStrawPoll(poll));
                }
            }
            catch (Exception)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
            }
        }

        [Command("create")]
        [Summary("Creates a new Strawpoll ... ex: `strawpoll create Dogs or Cats? {Dogs;Cats}`")]
        [Alias("new", "make")]
        public async Task CreateAsync([Remainder][Summary("Poll String")] string pollString)
        {
            try
            {
                var title = pollString.Substring(0, pollString.IndexOf('{'));
                var questionString = Regex.Match(pollString, @"\{([^)]*)\}").Groups[1].Value;

                Debug.WriteLine(pollString);
                Debug.WriteLine(title);
                Debug.WriteLine(questionString);

                StrawpollService service = StrawpollService.Instance;
                var pollSettings = new StrawpollSettings();
                pollSettings.Title = title;
                pollSettings.Options.AddRange(questionString.Split(';'));

                foreach (var s in pollSettings.Options)
                {
                    Debug.WriteLine(s);
                }

                StrawpollPoll poll = await service.PostPoll(pollSettings);

                Debug.WriteLine("Post Complete");

                await Context.Channel.SendMessageAsync(PrintStrawPoll(poll));
            }
            catch (Exception)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
            }
        }

        private string PrintStrawPoll(StrawpollPoll poll)
        {
            if (poll != null)
            {
                var pollString = $"{poll.title} \n" +
                    $"VOTE HERE - {poll.GetPollUrl()} \n";

                for (int i = 0; i < poll.options.Count; i++)
                {
                    pollString += string.Format("\t{0,-15}{1}\n", poll.votes[i] + " votes", poll.options[i]);
                }

                return pollString;
            }
            else
            {
                return "There was an error retrieving this particular poll. Please try again later.";
            }
        }
    }
}
