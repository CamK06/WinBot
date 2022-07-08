using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class YudsCommand : BaseCommandModule
    {
        [Command("yuds")]
        [Description("Yuds")]
        [Category(Category.Fun)]
        public async Task Yuds(CommandContext Context, [RemainingText]string query)
        {
			await Context.ReplyAsync("Yuds");
        }
    }
}