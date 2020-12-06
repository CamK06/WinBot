using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class BlacklistedUsersCommand : ModuleBase<SocketCommandContext>
    {
        [Command("blacklistusers")]
        [Summary("Shows all blacklisted users|")]
        [Priority(Category.Owner)]
        private async Task Blacklist()
        {
            if(Context.Message.Author.Id != Globals.StarID) return;
            
            // Create a string list of blacklisted users
            StringBuilder builder = new StringBuilder();
            foreach(ulong userId in Bot.blacklistedUsers) {
                SocketGuildUser user = Context.Guild.GetUser(userId);
                builder.AppendLine(user.ToString());
            }
            
            // Create an embed
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithTitle("Blacklisted Users");
            eb.WithColor(Bot.config.embedColour);
            eb.WithDescription($"```\n{builder.ToString()}```");

            // Send the embed
            await ReplyAsync("", false, eb.Build());
        }
    }
}