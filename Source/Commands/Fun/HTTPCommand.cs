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
        [Category(Category.Fun)]
        public async Task HTTP(CommandContext Context, [RemainingText]string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new Exception("Enter a valid HTTP status code to get a cute cat.");

            //TODO: Figure out how to embed the 404 image that the website gives when an invalid code is entered
            await Context.ReplyAsync($"https://http.cat/{input}");
        }
    }
}
