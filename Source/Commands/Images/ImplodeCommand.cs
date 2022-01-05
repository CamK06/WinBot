using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Util;
using WinBot.Commands.Attributes;

using ImageMagick;

namespace WinBot.Commands.Images
{
    public class ImplodeCommand : BaseCommandModule
    {
        [Command("implode")]
        [Description("Implode an image")]
        [Usage("[image] [-scale]")]
        [Category(Category.Images)]
        public async Task Implode(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);

            // Download the image
            string tempImgFile = TempManager.GetTempFile("implodeDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // I m p l o d e
            MagickImage img = new MagickImage(tempImgFile);
            img.Scale(img.Width/2, img.Height/2);
            TempManager.RemoveTempFile("implodeDL."+args.extension);
            args.extension = img.Format.ToString().ToLower();
            img.Implode(args.scale*.3f, PixelInterpolateMethod.Undefined);
            img.Scale(img.Width*2, img.Height*2);

            // Save the image
            string finalimgFile = TempManager.GetTempFile("implode." + args.extension, true);
            img.Write(finalimgFile);

            // Send the image
            await Context.Channel.SendFileAsync(finalimgFile);
            await msg.DeleteAsync();
            TempManager.RemoveTempFile("implode."+args.extension);
        }
    }
}