using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Images
{
    public class HTTPCommand : BaseCommandModule
    {
        [Command("http")]
        [Description("Sends a very cute HTTP cat.")]
        [Usage("[status code]")]
        [Attributes.Category(Category.Fun)]
        public async Task HTTP(CommandContext Context, string input = "")
        {
            bool exists = Array.IndexOf(status_codes, input) != -1;
            // The extremely long array of HTTP status codes was to
            // circumvent a bug in which the link would show and the 404 cat
            // would not show. If you'd rather this happen instead of a bunch of
            // bloat feel free to remove it. 0 is valid because there is an image.
            if (string.IsNullOrEmpty(input) || !exists) {
                await Context.ReplyAsync("https://http.cat/404");
                return;
            }

            await Context.ReplyAsync($"https://http.cat/{input}");
        }
        
        public static string[] status_codes = {
            "0",
            "100", "101", "102",
            "200", "201", "202", "203", "204", "206", "207",
            "300", "301", "302", "303", "304", "305", "307", "308",
            "400", "401", "402", "403", "404", "405", "406", "407", "408", "409", "410", "411", "412", "413", "414", "415", "416", "417", "418", "420", "421", "422", "423", "424", "425", "426", "429", "431", "444", "450", "451", "497", "498", "499",
            "500", "501", "502", "503", "504", "506", "507", "508", "509", "510", "511", "521", "522", "523", "525", "599"
    	};
    }
}
