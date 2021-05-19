#if TOFU
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main.Tofu
{
    public class ReportCommand : BaseCommandModule
    {
        [Command("report")]
        [Description("Report something to staff")]
        [Usage("[Issue]")]
        [Category(Category.Main)]
        public async Task Report(CommandContext Context, [RemainingText]string issue)
        {
            await Context.Message.DeleteAsync();
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithTitle("Report");
            eb.WithColor(DiscordColor.Gold);
            eb.AddField("Submitted by", Context.User.Username, true);
            eb.AddField("Issue", issue, true);
            eb.WithTimestamp(DateTime.Now);
            await Bot.staffChannel.SendMessageAsync("", eb.Build());
            await ((DiscordMember)Context.User).SendMessageAsync("Your report has been sent. Thank you for helping to keep the Cerro Gordo community safe!");
        }
    }
}
#endif