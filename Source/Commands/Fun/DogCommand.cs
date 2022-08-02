using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using Newtonsoft.Json;

namespace WinBot.Commands.Fun
{
    public class DogCommand : BaseCommandModule
    {
        [Command("dog")]
        [Description("Gets a random picture of a dog")]
        [Category(Category.Fun)]
        public async Task Dog(CommandContext Context)
        {
            string json = "";
	    
	    using (WebClient client = new WebClient())
                json = client.DownloadString("https://dog.ceo/api/breeds/image/random");
            dynamic data = JsonConvert.DeserializeObject(json);

            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
                eb.WithTitle("Random Doggo :dog:");
                eb.WithColor(DiscordColor.Gold);
                eb.WithImageUrl($"{(string)data.message}");

            await Context.ReplyAsync("", eb.Build());
        }
    }
}
