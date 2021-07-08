using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using WinBot.Misc;

namespace WinBot.Commands.Owner
{
    public class SudoCommand : BaseCommandModule
    {
        [Command("sudo")]
        [Description("Execute a command as another user")]
        [Category(Category.Owner)]
        public async Task Sudo(CommandContext Context, DiscordUser user, [RemainingText]string command)
        {
            // Owner check
            if(Context.User.Id != Bot.config.ownerId)
				throw new Exception("You must be the bot owner to run this command!");
			
            // Find the command
            Command realCommand = Bot.commands.FindCommand(command, out var args);
            if(realCommand == null)
                throw new Exception("Invalid command");

            // Execute the command
            CommandContext context = Bot.commands.CreateFakeContext(user, Context.Channel, command, ".", realCommand, args);
			await Bot.commands.ExecuteCommandAsync(context);
        }
    }
}