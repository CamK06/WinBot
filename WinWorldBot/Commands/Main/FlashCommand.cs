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
    public class FlashCommand : ModuleBase<SocketCommandContext>
    {
        [Command("flash")]
        [Summary("flash|[flash]")]
        [Priority(Category.Fun)]
        private async Task TwentyTwenty()
        {
            var timeSpan = new DateTime(2021, 1, 1, 0, 0, 0).Subtract(DateTime.Now);
            await ReplyAsync($"Flash officially dies in {timeSpan.Humanize()}. RIP");
        }
    }
}