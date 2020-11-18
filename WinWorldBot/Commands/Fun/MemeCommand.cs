using System.Net;
using System.Threading.Tasks;

using Discord.Commands;
using Discord;

using Newtonsoft.Json;

namespace WinWorldBot.Commands
{
    public class MemeCommand : ModuleBase<SocketCommandContext>
    {
        [Command("meme")]
        [Summary("Sends a random meme|")]
        [Priority(Category.Fun)]
        private async Task Meme()
        {
            string json = "";
            using(WebClient client = new WebClient())
                json = client.DownloadString("https://meme-api.herokuapp.com/gimme");
            dynamic output = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithColor(Bot.config.embedColour);
            Embed.WithTitle((string)output.title);
            Embed.WithImageUrl((string)output.url);
            Embed.WithCurrentTimestamp();
            await ReplyAsync("", false, Embed.Build()); ;
        }
    }
}