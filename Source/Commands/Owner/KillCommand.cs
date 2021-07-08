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
				throw new Exception("You must be the bot owner to run this command!");
			
			await Context.RespondAsync("Shutting down...");
			await Bot.logChannel.SendMessageAsync("Shutdown triggered by command");
			DailyReportSystem.CreateBackup();
            UserData.SaveData();
			Environment.Exit(0);
        }
    }
}