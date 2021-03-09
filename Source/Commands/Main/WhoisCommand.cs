using System;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using WinBot.Util;

namespace WinBot.Commands.Main
{
	public class WhoisCommand : ModuleBase<SocketCommandContext>
	{
		[Command("whois")]
		[Summary("Get information about a user|[User]")]
		[Priority(Category.Main)]
		public async Task Whois([Remainder] SocketGuildUser user)
		{
			if (user == null) user = Context.Message.Author as SocketGuildUser;

			try
			{
				// Set up the embed
				EmbedBuilder Embed = new EmbedBuilder();
				Embed.WithColor(Color.Gold);

				// Basic user info
				if (user != null) Embed.WithAuthor(user);
				if (user.GetAvatarUrl() != null)
					Embed.WithThumbnailUrl(user.GetAvatarUrl());
				else
					Embed.WithThumbnailUrl(user.GetDefaultAvatarUrl());
				Embed.AddField("**ID**", user.Id, false);
				if (!string.IsNullOrWhiteSpace(user.Nickname))
					Embed.AddField("**Nickname**", user.Nickname, true);

				// Embed dates
				Embed.AddField("**Created On**", $"{MiscUtil.FormatDate(user.CreatedAt)} ({(int)DateTime.Now.Subtract(user.CreatedAt.DateTime).TotalDays} days ago)", true);
				if (user.JoinedAt.HasValue) Embed.AddField("**Joined On**", $"{MiscUtil.FormatDate(user.JoinedAt.Value)} ({(int)DateTime.Now.Subtract(user.JoinedAt.Value.DateTime).TotalDays} days ago)", true);

				// User activity
				if (user.Activity != null && !string.IsNullOrEmpty(user.Activity.ToString()))
					Embed.AddField($"**{user.Activity.Type.ToString()}**", user.Activity.ToString(), true);

				await ReplyAsync("", false, Embed.Build());
			}
			catch (Exception ex)
			{
				await ReplyAsync("Error: " + ex.Message + "\nStack Trace:" + ex.StackTrace);
			}
		}
	}
}