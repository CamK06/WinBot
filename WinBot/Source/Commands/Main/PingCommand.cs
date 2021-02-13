using System.Threading.Tasks;

using Discord.Commands;

namespace WinBot.Commands.Main
{
	public class PingCommand : ModuleBase<SocketCommandContext>
	{
		[Command("ping")]
		[Summary("Gets the bots latency with Discord|")]
		[Priority(Category.Main)]
		public async Task Ping()
		{
			await ReplyAsync($"🏓 Pong! **{Bot.client.Latency}ms**");
		}
	}
}