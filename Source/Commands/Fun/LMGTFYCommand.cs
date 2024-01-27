using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class LMGTFYCommand : BaseCommandModule
    {
        [Command("lmgtfy")]
        [Description("Lemme Google that for you...")]
        [Usage("[query]")]
        [Attributes.Category(Category.Fun)]
        public async Task LMGTFY(CommandContext Context, [RemainingText]string query = null)
        {
            if(query == null) {
                await Context.ReplyAsync("https://www.google.com/");
                return;
            }

            await Context.ReplyAsync($"https://letmegooglethat.com/?q={System.Web.HttpUtility.UrlEncode(query)}");
        }
    }
}
