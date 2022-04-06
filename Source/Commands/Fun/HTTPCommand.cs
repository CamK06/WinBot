using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Util;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Images
{
    public class HTTPCommand : BaseCommandModule
    {
        [Command("http")]
        [Description("Sends a very cute HTTP cat.")]
        [Usage("[status code]")]
        [Category(Category.Fun)]
        public async Task HTTP(CommandContext Context, string input = "")
        {
		bool exists = Array.IndexOf(Global.status_codes, input) != -1;
		// The extremely long array of http status codes was to
		// circumvent a bug in which the link would show and the 404 cat
		// would not show. If you'd rather this happen instead of a bunch of
		// bloat feel free to remove it. 0 is valid because there is an image.
		if (string.IsNullOrEmpty(input) || !exists)
		{
			await Context.ReplyAsync("https://http.cat/404");
			//await Context.ReplyAsync("Provide a valid status code.");
			return;
		}

		await Context.ReplyAsync($"http://http.cat/{input}");
        }
    }
}
