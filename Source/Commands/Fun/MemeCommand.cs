 using System.Net;
using System.Threading.Tasks;

using Discord.Commands;
using Discord;

using Newtonsoft.Json;

namespace WinBot.Commands.Fun
{
	public class MemeCommand : ModuleBase<SocketCommandContext>
	{
		[Command("meme")]
		[Summary("Send a random meme|")]
		[Priority(Category.Fun)]
		public async Task Meme()
		{
			// Talk to the meme API
			string json = "";
			using(WebClient client = new WebClient())
				json = client.DownloadString("https://meme-api.herokuapp.com/gimme");
			dynamic output = JsonConvert.DeserializeObject(json);

			// Send the meme
			EmbedBuilder eb = new EmbedBuilder();
			eb.WithColor(Color.Gold);
			eb.WithTitle((string)output.title);
			eb.WithImageUrl((string)output.url);
			await ReplyAsync("", false, eb.Build());
		}
	}
}