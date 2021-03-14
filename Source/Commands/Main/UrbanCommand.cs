using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Miki.UrbanDictionary;

using WinBot.Util;

namespace WinBot.Commands.Main
{
	public class UrbanCommand : ModuleBase<SocketCommandContext>
	{
		[Command("urban")]
		[Summary("Search the urban dictionary|[Query]")]
		[Priority(Category.Main)]
		public async Task Urban([Remainder] string query)
		{
			// Get the definition
			UrbanDictionaryApi api = new UrbanDictionaryApi();
			var definition = await api.SearchTermAsync(query);

			// Error checking
			bool hasExample = true;
			if (definition.List.Count < 1 || string.IsNullOrWhiteSpace(definition.List.First().Definition.Truncate(1024)))
			{
				await ReplyAsync("Error: There are no results for that query.");
				return;
			}
			else if(string.IsNullOrWhiteSpace(definition.List.First().Example.Truncate(1024)))
				hasExample = false;

			// Create an embed
			EmbedBuilder eb = new EmbedBuilder();
			var result = definition.List.First();
			eb.WithTitle($"Urban Dictionary: {query}");
			eb.WithColor(Color.Gold);
			eb.AddField("Definition", result.Definition.Truncate(1024));
			if(hasExample)
				eb.AddField("Examples", result.Example.Truncate(1024));
			eb.WithUrl(result.Permalink);
			eb.WithThumbnailUrl("https://reclaimthenet.org/wp-content/uploads/2020/07/urban-dictionary.png");
			eb.WithFooter("The information above does not represent the views of WinWorld or Starman, obviously :P");

			await ReplyAsync("", false, eb.Build());
		}
	}
}