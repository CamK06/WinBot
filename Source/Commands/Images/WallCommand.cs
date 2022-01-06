using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Util;
using WinBot.Commands.Attributes;

using ImageMagick;

namespace WinBot.Commands.Images
{
    public class WallCommand : BaseCommandModule
    {
        [Command("wall")]
        [Description("Build a great big beautiful wall ~~and make Mexico pay for it~~")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task Wall(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-wallDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // W a l l
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                img.VirtualPixelMethod = VirtualPixelMethod.Tile;
                img.Distort(DistortMethod.Perspective, new double[] { 0,0,57,42,  0,128,63,130,  128,0,140,60,  128,128,140,140 });
                img.Resize(512, 512);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    frame.VirtualPixelMethod = VirtualPixelMethod.Tile;
                    frame.Distort(DistortMethod.Perspective, new double[] { 0,0,57,42,  0,128,63,130,  128,0,140,60,  128,128,140,140 });
                    frame.Resize(512, 512);   
                }
            }
            TempManager.RemoveTempFile(seed+"-wallDL."+args.extension);
            if(args.extension.ToLower() != "gif")
                args.extension = img.Format.ToString().ToLower();

            // Save the image
            string finalimgFile = TempManager.GetTempFile(seed+"-wall." + args.extension, true);
            if(args.extension.ToLower() != "gif")
                img.Write(finalimgFile);
            else
                gif.Write(finalimgFile);

            // Send the image
            await Context.Channel.SendFileAsync(finalimgFile);
            await msg.DeleteAsync();
            TempManager.RemoveTempFile(seed+"-wall."+args.extension);
        }
    }
}