using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;
using WinBot.Misc;
using WinBot.Util;

namespace WinBot.Commands.Main
{
    public class LeaderboardCommand : BaseCommandModule
    {
        [Command("lb")]
        [Description("Show the server leaderboard")]
        [Attributes.Category(Category.Main)]
        public async Task Leaderboard(CommandContext Context)
        {
            // Get the leaderboard and #1 user
            List<User> leaderboard = Leveling.GetOrderedLeaderboard();
            DiscordUser topUser = Bot.client.GetUserAsync(leaderboard[0].id).Result;

            // Generate an embed description
            string description = "";
            int userCounter = 0;
            bool hasDisplayedCurrentUser = false;
            foreach(User lbUser in leaderboard) {
                if(userCounter < 10) {
                    description += $"**{userCounter+1}.** {lbUser.username} - {lbUser.level} ({MiscUtil.FormatNumber((int)lbUser.totalxp)} Total XP)\n";
                    if(lbUser.id == Context.User.Id)
                        hasDisplayedCurrentUser = true;
                }
                else if(lbUser.id == Context.User.Id) {
                    description += $"**{userCounter+1}.** {lbUser.username} - {lbUser.level} ({MiscUtil.FormatNumber((int)lbUser.totalxp)} Total XP)\n";
                    if(userCounter != 10) 
                        description += "...";
                }
                else if(userCounter == 10 && !hasDisplayedCurrentUser) {
                    description += "...\n";
                }
                userCounter++;
            }

            // Create the embed
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithThumbnail(topUser.GetAvatarUrl(DSharpPlus.ImageFormat.Jpeg));
            eb.WithDescription(description);
            eb.WithFooter("Note: this is NOT a temporary leaderboard");
            await Context.ReplyAsync("", eb.Build());
        }
    }
}