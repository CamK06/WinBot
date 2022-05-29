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
    public class FOF2Command : BaseCommandModule
    {
        [Command("fof2")]
        [Description("Sends a map of radio MUF")]
        [Category(Category.NerdStuff)]
        public async Task FOF2(CommandContext Context)
        {
            // This code is garbage and barely works... but it does work.
            string svgFile = TempManager.GetTempFile("fof2.svg", false);
            string pngOut = TempManager.GetTempFile("fof2.png", true);
            new WebClient().DownloadFile("https://prop.kc2g.com/renders/current/fof2-normal-now.svg", svgFile);
            ImageMagick.MagickImage img = new ImageMagick.MagickImage(svgFile);
            img.Format = ImageMagick.MagickFormat.Png;
            img.Write(pngOut);
            await Context.Channel.SendFileAsync(pngOut);
        }
    }
}
