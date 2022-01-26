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
    public class ResizeCommand : BaseCommandModule
    {
        [Command("resize")]
        [Description("Resize an image")]
        [Usage("[image] [-scale -size]")]
        [Category(Category.Images)]
        public async Task Resize(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);
            if(args.scale == 1 && args.size == 1)
                throw new System.Exception("A scale or size must be provided!");

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-resizeDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // R e s i z e
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                if(args.size != 1) 
                    img.Resize(new MagickGeometry(args.size));
                else if(args.scale != 1)
                    img.Scale(args.scale*img.Width, args.scale*img.Height);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    if(args.size != 1) 
                        frame.Resize(new MagickGeometry(args.size));
                    else if(args.scale != 1)
                        frame.Scale(args.scale*frame.Width, args.scale*frame.Height);
                }
            }

            TempManager.RemoveTempFile(seed+"-resizeDL."+args.extension);

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            if(args.extension.ToLower() != "gif")
                img.Write(imgStream);
            else
                gif.Write(imgStream);
            imgStream.Position = 0;

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            await Context.Channel.SendFileAsync(imgStream, "resized."+args.extension);
            await msg.DeleteAsync();
        }
    }
}