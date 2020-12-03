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
        private async Task Blacklist(SocketUser user)
        {
            if(Context.Message.Author.Id != Globals.StarID) return;
            
            Bot.blacklistedUsers.Add(user.Id);
            MiscUtil.SaveBlacklist();
            
            await ReplyAsync($"Blacklisted {user}");
        }
    }
}