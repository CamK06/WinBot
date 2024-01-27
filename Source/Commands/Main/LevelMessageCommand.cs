using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;
using WinBot.Misc;

namespace WinBot.Commands.Main
{
    public class LevelMessageCommand : BaseCommandModule
    {
        [Command("togglelevelmsg")]
        [Description("Toggle level messages")]
        [Attributes.Category(Category.Main)]
        public async Task LevelMessage(CommandContext Context)
        {
            User user = UserData.GetOrCreateUser(Context.User);
            user.levelMessages = !user.levelMessages;
            string state = user.levelMessages ? "on" : "off";
            await Context.ReplyAsync($"Your level messages are now {state}!");
        }
    }
}