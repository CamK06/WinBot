using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class HCommand : BaseCommandModule
    {
        [Command("h")]
        [Description("h")]
        [Category(Category.Main)]
        public async Task H(CommandContext Context)
        {
            await Context.ReplyAsync("h");
        }
    }
}