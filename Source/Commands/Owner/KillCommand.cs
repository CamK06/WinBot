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
            await Context.ReplyAsync(replyGifs[new Random().Next(0, replyGifs.Length)]);
			Log.Information("Shutdown triggered by command");
			//DailyReportSystem.CreateBackup();
            UserData.SaveData();
			Environment.Exit(0);
        }

        static string[] replyGifs = {
            "https://tenor.com/bbEm5.gif",
            "https://tenor.com/Z6a1.gif",
            "https://tenor.com/bjJy4.gif",
            "https://tenor.com/KC31.gif",
            "https://tenor.com/bBZrD.gif",
            "https://tenor.com/bkhTF.gif",
            "https://tenor.com/bM2j7.gif",
            "https://tenor.com/bckT6.gif",
            "https://tenor.com/t8rI.gif",
            "https://tenor.com/t8qR.gif",
            "https://tenor.com/Tm09.gif",
            "https://tenor.com/bzcn5.gif",
            "https://tenor.com/bPRLn.gif",
            "https://tenor.com/8Hw2.gif",
            "https://tenor.com/biuHb.gif",
            "https://tenor.com/biC9R.gif",
            "https://tenor.com/bVtZn.gif"
        };
    }
}