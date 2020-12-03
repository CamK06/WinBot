using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class UnblacklistCommand : ModuleBase<SocketCommandContext>
    {
        [Command("unblacklist")]
        [Summary("Remove a user from the blacklist|")]
        [Priority(Category.Owner)]
        private async Task UnBlacklist(SocketUser user)
        {
            if(Context.Message.Author.Id != Globals.StarID) return;
            
            if(Bot.blacklistedUsers.Contains(user.Id))
                Bot.blacklistedUsers.Remove(user.Id);
                
            MiscUtil.SaveBlacklist();
            await ReplyAsync($"Removed {user} from the blacklist");
        }
    }
}