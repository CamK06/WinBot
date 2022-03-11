using System;
using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.NerdStuff
{
    public class MUFCommand : BaseCommandModule
    {
        [Command("muf")]
        [Description("Sends a map of radio MUF")]
        [Category(Category.NerdStuff)]
        public async Task MUF(CommandContext Context)
        {
            // This code is garbage and barely works... but it does work.
            string svgFile = TempManager.GetTempFile("muf.svg", false);
            string pngOut = TempManager.GetTempFile("muf2.png", true);
            new WebClient().DownloadFile("https://prop.kc2g.com/renders/current/mufd-normal-now.svg", svgFile);
            ImageMagick.MagickImage img = new ImageMagick.MagickImage(svgFile);
            img.Format = ImageMagick.MagickFormat.Png;
            img.Write(pngOut);
            await Context.Channel.SendFileAsync(pngOut);
        }
    }
}
