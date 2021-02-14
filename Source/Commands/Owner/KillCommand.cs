using System;
using System.Threading.Tasks;

using Discord.Commands;

using WinBot.Util;

namespace WinBot.Commands.Owner
{
	public class KillCommand : ModuleBase<SocketCommandContext>
	{
		[Command("kill")]
		[Summary("Kills the bot|")]
		[Priority(Category.Owner)]
		public async Task Kill()
		{
			if(Context.User.Id != Bot.config.ownerId)
				return;
			
			await ReplyAsync("Shutting down...");
			Log.Write("Shutdown triggered by command");
			DailyReportSystem.CreateBackup();
			Environment.Exit(0);
		}
	}
}