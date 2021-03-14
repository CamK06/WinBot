using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Humanizer;

namespace WinBot.Commands.Main
{
	public class AboutCommand : ModuleBase<SocketCommandContext>
	{
		[Command("about")]
		[Summary("Get info on the bot and server|")]
		[Priority(Category.Main)]
		public async Task About()
		{
			EmbedBuilder eb = new EmbedBuilder();
			eb.WithTitle("WinBot");
			eb.WithColor(Color.Gold);
			eb.AddField("Author", "Starman0620#8456", true);
			eb.AddField("Contributors", "floppydisk#0590", true);
			eb.AddField("Language", "C#", true);
			eb.AddField("Library", "Discord.NET 2.3.0", true);
			eb.AddField("Member Count", Context.Guild.MemberCount, true);
			eb.AddField("Uptime", TimeSpanHumanizeExtensions.Humanize(DateTime.Now.Subtract(Bot.startedAt)), true);
			eb.WithThumbnailUrl(Bot.client.CurrentUser.GetAvatarUrl());
			await ReplyAsync("", false, eb.Build());
		}
	}
}