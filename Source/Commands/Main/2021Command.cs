using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class YearCommand : BaseCommandModule
    {
        [Command("2021")]
        [Description("Gets the remaining time in 2021")]
        [Category(Category.Fun)]
        public async Task Ping(CommandContext Context)
        {
            var timeSpan = new DateTime(2022, 1, 1).Subtract(DateTime.Now);
            await Context.ReplyAsync($"There are {(int)timeSpan.TotalDays} days left in 2021. That's {(int)timeSpan.TotalHours} hours, {(int)timeSpan.TotalMinutes} minutes, or {(int)timeSpan.TotalSeconds} seconds.");
        }
    }
}