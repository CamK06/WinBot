using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;

using Discord;
using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class WikiHowCommand : ModuleBase<SocketCommandContext>
    {
        [Command("wikihow")]
        [Summary("Sends a random out of context WikiHow image|")]
        [Priority(Category.Fun)]
        private async Task WikiHow()
        {
            var client = new RestClient("https://hargrimm-wikihow-v1.p.rapidapi.com/images?count=1");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-key", Bot.config.WikiHowAPIKey);
            request.AddHeader("x-rapidapi-host", "hargrimm-wikihow-v1.p.rapidapi.com");
            IRestResponse response = client.Execute(request);
            dynamic resp = JsonConvert.DeserializeObject(response.Content);

            // Create and send the embed
            var eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);
            eb.WithTitle("Here's Your Random WikiHow Image!");
            eb.WithImageUrl((string)resp["1"]);
            eb.WithCurrentTimestamp();
            await ReplyAsync("", false, eb.Build());
            await Task.Delay(1);
        }
    }
}