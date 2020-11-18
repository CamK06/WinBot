using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Discord;
using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class DogCommand : ModuleBase<SocketCommandContext>
    {
        [Command("dog")]
        [Summary("Sends a random dog photo|")]
        [Priority(Category.Fun)]
        private async Task Dog()
        {
            string json = "";
            // Download the json string from the API
            returnPoint:
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString("https://random.dog/woof.json");
            }
            dynamic output = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object
            if (((string)output.url).Contains(".mp4")) goto returnPoint;

            // Create and send the embed
            var eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);
            eb.WithTitle("Here's Your Random Dog!");
            eb.WithImageUrl((string)output.url);
            eb.WithCurrentTimestamp();
            await ReplyAsync("", false, eb.Build());
        }
    }
}