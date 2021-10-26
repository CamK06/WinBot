using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using WinBot.Misc;

namespace WinBot.Commands.Owner
{
    public class LevelCommands : BaseCommandModule
    {
        [Command("addlvlrole")]
        [Description("Add a level role")]
        [Category(Category.Owner)]
        public async Task Kill(CommandContext Context, DiscordRole role, int level)
        {
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
				throw new Exception("You must be the bot owner to run this command!");
			
            if(level <= 0)
                return;

			Leveling.AddRole(role.Id, level);
            await Context.ReplyAsync("Successfully added level role for level " + level);
        }
    }
}