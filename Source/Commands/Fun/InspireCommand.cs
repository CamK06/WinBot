using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using Newtonsoft.Json;

namespace WinBot.Commands.Fun
{
    public class InspireCommand : BaseCommandModule
    {
        [Command("inspire")]
        [Description("Get some much needed AI generated inspiration")]
        [Category(Category.Fun)]
        public async Task Inspire(CommandContext Context)
        {
            string url = new WebClient().DownloadString("https://inspirobot.me/api?generate=true");
            await Context.RespondAsync(url);
        }
    }
}