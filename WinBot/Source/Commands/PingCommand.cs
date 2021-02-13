using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

namespace WinBot.Commands
{
	public class PingCommand : ModuleBase<SocketCommandContext>
	{
		[Command("ping")]
		public async Task Ping()
		{
			await ReplyAsync($"🏓 Pong! **{Bot.client.Latency}ms**");
		}
	}
}