using System.Data;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WinBot.Commands.Attributes;

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
            var res = new DataTable().Compute(expression, "");
            await Context.RespondAsync($"{res}");
        }
    }
}
