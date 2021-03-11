using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using WinBot.Util;

namespace WinBot.Commands.Owner
{
	public class BlacklistCommand : ModuleBase<SocketCommandContext>
	{
		[Command("blacklist")]
		[Summary("Blacklist or unblacklist someone|[User]")]
		[Priority(Category.Owner)]
		public async Task Blacklist(SocketGuildUser user = null)
		{
			if (Context.User.Id != Bot.config.ownerId && !((SocketGuildUser)Context.User).GuildPermissions.KickMembers)
				return;

			if(user == null)
			{
				// Format blacklisted users into a nice string list
				StringBuilder builder = new StringBuilder();
				foreach(ulong userId in Bot.blacklistedUsers)
				{
					SocketGuildUser bUser = Context.Guild.GetUser(userId);
					if(bUser != null) builder.AppendLine(bUser.Username);
				}

				// Send an embed with the users
				EmbedBuilder eb = new EmbedBuilder();
				eb.WithColor(Color.Gold);
				eb.WithTitle("Blacklisted Users");
				eb.WithDescription($"```\n{builder.ToString()}```");
				await ReplyAsync("", false, eb.Build());
				
				return;
			}

			if (!Bot.blacklistedUsers.Contains(user.Id)) // if the user isn't blacklisted, blacklist them
			{
				Bot.blacklistedUsers.Add(user.Id);
				MiscUtil.SaveBlacklist();
				await ReplyAsync($"Blacklisted {user.Mention}!");
			}
			else										// if they are blacklisted, unblacklist them
			{
				Bot.blacklistedUsers.Remove(user.Id);
				MiscUtil.SaveBlacklist();
				await ReplyAsync($"Unblacklisted {user.Mention}!");
			}
		}
	}
}