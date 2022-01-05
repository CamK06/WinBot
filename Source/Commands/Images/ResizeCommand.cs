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
            if(args.scale == 1 && args.size == 1)
                throw new System.Exception("A scale or size must be provided!");

            // Download the image
            string tempImgFile = TempManager.GetTempFile("resizeDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // R e s i z e
            MagickImage img = new MagickImage(tempImgFile);
            TempManager.RemoveTempFile("resizeDL."+args.extension);
            args.extension = img.Format.ToString().ToLower();
            if(args.size != 1) 
                img.Resize(new MagickGeometry(args.size));
            else if(args.scale != 1)
                img.Scale(args.scale*img.Width, args.scale*img.Height);

            // Save the image
            string finalimgFile = TempManager.GetTempFile("resize." + args.extension, true);
            img.Write(finalimgFile);

            // Send the image
            await Context.Channel.SendFileAsync(finalimgFile);
            await msg.DeleteAsync();
            TempManager.RemoveTempFile("resize."+args.extension);
        }
    }
}