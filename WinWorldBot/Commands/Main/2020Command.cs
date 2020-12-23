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
    public class TwentyTwentyCommand : ModuleBase<SocketCommandContext>
    {
        [Command("2020")]
        [Summary("2020|[2020]")]
        [Priority(Category.Fun)]
        private async Task TwentyTwenty()
        {
            var timeSpan = new DateTime(2021, 1, 1).Subtract(DateTime.Now);
            await ReplyAsync($"There are {(int)timeSpan.TotalDays} days left in 2020. That's {(int)timeSpan.TotalHours} hours, {(int)timeSpan.TotalMinutes} minutes, or {(int)timeSpan.TotalSeconds} seconds.");
        }
    }
}