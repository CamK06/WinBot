using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Discord;
using Discord.Commands;

namespace WinBot.Commands.Fun
{
    public class BoredCommand : ModuleBase<SocketCommandContext>
    {
        [Command("bored")]
        [Summary("Sends a random activity to do when you're bored|")]
        [Priority(Category.Fun)]
        public async Task Bored()
        {
            string json = "";
            // Download the json string from the API
            using (WebClient client = new WebClient())
                json = client.DownloadString("https://www.boredapi.com/api/activity");
            dynamic output = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object

            await ReplyAsync((string)output.activity);
        }
    }
}