using System.Net.Http;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class InspireCommand : BaseCommandModule
    {
        [Command("inspeyer")]
        [Description("gt insperatin")]
        [Category(Category.Fun)]
        public async Task Inspire(CommandContext Context)
        {
            string url = await new HttpClient().GetStringAsync("https://inspirobot.me/api?generate=true");
            await Context.ReplyAsync(url);
        }
    }
}