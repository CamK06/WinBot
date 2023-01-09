using System.IO;
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
        static float scale = 3;

        [Command("implode")]
        [Description("Implode an image")]
        [Usage("[image] [-scale]")]
        [Category(Category.Images)]
        public async Task Implode(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);
            args.scale+=2;
            scale = args.scale;

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-implodeDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // I m p l o d e
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                DoImplode(img, args);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                bool scaleup = !string.IsNullOrWhiteSpace(args.textArg) && args.textArg.ToLower() == "-scaleup";
                if(scaleup)
                    scale = 0.25f;
                foreach(var frame in gif) {
                    DoImplode((MagickImage)frame, args);
                    if(scaleup)
                        scale += (float)args.scale/(float)gif.Count;
                }
            }
            TempManager.RemoveTempFile(seed+"-implodeDL."+args.extension);
            if(args.extension.ToLower() != "gif")
                args.extension = img.Format.ToString().ToLower();

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            if(args.extension.ToLower() != "gif")
                img.Write(imgStream);
            else
                gif.Write(imgStream);
            imgStream.Position = 0;

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            await Context.Channel.SendFileAsync(imgStream, "implode."+args.extension);
            await msg.DeleteAsync();
        }

        public static void DoImplode(MagickImage img, ImageArgs args)
        {
            img.Scale(img.Width/2, img.Height/2);
            img.Implode(scale*.3f, PixelInterpolateMethod.Undefined);
            img.Scale(img.Width*2, img.Height*2);
        }
    }
}