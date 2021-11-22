using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using Newtonsoft.Json;

namespace WinBot.Commands.Fun
{
    public class BsodCommand : BaseCommandModule
    {
        [Command("bsod")]
        [Description("It does the thing™")]
        [Category(Category.Fun)]
        public async Task BSOD(CommandContext Context)
        {
            await Context.ReplyAsync("succ is dead, no succ");
        }
    }
}