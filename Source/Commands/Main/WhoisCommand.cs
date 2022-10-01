using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class WhoisCommand : BaseCommandModule
    {
        [Command("whois")]
        [Description("Gets basic info about a user")]
        [Usage("[user]")]
        [Category(Category.Main)]
        public async Task Whois(CommandContext Context, [RemainingText] DiscordUser usr) {
            DiscordMember user = null;
            if(usr == null)
                usr = Context.User;
            try {
            user = await Context.Guild.GetMemberAsync(usr.Id);
            }
            catch{}

            try {
                // Set up the embed
                DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();
                Embed.WithColor(user.Color);   

                // Basic user info
                if (user.AvatarUrl != null) {
                    Embed.WithThumbnail(user.AvatarUrl);
                    Embed.WithAuthor(user.Username, null, user.AvatarUrl);
                }
                else {
                    Embed.WithThumbnail(user.DefaultAvatarUrl);
                    Embed.WithAuthor(user.Username, null, user.DefaultAvatarUrl);
                }
                string isBot = user.IsBot ? "Yes" : "No";
                string isOwner = user.IsOwner ? "Yes" : "No";
                string hasMFA;
                if (((DiscordUser)user).MfaEnabled == null) hasMFA = "COCK";
                else hasMFA = (bool) (((DiscordUser)user).MfaEnabled) ? "Yes" : "No";

                Embed.AddField("**Information**", $"**Mention:** <@{user.Id.ToString()}>\n**ID:** {user.Id.ToString()}\n**Bot:** {isBot}", true);

                // Embed dates
                Embed.AddField("**Joined**", $"**Discord:** {(int)DateTime.Now.Subtract(user.CreationTimestamp.DateTime).TotalDays} days ago\n**->**<t:{user.CreationTimestamp.ToUnixTimeSeconds()}:f>\n**Guild:** {(int)DateTime.Now.Subtract(user.JoinedAt.DateTime).TotalDays} days ago\n**->**<t:{user.JoinedAt.ToUnixTimeSeconds()}:f>", true);

                // User roles
                string roles = "`@everyone`, ";
                int roleCount = 1;
                foreach (DiscordRole role in user.Roles) {
                    roles += role.Mention + ", ";
                    roleCount += 1;
                }
                roles = roles.Substring(0, roles.Length - 2);
                string guild = user.Guild.Name == null ? "None" : user.Guild.Name;
                Embed.AddField("Guild Specific", $"**Nickname:** {user.Nickname}\n**Roles ({roleCount}): {roles}**\n**Owner:** {isOwner}\n**Hierarchy Position:** {user.Hierarchy.ToString()}");
                Embed.AddField("Guild", guild);
                await Context.ReplyAsync("", Embed.Build());
            }
            catch (Exception ex) {
                await Context.ReplyAsync("Error: " + ex.Message + "\nStack Trace:" + ex.StackTrace);
            }
        }
    }
}
