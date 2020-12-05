using System;
using System.Threading.Tasks;

using Discord.WebSocket;
using Discord;
using Discord.Rest;
using Discord.Net;
using Discord.Commands;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class RestartCommand : ModuleBase<SocketCommandContext>
    {
        /*
        [Command("restart")]
        [Summary("Restart the bot|")]
        [Priority(Category.Owner)]
        private async Task Restart()
        {
            if(Context.Message.Author.Id != Globals.StarID) return;
            await ReplyAsync("Restarting...");
            Log.Write("Restart triggered by command.");
            "systemctl restart WWBot".Bash();
        }*/
    }
}