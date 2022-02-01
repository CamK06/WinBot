using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class CalcCommand : BaseCommandModule
    {
        [Command("calc")]
        [Description("Do some M A F S")]
        [Category(Category.Main)]
        public async Task Calc(CommandContext Context, [RemainingText]string expression)
        {
            
        }
    }
}
