using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using WinBot.Misc;

namespace WinBot.Commands.Owner
{
    public class KillCommand : BaseCommandModule
    {
        [Command("kill")]
        [Description("Kills the bot")]
        [Category(Category.Owner)]
        public async Task Kill(CommandContext Context)
        {
            if(Context.User.Id != Bot.config.ownerId)
				return;
			
			await Context.RespondAsync("Shutting down...");
			await Bot.logChannel.SendMessageAsync("Shutdown triggered by command");
			DailyReportSystem.CreateBackup();
			Environment.Exit(0);
        }
    }
}