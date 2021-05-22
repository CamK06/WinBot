using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;
using WinBot.Misc;

namespace WinBot.Commands.Main
{
    public class RankCommand : BaseCommandModule
    {
        [Command("rank")]
        [Description("Get your current level")]
        [Category(Category.Main)]
        public async Task Ping(CommandContext Context)
        {
            await Context.RespondAsync($"You are currently at level {UserData.GetOrCreateUser(Context.User).level} with {UserData.GetOrCreateUser(Context.User).xp}/{((UserData.GetOrCreateUser(Context.User).level+1)*5)*40} to get to level {UserData.GetOrCreateUser(Context.User).level}");
        }
    }
}