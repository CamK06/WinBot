using System.Net.Http;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using Newtonsoft.Json;

namespace WinBot.Commands.Main
{
    public class McInfoCommand : BaseCommandModule
    {
        [Command("mcin")]
        [Description("Mincfr inf")]
        [Category(Category.Main)]
        public async Task McInfo(CommandContext Context)
        {
            await Context.Channel.TriggerTypingAsync();

            // Download the server info
            string json = "";
            using(HttpClient http = new HttpClient())
#if !TOFU
                json = await http.GetStringAsync("https://api.mcsrvstat.us/2/comserv.winworldpc.com");
#else
                json = await http.GetStringAsync("https://api.mcsrvstat.us/2/cgmc.nick99nack.com");
#endif
            dynamic serverInfo = JsonConvert.DeserializeObject(json);
            
            // Format the info in an embed
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            
			// Set up the embed
            if((bool)serverInfo.online) {
                eb.WithThumbnail(Context.Guild.IconUrl);
                eb.WithTitle((string)serverInfo.motd.clean[0]);
#if !TOFU
                eb.AddField("Address", "comserv.winworldpc.com", true);
                eb.AddField("Versions", "1.5.2 -> 1.16.5", true);
		eb.AddField("Dynmap", "http://comserv.winworldpc.com:8123/", true);
#else
                eb.AddField("Address", "cgmc.nick99nack.com", true);
                eb.AddField("Versions", "1.5.2 -> 1.16.5 & Bedrock", true);
                eb.AddField("Dynmap", "http://cgmc.nick99nack.com:8123/", true);
#endif
                eb.AddField("Online?", ((bool)serverInfo.online) ? "Yes" : "No", true);
                eb.AddField("Users Count", $"{(int)serverInfo.players.online}/{(int)serverInfo.players.max}", true);
                if((int)serverInfo.players.online > 0) {
					eb.AddField("Users", $"{string.Join('\n', serverInfo.players.list)}", true);
				}
                eb.AddField("Supports Cracked Accounts?", "No. It never will, just buy the game or stop asking.", true);
            }
            else {
                eb.WithTitle("Server is Offline!");
                eb.WithColor(DiscordColor.Red);
            }

            await Context.ReplyAsync("", eb.Build());
        }
    }
}
