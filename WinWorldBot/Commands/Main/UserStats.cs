using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Data;
using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class UserStatsCommand : ModuleBase<SocketCommandContext>
    {
        [Command("userstats")]
        [Priority(Category.Main)]
        [Summary("Get stats on a user|[User]")]
        private async Task UserStats(SocketUser User = null)
        {
            if (User == null) User = Context.Message.Author;
            User u = UserData.GetUser(User);

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithAuthor(User);
            eb.WithFooter($"Data for {User} starts on {u.StartedLogging.ToShortDateString()}");
            eb.WithColor(Bot.config.embedColour);
            eb.AddField("Total Messages", u.Messages.Count, true);
            eb.AddField("Most Active Channel", GetMostActiveChannel(u), true);
            eb.AddField("Correct Trivia Answers", u.CorrectTrivia, true);
            eb.AddField("Incorrect Trivia Answers", u.IncorrectTrivia, true);

            await ReplyAsync("", false, eb.Build());
        }

        string GetMostActiveChannel(User u)
        {
            return u.Messages.GroupBy(i => i).OrderByDescending(grp => grp.Count())
                    .Select(grp => grp.Key.Channel).First();
        }
    }
}