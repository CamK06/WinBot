using System.Drawing;
using System.Drawing.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Util;
using static WinBot.Util.ResourceManager;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class GZTrCommand : BaseCommandModule
    {
		[Command("gztr")]
        [Description("One-way Gen Z translator")]
        [Usage("[Normal Human Text]")]
        [Category(Category.Fun)]
        public async Task bed(CommandContext Context, [RemainingText]string normalPersonText)
        {
            string res = normalPersonText.Replace("e", "").Replace("u", "").Replace("o", "").Replace("a", "");
            await Context.Channel.SendMessageAsync(res);
		}
	}
}