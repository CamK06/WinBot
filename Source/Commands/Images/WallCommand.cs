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
                DoWall(img, args);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif)
                    DoWall((MagickImage)frame, args);
            }
            TempManager.RemoveTempFile(seed+"-wallDL."+args.extension);
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
            await Context.Channel.SendFileAsync(imgStream, "wall."+args.extension);
            await msg.DeleteAsync();
        }

        public static void DoWall(MagickImage img, ImageArgs args)
        {
            img.Resize(new MagickGeometry("128"));
            img.VirtualPixelMethod = VirtualPixelMethod.Tile;
            img.MatteColor = MagickColors.None;
            img.BackgroundColor = MagickColors.None;
            img.Resize(new MagickGeometry("512x512!"));
            img.Distort(DistortMethod.Perspective, new double[] { 0,0,57,42,  0,128,63,130,  128,0,140,60,  128,128,140,140 });
        }
    }
}