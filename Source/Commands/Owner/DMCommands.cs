using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using WinBot.Util;

namespace WinBot.Commands.Owner
{
    public class DMCommands : BaseCommandModule
    {
        [Command("odm")]
        [Category(Category.Owner)]
        public async Task OpenDM(CommandContext Context, DiscordMember user, string showNameString = "false")
        {
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
				throw new Exception("You must be the bot owner to run this command!");

            await Context.Message.DeleteAsync();
            bool names = showNameString.ToLower() == "true";

            DMSystem.Open(user, Context.Channel.Id, names);
        }

        [Command("cdm")]
        [Category(Category.Owner)]
        public async Task CloseDM(CommandContext Context)
        {
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
				throw new Exception("You must be the bot owner to run this command!");

            try {
                DMSystem.Save(DMSystem.chats.FirstOrDefault(x => x.channelId == Context.Channel.Id).user.Id);
                DMSystem.Close(Context.Channel.Id);
                await Context.RespondAsync("Conversation closed!");
            }
            catch {}
        }
    }
}