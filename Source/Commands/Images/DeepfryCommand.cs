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
    public class DeepfryCommand : BaseCommandModule
    {
        [Command("deepfry")]
        [Description("R O A S T... wait, F R Y an image")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task Invert(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-fryDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // D E E P F R Y  I T 
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                DoEnhancedFrying(img, args);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    DoEnhancedFrying((MagickImage)frame, args);
                }
            }
            TempManager.RemoveTempFile(seed+"-fryDL."+args.extension);
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
            await Context.Channel.SendFileAsync(imgStream, "deepfry."+args.extension);
            await msg.DeleteAsync();
        }

        public static void DoEnhancedFrying(MagickImage image, ImageArgs args)
        {
            if(args.scale > 3)
                throw new System.Exception("Scale must not be greater than 3");

            image.Resize(image.Width/(1*2), image.Height/(1*2));
            image.Posterize(2, DitherMethod.Undefined, Channels.RGB);
            image.Resize(image.Width*(1*2), image.Height*(1*2));
        }
    }
}