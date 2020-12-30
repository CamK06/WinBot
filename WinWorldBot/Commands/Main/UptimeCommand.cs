using System;
using System.Threading.Tasks;

using Discord.WebSocket;
using Discord;
using Discord.Rest;
using Discord.Net;
using Discord.Commands;

using Humanizer;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class UptimeCommand : ModuleBase<SocketCommandContext>
    {
        [Command("uptime")]
        [Summary("Shows the uptime of the bot|")]
        [Priority(Category.Main)]
        private async Task Uptime()
        {
            TimeSpan elapsed = DateTime.Now.Subtract(Bot.startTime);
            await ReplyAsync($"The bot has been online for {elapsed.Humanize(3)}");
        }
    }
}