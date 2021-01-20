using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

using Humanizer;

namespace WinWorldBot.Commands
{
    public class AboutCommand : ModuleBase<SocketCommandContext>
    {
        [Command("about")]
        [Summary("Shows information about the bot|")]
        [Priority(Category.Main)]
        private async Task About(string arg = null)
        {
            // Collect data from files
            string nortonS = File.ReadAllText("nortons");
            string ohS = File.ReadAllText("oh");
            string okS = File.ReadAllText("ok");
            string question = File.ReadAllText("?");
            string ahyest = File.ReadAllText("ahyes");
            var uptime = DateTime.Now.Subtract(Bot.startTime);
            int.TryParse(nortonS, out int norton);
            int.TryParse(ohS, out int oh);
            int.TryParse(okS, out int okay);
            int.TryParse(question, out int questions);
            int.TryParse(ahyest, out int ahyes);

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);
            eb.WithTitle("WinWorld Bot");
            eb.WithThumbnailUrl(Bot.client.CurrentUser.GetAvatarUrl());
            eb.AddField("Language", "C#", true);
            eb.AddField("Library", "Discord.NET", true);
            eb.AddField("Author", "Starman#8456", true);
            eb.AddField("Member Count", Context.Guild.MemberCount, true);
            eb.AddField("Norton Count", norton, true);
            eb.AddField("\"Ah, yes\" Count", ahyes, true);
            eb.AddField("Uptime", uptime.Humanize(3), true);
            if(Context.Channel.Id == 563206142755471381 || arg == "-a" && Context.Message.Author.Id == 363850072309497876) {
                eb.AddField("Yuds' \"?\" Count", questions, true);
            }

            await ReplyAsync("", false, eb.Build());
        }
    }
}