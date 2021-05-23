using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;
using WinBot.Misc;

namespace WinBot.Commands.Main
{
    public class LeaderboardCommand : BaseCommandModule
    {
        [Command("leaderboard")]
        [Aliases(new string[]{ "lb" })]
        [Description("Show the server leaderboard")]
        [Category(Category.Main)]
        public async Task Leaderboard(CommandContext Context)
        {
            // Get the leaderboard and #1 user
            List<User> leaderboard = Leveling.GetOrderedLeaderboard();
            DiscordUser topUser = Bot.client.GetUserAsync(leaderboard[0].id).Result;

            // Generate an embed description
            string description = "";
            int userCounter = 0;
            foreach(User lbUser in leaderboard) {
                if(userCounter < 10) {
                    description += $"**{userCounter+1}.** {lbUser.username} - {lbUser.level} ({lbUser.totalxp} Total XP)\n";
                }
                userCounter++;
            }

            // Create the embed
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithThumbnail(topUser.GetAvatarUrl(DSharpPlus.ImageFormat.Jpeg));
            eb.WithDescription(description);
            await Context.RespondAsync("", eb.Build());
        }
    }
}