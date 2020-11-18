using System.Threading.Tasks;

using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class PingCommand : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Gets the bots latency to Discord|")]
        [Priority(Category.Main)]
        private async Task Ping()
        {
            await Context.Channel.TriggerTypingAsync();
            await ReplyAsync($"🏓Pong! **{Bot.client.Latency}ms**");
        }
    }
}