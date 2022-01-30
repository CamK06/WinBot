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
    public class MagikCommand : BaseCommandModule
    {
        [Command("magik")]
        [Description("Really mess up an image")]
        [Usage("[image] [-scale=(1-5) -layers=(1-3)]")]
        [Category(Category.Images)]
        public async Task Magik(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);
            if(args.layers > 3)
                args.layers = 3;
            else if(args.scale > 5)
                args.scale = 5;

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-magikDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // MAGIKIFY
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                DoMagik(img, args);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif)
                    DoMagik((MagickImage)frame, args);
            }
            TempManager.RemoveTempFile(seed+"-magikDL."+args.extension);

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            if(args.extension.ToLower() != "gif")
                img.Write(imgStream);
            else
                gif.Write(imgStream);
            imgStream.Position = 0;

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            await Context.Channel.SendFileAsync(imgStream, "magik."+args.extension);
            await msg.DeleteAsync();
        }

        public static void DoMagik(MagickImage img, ImageArgs args)
        {
            img.Scale(img.Width/2, img.Height/2);
            args.extension = img.Format.ToString().ToLower();
            for(int i = 0; i < args.layers; i++) {
                img.LiquidRescale((int)(img.Width * 0.5), (int)(img.Height * 0.5), args.scale > 1 ? 0.5*args.scale : 1, 0);
                img.LiquidRescale((int)(img.Width * 1.5), (int)(img.Height * 1.5), args.scale > 1 ? args.scale : 2, 0);
            }
            img.Scale(img.Width*2, img.Height*2);
        }
    }
}