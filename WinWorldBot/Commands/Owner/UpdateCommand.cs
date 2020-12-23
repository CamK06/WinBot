using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class UpdateCommand : ModuleBase<SocketCommandContext>
    {
        [Command("update")]
        [Summary("Updates the bot with the latest code from git|")]
        [Priority(Category.Owner)]
        private async Task Update()
        {
            SocketGuildUser author = Context.Message.Author as SocketGuildUser;
            if(author.Id != Globals.StarID && !author.GuildPermissions.KickMembers) {
                await Context.Message.DeleteAsync();
                return;
            }

            await ReplyAsync("Updating...");
            Log.Write("Update triggered by " + Context.Message.Author);
            Shell.BashUpdate();
        }
    }
}