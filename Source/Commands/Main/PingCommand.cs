using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace WinBot.Commands.Main
{
    public class PingCommand : BaseCommandModule
    {
        [Command("ping")]
        [Description("Gets the bots latency to Discord")]
        //[Category(Category.Main)]
        public async Task Ping(CommandContext Context)
        {
            await Context.ReplyAsync($"🏓 Pong! **{Bot.client.Ping}ms** from WinBot {Bot.VERSION}");
        }
    }
}