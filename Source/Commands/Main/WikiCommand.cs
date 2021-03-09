using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using WikipediaNet;
using WikipediaNet.Objects;

namespace WinBot.Commands.Main
{
	public class WikiCommand : ModuleBase<SocketCommandContext>
	{
		[Command("wiki")]
		[Summary("Search wikipedia|[Query]")]
		[Priority(Category.Main)]
		public async Task Wiki([Remainder]string query)
		{
			Wikipedia wiki = new Wikipedia();
			wiki.Limit = 1;
			QueryResult results = wiki.Search(query);

			if(results.SearchInfo.TotalHits < 1) {
				await ReplyAsync($"Error: There are no results for that query.");
				return;
			}

			await ReplyAsync(results.Search.First().Url.ToString().Replace(" ", "_"));
		}
	}
}