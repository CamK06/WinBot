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
    public class GaagCommand : BaseCommandModule
    {
        [Command("gaag")]
        [Description("H")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task Gaag(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);
            args.scale+=2;

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-gaagDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // G a a g. There's probably better ways to do this but meh
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif")
                return;
            else {
                gif = new MagickImageCollection(tempImgFile);
                MagickImageCollection tempGif = new MagickImageCollection(tempImgFile);
                tempGif.Reverse();
                foreach(var frame in tempGif) {
                    gif.Add(frame);
                }
            }
            TempManager.RemoveTempFile(seed+"-gaagDL."+args.extension);

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            gif.Write(imgStream);
            imgStream.Position = 0;

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            await Context.Channel.SendFileAsync(imgStream, "gaag."+args.extension);
            await msg.DeleteAsync();
        }
    }
}