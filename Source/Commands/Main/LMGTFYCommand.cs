using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class PingCommand : BaseCommandModule
    {
        [Command("lmgtfy")]
        [Description("Lemme Google that for you...")]
        [Category(Category.Main)]
        public async Task Ping(CommandContext Context)
        {
            await Context.RespondAsync("https://www.google.com/");
        }
    }
}
