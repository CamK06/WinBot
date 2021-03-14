using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Discord;
using Discord.Commands;

namespace WinBot.Commands.Fun
{
    public class RoryCommand : ModuleBase<SocketCommandContext>
    {
        [Command("rory")]
        [Summary("Gets a random picture of Rory!")]
        [Priority(Category.Fun)]
        public async Task Rory()
        {
            string json = "";
            // Grab the json string from the API
            using (WebClient client = new WebClient())
                json = client.DownloadString("https://rory.cat/purr");
            dynamic output = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object

            string roryurl = (string)output.url;

            // Send the image in an embed
            await ReplyAsync("**Rory ID:** " + (string)output.id + "\n**Link:** " + roryurl);
        }
    }
}
