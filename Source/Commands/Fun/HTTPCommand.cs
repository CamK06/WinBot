using System;
using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using Newtonsoft.Json;

namespace WinBot.Commands.Fun
{
    public class HTTPCommand : BaseCommandModule
    {
        [Command("http")]
        [Description("Gets the specified (or random) error code from http.cat")]
        [Category(Category.Fun)]
        public async Task HTTP(CommandContext Context, string code = "")
        {
            string url = "";
			string[] codes = { "100", "101", "102", "200", "201", "202", "203", "204", "206", "207", "300", "301", "302", "303", "304", "305", "307", "308", "400", "401", "402", "403", "404", "405", "406", "407", "408", "409", "410", "411", "412", "413", "414", "415", "416", "417", "418", "420", "421", "422", "423", "424", "425", "426", "429", "431", "444", "450", "451", "497", "498", "499", "500", "501", "502", "503", "504", "506", "507", "508", "509", "510", "511", "521", "523", "525", "599" };
            if (code != "")
				if (Array.IndexOf(codes, code) != -1) url = $"https://http.cat/{code}";
                else throw new System.Exception("Invalid HTTP error code!");
            else {
                int codeLength = codes.Length - 1;
                var rand = new Random();
                int randomCodeIDX = rand.Next(codeLength);
                code = codes[randomCodeIDX];
                url = $"https://http.cat/{code}";
            }
            // Send the image in an embed
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithTitle($"HTTP {code}");
            eb.WithColor(new DiscordColor("#ED4245"));
            eb.WithFooter($"Provided by http.cat");
            eb.WithImageUrl(url);
            await Context.RespondAsync("", eb.Build());
        }
    }
}
