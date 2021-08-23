using System.Net;
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
        [Command("mcinfo")]
        [Description("Gets information on the Minecraft server (WinWorldMC)")]
        [Category(Category.Main)]
        public async Task McInfo(CommandContext Context)
        {
            await Context.Channel.TriggerTypingAsync();
            
            // Download the server info
            string json = "";
            using(WebClient webClient = new WebClient())
#if !TOFU
                json = webClient.DownloadString("https://api.mcsrvstat.us/2/mc.winworldpc.com:48666");
#else
                json = webClient.DownloadString("https://api.mcsrvstat.us/2/minecraft.cerrogordo.club");
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
                eb.AddField("IP", "mc.winworldpc.com:48666", true);
                eb.AddField("Versions", "1.5.2 -> 1.16.5", true);
#else
                eb.AddField("IP", "minecraft.cerrogordo.club", true);
                eb.AddField("Versions", "1.5.2 -> 1.16.5 & Bedrock", true);
                eb.AddField("Dynmap", "http://minecraft.cerrogordo.club:1865/", true);
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