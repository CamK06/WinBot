using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using WinBot.Util;

namespace WinBot.Commands.Owner
{
    public class ExecCommand : BaseCommandModule
    {
        [Command("exec")]
        [Description("Execute a terminal command")]
        [Usage("[command]")]
        [Category(Category.Owner)]
        [RequireOwner]
        public async Task Exec(CommandContext Context, [RemainingText]string command)
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithTitle("Exec");
            eb.WithColor(DiscordColor.Gold);
            eb.AddField("Input", $"```sh\n{command}```");
            eb.AddField("Output", $"```sh\n{command.Bash()}```");
            await Context.ReplyAsync("", eb.Build());
        }
    }
}