using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using Newtonsoft.Json;

namespace WinBot.Commands.Fun
{
    public class CatCommand : BaseCommandModule
    {
        [Command("cat")]
        [Description("Gets a random picture of a cat")]
        [Category(Category.Fun)]
        public async Task Cat(CommandContext Context)
        {
            string json = "";
            // Grab the json string from the API
            using (WebClient client = new WebClient())
                json = client.DownloadString("https://cataas.com/cat?json=true");
            dynamic output = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object

            // Send the image in an embed
			DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
			eb.WithTitle("Random cat");
			eb.WithColor(DiscordColor.Gold);
			eb.WithFooter($"ID: {output.id}");
			eb.WithImageUrl($"https://cataas.com{(string)output.url}");
			await Context.ReplyAsync("", eb.Build());
        }
    }
}