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
    public class WoowCommand : BaseCommandModule
    {
        [Command("woow")]
        [Description("W o o w")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task Woow(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);
            args.scale+=2;

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-woowDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // W o o w
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                DoWoow(img);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    DoWoow((MagickImage)frame);
                }
            }
            TempManager.RemoveTempFile(seed+"-woowDL."+args.extension);
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
            await Context.Channel.SendFileAsync(imgStream, "woow."+args.extension);
            await msg.DeleteAsync();
        }

        public static void DoWoow(MagickImage image)
        {
            MagickImage bottom;
            bottom = (MagickImage)image.Clone();
            
            int height = image.Height/2;
            if(height <= 0)
                height = 1;

            bottom.Crop(image.Width, height, Gravity.North);
            bottom.Flip();
            
            image.Composite(bottom, 0, height);
        }
    }
}