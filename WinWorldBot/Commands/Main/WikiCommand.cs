using System.Threading.Tasks;
using System.Linq;

using Discord;
using Discord.Commands;

using WikiDotNet;

namespace WinWorldBot.Commands
{
    public class WikiCommand : ModuleBase<SocketCommandContext>
    {
        [Command("wiki")]
        [Summary("Search for an article on Wikipedia|")]
        [Priority(Category.Main)]
        private async Task Wiki([Remainder]string query)
        {
            WikiSearchResponse response = WikiSearcher.Search(query);
            WikiSearchResult result = response.Query.SearchResults.FirstOrDefault();

            if(result == null || string.IsNullOrEmpty(result.Title)) {
                await ReplyAsync("Error: No search results.");
                return;
            }

            await ReplyAsync(result.Url("en").Replace(" ", "_"));

            /* TODO: Find better wikipedia library or make own so embeds can work (previews and images are broken)
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithTitle(result.Title);
            eb.WithUrl(result.Url("en").Replace(" ", "_"));
            eb.WithDescription(result.Preview);
            eb.WithColor(Bot.config.embedColour);
            eb.WithFooter($"Last edited on: {result.LastEdited.ToShortDateString()}");
            await ReplyAsync("", false, eb.Build());*/
        }
    }
}