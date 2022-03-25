using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;

using Miki.UrbanDictionary;

namespace WinBot.Commands.Fun
{
    public class UrbanCommand : BaseCommandModule
    {
        [Command("urban")]
        [Description("Search the urban dictionary")]
        [Usage("[query]")]
        [Category(Category.Fun)]
        public async Task Urban(CommandContext Context, [RemainingText]string query)
        {
			if(string.IsNullOrWhiteSpace(query)) {
				throw new Exception("You must provide a search query!");
			}

            // Get the definition
			UrbanDictionaryApi api = new UrbanDictionaryApi();
			var definition = await api.SearchTermAsync(query);

			// Error checking
			bool hasExample = true;
			if (definition.List.Count < 1 || string.IsNullOrWhiteSpace(definition.List.First().Definition.Truncate(1024)))
			{
				await Context.ReplyAsync("Error: There are no results for that query.");
				return;
			}
			else if(string.IsNullOrWhiteSpace(definition.List.First().Example.Truncate(1024)))
				hasExample = false;

			// Create an embed
			DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
			var result = definition.List[new Random().Next(0, definition.List.Count)];
			eb.WithTitle($"Urban Dictionary: {query}");
			eb.WithColor(DiscordColor.Gold);
			eb.AddField("Definition", result.Definition.Truncate(1024));
			if(hasExample)
				eb.AddField("Examples", result.Example.Truncate(1024));
			eb.WithUrl(result.Permalink);
			eb.WithThumbnail("https://reclaimthenet.org/wp-content/uploads/2020/07/urban-dictionary.png");
			eb.WithFooter($"The information above does not represent the views of {Context.Guild.Name} or {Bot.client.CurrentApplication.Owners.First().Username}, obviously :P");

			await Context.ReplyAsync("", eb.Build());
        }
    }
}