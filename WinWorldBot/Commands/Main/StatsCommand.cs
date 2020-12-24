using System;
using System.IO;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Humanizer;
using Humanizer.DateTimeHumanizeStrategy;

namespace WinWorldBot.Commands
{
    public class StatsCommand : ModuleBase<SocketCommandContext>
    {
        [Command("stats")]
        [Summary("Shows various statistics and things|")]
        [Priority(Category.Main)]
        private async Task Stats()
        {
            // Collect data from files
            string nortonS = File.ReadAllText("nortons");
            string ohS = File.ReadAllText("oh");
            string okS = File.ReadAllText("ok");
            var uptime = DateTime.Now.Subtract(Bot.startTime);
            int.TryParse(nortonS, out int norton);
            int.TryParse(ohS, out int oh);
            int.TryParse(okS, out int okay);

            // Build the embed
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithTitle("Statistics");
            eb.WithColor(Bot.config.embedColour);
            eb.WithThumbnailUrl(Context.Guild.IconUrl);
            eb.AddField("Member Count", Context.Guild.MemberCount, true);
            eb.AddField("Norton Count", norton, true);
            if(Context.Channel.Id == 563206142755471381) {
                eb.AddField("Oh Count", oh, true);
                eb.AddField("Ok Count", okay, true);
            }
            //eb.AddField("Uptime", $"{uptime.Days}:{uptime.Hours}:{uptime.Minutes}:{uptime.Seconds}", true);
            eb.AddField("Uptime", TimeSpanHumanizeExtensions.Humanize(uptime), true);

            await ReplyAsync("", false, eb.Build());
        }
    }
}