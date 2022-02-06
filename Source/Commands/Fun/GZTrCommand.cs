using System.Drawing;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public async Task gztrnocapbrofax(CommandContext Context, [RemainingText]string normalPersonText)
        {
            string output = normalPersonText.Replace("ing", "in");
            foreach(var word in Dicctionary)
                output = output.Replace(word.Key, word.Value);
            output = output.Replace("'", "");
            await Context.ReplyAsync(output);
		}

        public Dictionary<string, string> Dicctionary = new Dictionary<string, string>()
        {
            { "enough", "enf" },
            { "ok", "k" },
            { "okay", "k" },
            { "what", "wut" },
            { "you", "u" },
            { "already", "ady" },
            { "true", "no cap" },
            { "real", "no cap" },
            { "bro", "bru" },
            { "for", "4" },
            { "they", "dey" },
            { "yeah", "yuh" },
            { "dont", "don" },
            { "don't", "dun" },
            { "and", "n" },
            { "because", "bc" },
            { "why", "y" },
            { "about", "bout" },
            { "everyone", "every1" },
            { "pretty", "pre" },
            { "be", "b" },
            { "telling", "tellin" },
            { "one", "1" }
        };
	}
}