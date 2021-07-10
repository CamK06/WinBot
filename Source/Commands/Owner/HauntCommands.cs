using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Owner
{
    public class HauntCommands : BaseCommandModule
    {
        [Command("hc")]
        [Category(Category.Owner)]
        public async Task HauntChannel(CommandContext Context, DiscordChannel target)
        {
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
				throw new Exception("You must be the bot owner to run this command!");

            await Context.Message.DeleteAsync();
            if(HauntSystem.chats.FirstOrDefault(x => x.host == Context.Channel) != null)
                throw new Exception("A channel is already being haunted from here!");
            HauntSystem.Open(target, Context.Channel);
        }

        [Command("ch")]
        [Category(Category.Owner)]
        public async Task CloseHaunt(CommandContext Context)
        {
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
				throw new Exception("You must be the bot owner to run this command!");

            try {
                HauntSystem.Close(Context.Channel);
                await Context.RespondAsync("Conversation closed!");
            }
            catch {}
        }
    }
}