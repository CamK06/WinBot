using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using Newtonsoft.Json;

namespace WinBot.Commands.Fun
{
    public class RoryCommand : BaseCommandModule
    {
        [Command("rory")]
        [Description("Gets a random picture of rory")]
        [Category(Category.Fun)]
        public async Task Rory(CommandContext Context)
        {
            string json = "";
            // Grab the json string from the API
            using(HttpClient http = new HttpClient())
                json = await http.GetStringAsync("https://rory.cat/purr");
                dynamic output = JsonConvert.DeserializeObject(json);
                
            // Send the image in an embed
			DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
			eb.WithTitle("Rory");
			eb.WithColor(DiscordColor.Gold);
			eb.WithFooter($"Rory ID: {output.id}");
			eb.WithImageUrl((string)output.url);
			await Context.ReplyAsync("", eb.Build());
        }
    }
}