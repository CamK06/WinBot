using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

using WinBot.Misc;

using Serilog;

namespace WinBot.Commands.Owner
{
    public class KillCommand : BaseCommandModule
    {
        [Command("kill")]
        [Description("Kills the bot")]
        [Category(Category.Owner)]
        [RequireOwner]
        public async Task Kill(CommandContext Context)
        {
			await Context.ReplyAsync("Shutting down...");
			Log.Information("Shutdown triggered by command");
			//DailyReportSystem.CreateBackup();
            UserData.SaveData();
			Environment.Exit(0);
        }
    }
}