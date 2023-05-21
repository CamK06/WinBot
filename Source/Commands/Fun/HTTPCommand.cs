using System;
using System.Net;
using System.Net.Http;
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
        [Category(Category.Fun)]
        public async Task HTTP(CommandContext Context, string input = "")
        {
            // This is a cleaner fix to the problem where Discord doesn't embed images when they return a 404

            using (HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.GetAsync($"https://http.cat/{input}");
                if (response.StatusCode == HttpStatusCode.NotFound) {
                    await Context.ReplyAsync($"https://http.cat/404");
                    return;
                }
            }
            
            await Context.ReplyAsync($"https://http.cat/{input}");
        }
    }
}
