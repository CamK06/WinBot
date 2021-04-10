using System;
using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;

using RestSharp;

using Newtonsoft.Json;

namespace WinBot.Commands.Fun
{
    public class WikiHowCommand : BaseCommandModule
    {
        [Command("wikihow")]
        [Description("Gets a random out of context wikihow image")]
        [Category(Category.Fun)]
        public async Task WikiHow(CommandContext Context)
        {
            var client = new RestClient("https://hargrimm-wikihow-v1.p.rapidapi.com/images?count=1");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-key", Bot.config.wikihowAPIKey);
            request.AddHeader("x-rapidapi-host", "hargrimm-wikihow-v1.p.rapidapi.com");
            IRestResponse response = client.Execute(request);
            dynamic resp = JsonConvert.DeserializeObject(response.Content);

            // Create and send the embed
            var eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithTitle("Here's Your Random WikiHow Image!");
            eb.WithImageUrl((string)resp["1"]);
            eb.WithTimestamp(DateTime.Now);
            await Context.RespondAsync("", eb.Build());
            await Task.Delay(1);
        }
    }
}