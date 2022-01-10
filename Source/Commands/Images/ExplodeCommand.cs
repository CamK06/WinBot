using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Util;
using WinBot.Commands.Attributes;

using ImageMagick;

namespace WinBot.Commands.Images
{
    public class ExplodeCommand : BaseCommandModule
    {
        [Command("explode")]
        [Description("explode an image")]
        [Usage("[image] [-scale]")]
        [Category(Category.Images)]
        public async Task Explode(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);
            args.scale+=2;

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-explodeDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // E x p l o d e
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                img.Scale(img.Width/2, img.Height/2);
                img.Implode(args.scale*-.3f, PixelInterpolateMethod.Undefined);
                img.Scale(img.Width*2, img.Height*2);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    frame.Scale(frame.Width/2, frame.Height/2);
                    frame.Implode(args.scale*-.3f, PixelInterpolateMethod.Undefined);
                    frame.Scale(frame.Width*2, frame.Height*2);
                }
            }
            TempManager.RemoveTempFile(seed+"-explodeDL."+args.extension);
            if(args.extension.ToLower() != "gif")
                args.extension = img.Format.ToString().ToLower();

            // Save the image
            await msg.ModifyAsync("Saving...\nThis may take a while depending on the image size");
            string finalimgFile = TempManager.GetTempFile(seed+"-explode." + args.extension, true);
            if(args.extension.ToLower() != "gif")
                img.Write(finalimgFile);
            else
                gif.Write(finalimgFile);

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            await Context.Channel.SendFileAsync(finalimgFile);
            await msg.DeleteAsync();
            TempManager.RemoveTempFile(seed+"-explode."+args.extension);
        }
    }
}