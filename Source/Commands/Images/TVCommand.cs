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
    public class TVCommand : BaseCommandModule
    {
        [Command("tv")]
        [Description("Watch TV or something idk")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task TV(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-tvDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            MagickImage tv = new MagickImage(ResourceManager.GetResourcePath("tv.png", ResourceType.Resource));
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                img.Resize(new MagickGeometry("260x145!"));
                img.BackgroundColor = MagickColors.Transparent;
                img.Rotate(0.15);
                tv.Composite(img, 166, 45, CompositeOperator.SrcIn);
                img = tv;
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    frame.Resize(new MagickGeometry("260x145!"));
                    frame.BackgroundColor = MagickColors.Transparent;
                    frame.Rotate(0.15);
                    tv.Composite(frame, 166, 45, CompositeOperator.SrcIn);
                    frame.Resize(new MagickGeometry($"{tv.Width}x{tv.Height}!"));
                    frame.CopyPixels(tv);
                    frame.Resize(800, 600);
                }
            }

            TempManager.RemoveTempFile(seed+"-tvDL."+args.extension);

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            if(args.extension.ToLower() != "gif")
                img.Write(imgStream);
            else
                gif.Write(imgStream);
            imgStream.Position = 0;

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            await Context.Channel.SendFileAsync(imgStream, "tv."+args.extension);
            await msg.DeleteAsync();
        }
    }
}