using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace WinBot.Commands
{
	public class HelpCommand : ModuleBase<SocketCommandContext>
	{
		[Command("help")]
		public async Task Help()
		{
			// Embed setup
			EmbedBuilder helpEmbed = new EmbedBuilder();
			helpEmbed.WithTitle("WinBot Commands");
			helpEmbed.WithColor(Color.Gold);

			// Embed contents
			helpEmbed.AddField("**Main**", "help | ping", false);

			await ReplyAsync("", false, helpEmbed.Build());
		}
	}
}