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
        [Summary("Gets a random picture of Rory!|")]
        [Priority(Category.Fun)]
        public async Task Rory()
        {
            string json = "";
            // Grab the json string from the API
            using (WebClient client = new WebClient())
                json = client.DownloadString("https://rory.cat/purr");
            dynamic output = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object

            // Send the image in an embed
			EmbedBuilder eb = new EmbedBuilder();
			eb.WithTitle("Rory");
			eb.WithColor(Color.Gold);
			eb.WithFooter($"Rory ID: {output.id}");
			eb.WithImageUrl((string)output.url);
			await ReplyAsync("", false, eb.Build());
        }
    }
}
