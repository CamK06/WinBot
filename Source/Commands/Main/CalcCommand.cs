using System.Data;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WinBot.Commands.Attributes;

using NCalc;

namespace WinBot.Commands.Main
{
    public class CalcCommand : BaseCommandModule
    {
        [Command("calc")]
        [Description("Evaluate an expression")]
        [Usage("[expression]")]
        [Category(Category.Main)]
        public async Task Calc(CommandContext Context, [RemainingText]string expression)
        {
            await Context.RespondAsync($"{new Expression(expression).Evaluate()}");
        }
    }
}
