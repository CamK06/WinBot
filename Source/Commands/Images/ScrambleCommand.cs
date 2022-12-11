using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Util;
using WinBot.Commands.Attributes;

using ImageMagick;

namespace WinBot.Commands.Images
{
    public class ScrambleCommand : BaseCommandModule
    {
        [Command("scramble")]
        [Description("H")]
        [Usage("[gif]")]
        [Category(Category.Images)]
        public async Task Scramble(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);
            args.scale+=2;

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-randomDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // R a n d o m i z e
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif")
                return;
            else {
                gif = new MagickImageCollection(tempImgFile);
                var tmp = gif.OrderBy(x => new System.Random().Next()).ToArray();
                gif = new MagickImageCollection(tmp);
            }
            
            TempManager.RemoveTempFile(seed+"-randomDL."+args.extension);

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