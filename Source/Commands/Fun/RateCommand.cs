using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace WinBot.Commands.Fun
{
	public class RateCommand : ModuleBase<SocketCommandContext>
	{
		[Command("rate")]
		[Summary("Rate something|[thing to rate]")]
		[Priority(Category.Fun)]
		public async Task Rate([Remainder]string option)
		{
			Random r = new Random();
			EmbedBuilder eb = new EmbedBuilder();
			eb.WithColor(Color.Gold);
			eb.WithTitle($"🤔 I give **{option}** a solid {r.Next(1, 10)}/10");
			await ReplyAsync("", false, eb.Build());
		}
	}
}