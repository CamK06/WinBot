using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using WinBot.Misc;

namespace WinBot.Commands.Owner
{
    public class WhitelistCommand : BaseCommandModule
    {
        [Command("whitelist")]
        [Description("Whitelist a user to use owner commands temporarily")]
        [Usage("[user]")]
        [Category(Category.Owner)]
        public async Task Whitelist(CommandContext Context, DiscordUser user)
        {
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
				throw new Exception("You must be the bot owner to run this command!");
			
			Bot.whitelistedUsers.Add(user.Id);
            await Context.RespondAsync("Whitelisted " + user.Mention);
        }
    }
}