using System.Net;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using RestSharp;

using Newtonsoft.Json;

namespace WinBot.Commands.Main
{
    public class WeatherCommand : ModuleBase<SocketCommandContext>
    {
        [Command("weather")]
        [Summary("Get the weather for a specific location|[location]")]
        [Priority(Category.Main)]
        public async Task Weather([Remainder]string location)
        {
            await Context.Channel.TriggerTypingAsync();

			// Temporary thing for getting weather from my ESP32 lol
			/* Temporarily disabled
			if(location.ToLower() == "starman") {
				WebClient webClient = new WebClient();
				string temp = webClient.DownloadString($"http://{Bot.config.espWeather}/temperature/");
				string humid = webClient.DownloadString($"http://{Bot.config.espWeather}/humidity/");
				webClient.Dispose();
				
				EmbedBuilder embed = new EmbedBuilder();
				embed.WithColor(Color.Gold);
				embed.WithTitle("Local weather for Starman");
				embed.WithFooter("Weather is recorded by a sensor placed in Starmans window, served by an ESP32 server");
				embed.AddField("Temperature", temp + "°C", true);
				embed.AddField("Humidity", humid + "%", true);

				await ReplyAsync("", false, embed.Build());
				return;
			}*/

            // Pull data from the API
            RestClient client = new RestClient($"http://api.weatherapi.com/v1/forecast.json?key={Bot.config.weatherAPIKey}&q={location.Replace(" ", "%20")}");
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
            eb.WithColor(Color.Gold);
            eb.WithFooter($"Last Updated: {data.current.last_updated}");
            eb.WithDescription($"**{data.current.condition.text}**");
            eb.WithThumbnailUrl($"https://{((string)data.current.condition.icon).Replace("//", "")}");
			
            // F i e l d s
            eb.AddField("Time", $"{data.location.localtime} ({data.location.tz_id})", true);
            eb.AddField("Coordinates", $"**Longitude:** {data.location.lon}, **Latitude:** {data.location.lat}", true);
            eb.AddField("Country", $"{data.location.country}", true);
            eb.AddField("Temperature", $"{data.current.temp_c}°C ({data.current.temp_f}°F)", true);
			eb.AddField("Feels Like", $"{data.current.feelslike_c}°C ({data.current.feelslike_f}°F)", true);
            eb.AddField("Humidity", $"{data.current.humidity}%", true);
            eb.AddField("Precipitation", $"{data.current.precip_mm}mm ({data.current.precip_in}in)", true);
            eb.AddField("Wind Speed", $"{data.current.wind_kph} km/h ({data.current.wind_mph} mph) with gusts up to {data.current.gust_kph} km/h ({data.current.gust_mph} mph)", true);
            eb.AddField("Wind Direction", $"{data.current.wind_dir}", true);
            
            await ReplyAsync("", false, eb.Build());
        }
    }
}