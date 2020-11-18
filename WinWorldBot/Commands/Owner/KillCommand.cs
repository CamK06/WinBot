using System;
using System.Threading.Tasks;

using Discord.Commands;

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
            if(Context.Message.Author.Id != Globals.StarID) return;
            await ReplyAsync("Shutting down...");
            Log.Write("Shutdown triggered by command.");
            Environment.Exit(0);
        }
    }
}