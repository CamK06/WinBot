using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using RestSharp;

using Newtonsoft.Json;

namespace WinWorldBot.Commands
{
    public class WeatherCommand : ModuleBase<SocketCommandContext>
    {
        [Command("weather")]
        [Summary("Get the weather of a specific location|[Location]")]
        [Priority(Category.Main)]
        private async Task Ping([Remainder]string location)
        {
            await Context.Channel.TriggerTypingAsync();

            // Pull data from the API
            RestClient client = new RestClient($"http://api.weatherapi.com/v1/forecast.json?key={Bot.config.WeatherAPIKey}&q={location.Replace(" ", "%20")}");
            RestRequest request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if(!response.IsSuccessful) {
                await ReplyAsync("Unable to get the weather for that location, are you sure it exists?");
                return;
            }
            dynamic data = JsonConvert.DeserializeObject(response.Content);

            // Create and send the embed
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithTitle($"Weather for {data.location.name}, {data.location.region}");
            eb.WithColor(Bot.config.embedColour);
            eb.WithFooter($"Last Updated: {data.current.last_updated}");
            eb.WithDescription($"**{data.current.condition.text}**");
            eb.WithThumbnailUrl($"https://{((string)data.current.condition.icon).Replace("//", "")}");
            // F i e l d s
            eb.AddField("Time", $"{data.location.localtime} ({data.location.tz_id})", true);
            eb.AddField("Coordinates", $"**Longitude:** {data.location.lon}, **Latitude:** {data.location.lat}", true);
            eb.AddField("Country", $"{data.location.country}", true);
            eb.AddField("Temperature", $"{data.current.temp_c}°C (Feels like: {data.current.feelslike_c}°C)", true);
            eb.AddField("Humidity", $"{data.current.humidity}%", true);
            eb.AddField("Precipitation", $"{data.current.precip_mm}mm", true);
            eb.AddField("Wind Speed", $"{data.current.wind_kph} km/h with gusts up to {data.current.gust_kph} km/h", true);
            eb.AddField("Wind Direction", $"{data.current.wind_dir}", true);
            
            await ReplyAsync("", false, eb.Build());
        }
    }
}