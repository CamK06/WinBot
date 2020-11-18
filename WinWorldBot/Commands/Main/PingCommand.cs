using System.Threading.Tasks;

using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class PingCommand : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        private async Task Ping()
        {
            await Context.Channel.TriggerTypingAsync();
            await ReplyAsync($"🏓Pong! **{Bot.client.Latency}ms**");
        }
    }
}