using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;


namespace WinBot.Commands.Staff
{
    public class ReactPurgeCommand : BaseCommandModule
    {
        [Command("reactpurge")]
        [Category(Category.Owner)]
        [Usage("h")]
        [Description("h")]
        public async Task ReactPurge(CommandContext Context, int count = 0)
        {
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
				throw new Exception("You must be the bot owner to run this command!");

            await Context.Message.DeleteAsync();

            // Delete the messages
            var messages = await Context.Channel.GetMessagesAsync(count+1);
            foreach(var message in messages) {
                if(message.Reactions.Count > 0)
                    await message.DeleteAllReactionsAsync();
                await Task.Delay(70);
            }
        }
    }
}