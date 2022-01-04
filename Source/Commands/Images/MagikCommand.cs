using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Util;
using WinBot.Commands.Attributes;

using ImageMagick;

namespace WinBot.Commands.Images
{
    public class MagikCommand : BaseCommandModule
    {
        [Command("magik")]
        [Description("Really mess up an image")]
        [Usage("[image] [-scale=(1-5) -layers=(1-3)]")]
        [Category(Category.Images)]
        public async Task Magik(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            if(args.layers > 3)
                args.layers = 3;
            else if(args.scale > 5)
                args.scale = 5;

            // Download the image
            string tempImgFile = TempManager.GetTempFile("magikDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            // MAGIKIFY
            MagickImage img = new MagickImage(tempImgFile);
            img.Scale(img.Width/2, img.Height/2);
            TempManager.RemoveTempFile("magikDL."+args.extension);
            for(int i = 0; i < args.layers; i++) {
                img.LiquidRescale((int)(img.Width * 0.5), (int)(img.Height * 0.5), args.scale > 1 ? 0.5*args.scale : 1, 0);
                img.LiquidRescale((int)(img.Width * 1.5), (int)(img.Height * 1.5), args.scale > 1 ? args.scale : 2, 0);
            }
            img.Scale(img.Width*2, img.Height*2);

            // Save the image
            string finalimgFile = TempManager.GetTempFile("magik." + args.extension, true);
            img.Write(finalimgFile);

            // Send the image
            await Context.Channel.SendFileAsync(finalimgFile);
            TempManager.RemoveTempFile("magik."+args.extension);
        }
    }
}