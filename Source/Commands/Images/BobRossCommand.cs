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
    public class BobRossCommand : BaseCommandModule
    {
        [Command("bobross")]
        [Description("Paint a happy little image")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task Bobross(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-bobDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // C h a d i f y
            MagickImage bob = new MagickImage(ResourceManager.GetResourcePath("bobross.png", ResourceType.Resource));
            MagickImage bobClean = new MagickImage(ResourceManager.GetResourcePath("bobross.png", ResourceType.Resource));
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                img.Resize(new MagickGeometry("360x274!"));
                img.BackgroundColor = MagickColors.Transparent;
                img.Rotate(2.6);
                bob.Composite(img, 5, 55, CompositeOperator.SrcOver, "-background none -rotate -12");
                bob.Composite(bobClean, 0, 0, CompositeOperator.SrcOver, "-background none");
                img = bob;
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    frame.Resize(new MagickGeometry("360x274!"));
                    frame.BackgroundColor = MagickColors.Transparent;
                    frame.Rotate(2.6);
                    bob.Composite(frame, 5, 55, CompositeOperator.SrcOver, "-background none -rotate -12");
                    bob.Composite(bobClean, 0, 0, CompositeOperator.SrcOver, "-background none");
                    frame.Resize(new MagickGeometry($"{bob.Width}x{bob.Height}!"));
                    frame.CopyPixels(bob);
                    frame.Resize(800, 600);
                }
            }

            TempManager.RemoveTempFile(seed+"-bobDL."+args.extension);

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            if(args.extension.ToLower() != "gif")
                img.Write(imgStream);
            else
                gif.Write(imgStream);
            imgStream.Position = 0;

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            await Context.Channel.SendFileAsync(imgStream, "bobross."+args.extension);
            await msg.DeleteAsync();
        }
    }
}