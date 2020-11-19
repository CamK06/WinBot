using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Discord;
using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class CatCommand : ModuleBase<SocketCommandContext>
    {
        [Command("cat")]
        [Summary("Sends a random cat photo|")]
        [Priority(Category.Fun)]
        private async Task Cat()
        {
            string json = "";
            // Download the json string from the API
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString($"https://api.thecatapi.com/v1/images/search?api_key={Bot.config.CatAPIKey}");
            }
            dynamic output = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object

            // Create and send the embed
            var eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);
            eb.WithTitle("Here's Your Random Cat!");
            eb.WithImageUrl((string)output[0].url);
            eb.WithCurrentTimestamp();
            await ReplyAsync("", false, eb.Build());
            await Task.Delay(1);
        }
    }
}