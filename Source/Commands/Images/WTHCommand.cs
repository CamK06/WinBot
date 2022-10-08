using System;
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
    public class WTHCommand : BaseCommandModule
    {
        [Command("wth")]
        [Description("what h")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task WTH(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-wthDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                img = DoWTH(img, args);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    DoWTH((MagickImage)frame, args, true);
                    frame.Resize(400, 300);
                }
            }

            TempManager.RemoveTempFile(seed+"-wthDL."+args.extension);

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            if(args.extension.ToLower() != "gif")
                img.Write(imgStream);
            else
                gif.Write(imgStream);
            imgStream.Position = 0;

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            await Context.Channel.SendFileAsync(imgStream, "wth."+args.extension);
            await msg.DeleteAsync();
        }

        MagickImage DoWTH(MagickImage img, ImageArgs args, bool isGif = false) 
        {
            // Composite args
            float rotation = 0.15f;
            int srcX = 250;
            int srcY = 283;
            int compX = 63;
            int compY = 52;
            string imageFile = "wth.png";

            // Setup
            MagickImage wth = new MagickImage(ResourceManager.GetResourcePath(imageFile, ResourceType.Resource));
            MagickImage wthClean = new MagickImage(ResourceManager.GetResourcePath(imageFile, ResourceType.Resource));

            // Composite
            img.Resize(new MagickGeometry($"{srcX}x{srcY}!"));
            img.BackgroundColor = MagickColors.Transparent;
            img.Rotate(rotation);
            wth.Alpha(AlphaOption.Remove);
            wth.Composite(img, compX, compY, CompositeOperator.SrcIn);
            if(isGif) {
                img.Resize(new MagickGeometry($"{wth.Width}x{wth.Height}!"));
                img.Rotate(rotation*-1);
                img.CopyPixels(wth);
                return null;
            }
            else
                return wth;
        }
    }
}