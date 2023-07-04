using System.IO;
using System.Net;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Util;
using WinBot.Commands.Attributes;

using ImageMagick;

namespace WinBot.Commands.Images
{
    public class WideCommand : BaseCommandModule
    {
        [Command("wide")]
        [Description("Make an image W I D E")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task Wide(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-wideDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // I n v e r t
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                DoWide(img, args.scale, args.size);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                bool scaleup = !string.IsNullOrWhiteSpace(args.textArg) && args.textArg.ToLower() == "-scaleup";
                float scale = args.scale;
                if(scaleup)
                    scale = 1;
                foreach(var frame in gif) {
                    DoWide((MagickImage)frame, scale, args.size);
                    if(scaleup)
                        scale += (float)args.scale/(float)gif.Count;
                }
            }
            TempManager.RemoveTempFile(seed+"-wideDL."+args.extension);
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
            await Context.Channel.SendFileAsync(imgStream, "wide."+args.extension);
            await msg.DeleteAsync();
        }

        public static void DoWide(MagickImage img, float scale, int size)
        {
            if(scale <= 1 || scale > 5)
                scale = 3;

            if(size <= 1)
                img.Resize(new MagickGeometry($"{img.Width*scale}x{img.Height}!"));
            else if(size <= 4096)
                img.Resize(new MagickGeometry($"{size}x{img.Height}!"));
            else
                throw new System.Exception("Size must be less than 4096!");
        }
    }
}