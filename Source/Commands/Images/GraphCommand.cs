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
    public class GraphCommand : BaseCommandModule
    {
        [Command("graph")]
        [Description("LOOK AT THIS PHOTOGRAPH!")]
        [Usage("[image]")]
        [Attributes.Category(Category.Images)]
        public async Task Graph(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-graphDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // C h a d i f y
            MagickImage chad = new MagickImage(ResourceManager.GetResourcePath("chad.png", ResourceType.Resource));
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                img.Resize(new MagickGeometry("565x405!"));
                img.BackgroundColor = MagickColors.Transparent;
                img.Rotate(-12.3);
                chad.Composite(img, 895, 375, CompositeOperator.SrcOver, "-background none -rotate -12");
                img = chad;
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    frame.Resize(new MagickGeometry("565x405!"));
                    frame.BackgroundColor = MagickColors.Transparent;
                    frame.Rotate(-12.3);
                    chad.Composite(frame, 895, 375, CompositeOperator.SrcOver, "-background none -rotate -12");
                    frame.Resize(new MagickGeometry($"{chad.Width}x{chad.Height}!"));
                    frame.CopyPixels(chad);
                    frame.Resize(800, 600);
                }
            }

            TempManager.RemoveTempFile(seed+"-graphDL."+args.extension);

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            if(args.extension.ToLower() != "gif")
                img.Write(imgStream);
            else
                gif.Write(imgStream);
            imgStream.Position = 0;

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            await Context.Channel.SendFileAsync(imgStream, "graf."+args.extension);
            await msg.DeleteAsync();
        }
    }
}