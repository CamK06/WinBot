using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class BlacklistCommand : ModuleBase<SocketCommandContext>
    {
        [Command("blacklist")]
        [Summary("Blacklist a user|")]
        [Priority(Category.Owner)]
        private async Task Blacklist(SocketGuildUser user)
        {
            SocketGuildUser author = Context.Message.Author as SocketGuildUser;
            if(author.Id != Globals.StarID && !author.GuildPermissions.KickMembers) return;
            
            Bot.blacklistedUsers.Add(user.Id);
            MiscUtil.SaveBlacklist();
            
            await ReplyAsync($"Blacklisted {user}");
        }
    }
}