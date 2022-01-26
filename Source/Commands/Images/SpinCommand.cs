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
    public class SpinCommand : BaseCommandModule
    {
        [Command("spin")]
        [Description("Spin an image")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task Spin(CommandContext Context, [RemainingText]string input)
        {
            throw new System.Exception("This command is temporarily disabled.");

            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);
            if(args.layers > 3)
                args.layers = 3;
            else if(args.scale > 5)
                args.scale = 5;

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-spinDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // S p i n
            MagickImage img = new MagickImage(tempImgFile);
            MagickImageCollection gif = DoSpin(img, args);
            TempManager.RemoveTempFile(seed+"-spinDL."+args.extension);

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            gif.Write(imgStream);
            imgStream.Position = 0;

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            await Context.Channel.SendFileAsync(imgStream, "spin."+args.extension);
            await msg.DeleteAsync();
        }

        MagickImageCollection DoSpin(MagickImage img, ImageArgs args)
        {
            // Setup
            MagickImageCollection gifOut = new MagickImageCollection();
            MagickImage mask = new MagickImage(ResourceManager.GetResourcePath("circleMask.png", ResourceType.Resource));
            img.Alpha(AlphaOption.On);
            img.BackgroundColor = MagickColors.Transparent;

            // Scale & circularize
            img.Resize(new MagickGeometry("256x256!"));
            mask.Resize(new MagickGeometry($"{img.Width}x{img.Height}!"));
            img.Composite(mask, Channels.Opacity);
            
            // Rotate
            gifOut.Add(img);
            for(int i = 0; i <= 340; i += 20) {
                MagickImage newFrame = (MagickImage)img.Clone();
                newFrame.Rotate(i);
                newFrame.Crop(new MagickGeometry("256x256+0+0!"));
                gifOut.Add(newFrame);
            }
            foreach(var frame in gifOut) {
                frame.GifDisposeMethod = GifDisposeMethod.Previous;
                frame.Composite(mask, Channels.Alpha);
                frame.AnimationDelay = 5;
            }
            gifOut.Deconstruct();
            gifOut.RePage();
            
            return gifOut;
        }
    }
}