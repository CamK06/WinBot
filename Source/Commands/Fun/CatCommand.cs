using System;
using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;

using Newtonsoft.Json;

namespace WinBot.Commands.Fun
{
    public class CatCommand : BaseCommandModule
    {
        [Command("cat")]
        [Description("Gets a random cat photo")]
        [Category(Category.Fun)]
        public async Task About(CommandContext Context)
        {
            string json = "";
            // Download the json string from the API
            using (WebClient client = new WebClient())
                json = client.DownloadString($"https://api.thecatapi.com/v1/images/search?api_key={Bot.config.catAPIKey}");
            dynamic output = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object

            // Create and send the embed
            var eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithTitle("Here's Your Random Cat!");
            eb.WithImageUrl((string)output[0].url);
            eb.WithTimestamp(DateTime.Now);
            await Context.RespondAsync("", eb.Build());
            await Task.Delay(1);
        }
    }
}