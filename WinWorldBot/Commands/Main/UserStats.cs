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
            if(triviaPercent >= 95 && triviaPercent <= 100) level = "A+";
            else if(triviaPercent >= 87 && triviaPercent <= 94) level = "A";
            else if(triviaPercent >= 80 && triviaPercent <= 86) level = "A-";
            else if(triviaPercent >= 77 && triviaPercent <= 79) level = "B+";
            else if(triviaPercent >= 73 && triviaPercent <= 76) level = "B";
            else if(triviaPercent >= 70 && triviaPercent <= 72) level = "B-";
            else if(triviaPercent >= 67 && triviaPercent <= 69) level = "C+";
            else if(triviaPercent >= 63 && triviaPercent <= 66) level = "C";
            else if(triviaPercent >= 60 && triviaPercent <= 62) level = "C-";
            else if(triviaPercent >= 57 && triviaPercent <= 59) level = "D+";
            else if(triviaPercent >= 53 && triviaPercent <= 56) level = "D";
            else if(triviaPercent >= 50 && triviaPercent <= 52) level = "D-";
            else level = "F";
            Log.Write($"Grade is {triviaPercent}% and {level}. Total is {totalTrivia}, correct is {u.CorrectTrivia} and incorrect is {u.IncorrectTrivia}");

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