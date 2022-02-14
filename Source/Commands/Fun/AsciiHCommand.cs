using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class AsciiHCommand : BaseCommandModule
    {
        [Command("asciih")]
        [Description("h, but ascii")]
        [Category(Category.Main)]
        public async Task AsciiH(CommandContext Context)
        {
            await Context.ReplyAsync("\n``` __    __\n|  |  |  |\n|  |__|  |\n|   __   |\n|  |  |  |\n|__|  |__|\n           \n```");
        }
    }
}