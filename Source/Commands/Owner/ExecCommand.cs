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
        public async Task Exec(CommandContext Context, [RemainingText]string command)
        {
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
                throw new Exception("You must be the bot owner to run this command!");

            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithTitle("Exec");
            eb.WithColor(DiscordColor.Gold);
            eb.AddField("Input", $"```sh\n{command}```");
            eb.AddField("Output", $"```sh\n{command.Bash()}```");
            await Context.RespondAsync("", eb.Build());
        }
    }
}