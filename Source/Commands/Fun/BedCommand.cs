using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using DiscARC.Commands.Attributes;

namespace DiscARC.Commands.Main
{
    public class BedCommand : BaseCommandModule
    {
        [Command("bed")]
        [Description("Tell floppydisk to go the fuck to bed")]
        [Category(Category.Main)]
        public async Task Ping(CommandContext Context)
        {
            await Context.RespondAsync("<@437970062922612737>");
            await Context.RespondAsync("http://floppydisk.thisproject.space/img/bed.png");
        }
    }
}
