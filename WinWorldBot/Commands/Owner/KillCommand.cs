using System;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class KillCommand : ModuleBase<SocketCommandContext>
    {
        [Command("kill")]
        [Summary("Shut down the bot|")]
        [Priority(Category.Owner)]
        private async Task Kill()
        {
            SocketGuildUser author = Context.Message.Author as SocketGuildUser;
            if(author.Id != Globals.StarID && !author.GuildPermissions.KickMembers) {
                await Context.Message.DeleteAsync();
                return;
            }
            await ReplyAsync("Shutting down...");
            Log.Write("Shutdown triggered by command.");
            Environment.Exit(0);
        }
    }
}