using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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

            // **TERRIBLE** WAY TO GET GRADE LEVEL. FIX THIS LATER PLEASE!
            int totalTrivia = u.CorrectTrivia + u.IncorrectTrivia;
            float triviaPercent = u.CorrectTrivia / totalTrivia;
            string level = "";
            if(triviaPercent >= 95) level = "A+";
            else if(triviaPercent >= 87) level = "A";
            else if(triviaPercent >= 80) level = "A-";
            else if(triviaPercent >= 77) level = "B+";
            else if(triviaPercent >= 73) level = "B";
            else if(triviaPercent >= 70) level = "B-";
            else if(triviaPercent >= 67) level = "C+";
            else if(triviaPercent >= 63) level = "C";
            else if(triviaPercent >= 60) level = "C-";
            else if(triviaPercent >= 57) level = "D+";
            else if(triviaPercent >= 53) level = "D";
            else if(triviaPercent >= 50) level = "D-";
            else level = "F";

            eb.AddField("Trivia Grade", level, true);

            await ReplyAsync("", false, eb.Build());
        }

        string GetMostActiveChannel(User u)
        {
            List<string> elements = new List<string>();
            foreach(UserMessage msg in u.Messages)
                elements.Add(msg.Channel);

            return elements.GroupBy(i => i).OrderByDescending(grp => grp.Count())
                    .Select(grp => grp.Key).First();
        }
    }
}