using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Owner
{
    public class ChanExecCommand : BaseCommandModule
    {
        [Command("chanexec")]
        [Description("Execute a command in another channel")]
        [Category(Category.Owner)]
        public async Task ChanExec(CommandContext Context, DiscordChannel channel, [RemainingText]string command)
        {
            // Owner check
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
				throw new Exception("You must be the bot owner to run this command!");

            // Channel check
            if(channel == null || channel.Type == DSharpPlus.ChannelType.Voice)
                throw new Exception("Invalid channel");

            // Find the command
            Command realCommand = Bot.commands.FindCommand(command, out var args);
            if(realCommand == null)
                throw new Exception("Invalid command");

            // Execute the command
            CommandContext context = Bot.commands.CreateFakeContext(Context.User, channel, command, ".", realCommand, args);
			await Bot.commands.ExecuteCommandAsync(context);

            // React
            await Context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👍"));
        }
    }
}