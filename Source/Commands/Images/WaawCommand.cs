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
    public class WaawCommand : BaseCommandModule
    {
        [Command("waaw")]
        [Description("W a a w")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task Waaw(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);
            args.scale+=2;

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-waawDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // W a a w
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                DoWaaw(img);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    DoWaaw((MagickImage)frame);
                }
            }
            TempManager.RemoveTempFile(seed+"-waawDL."+args.extension);
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
            await Context.Channel.SendFileAsync(imgStream, "waaw."+args.extension);
            await msg.DeleteAsync();
        }

        public static void DoWaaw(MagickImage image)
        {
            MagickImage left;
            left = (MagickImage)image.Clone();
            
            int width = image.Width/2;
            if(width <= 0)
                width = 1;

            left.Crop(width, image.Height, Gravity.East);
            left.Rotate(180);
            left.Flip();
            
            image.Composite(left);
        }
    }
}