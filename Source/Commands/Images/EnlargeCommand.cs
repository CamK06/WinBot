using System.Net;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Util;
using WinBot.Commands.Attributes;

using ImageMagick;

namespace WinBot.Commands.Images
{
    public class EnlargeCommand : BaseCommandModule
    {
        [Command("enlarge")]
        [Aliases("e")]
        [Description("Get the raw image of an emote")]
        [Category(Category.Images)]
        public async Task Enlarge(CommandContext Context, string emoteStr = null)
        {
            // Parse the emote string
            if(emoteStr == null)
                throw new System.Exception("A guild emote to enlarge must be provided!");
            emoteStr = emoteStr.Split(":").LastOrDefault().Replace(">", "");
            ulong.TryParse(emoteStr, out ulong emoteID);

            // Parse the emote
            DiscordGuildEmoji.TryFromGuildEmote(Bot.client, emoteID, out DiscordEmoji emote);
            if(emote == null)
                throw new System.Exception("The provided emote is invalid! It *must* be a guild emote");

            // Enlarge
            int seed = new System.Random().Next(1000, 99999);
            string emoteFile = TempManager.GetTempFile($"{seed}-emote.png");
            new WebClient().DownloadFile(emote.Url, emoteFile);
            MagickImage image = new MagickImage(emoteFile);
            image.Resize(new MagickGeometry("512x512"));
            image.Write(emoteFile);

            // Send the image
            await Context.Channel.SendFileAsync(emoteFile);
            TempManager.RemoveTempFile($"{seed}-emote.png");
        }
    }
}
