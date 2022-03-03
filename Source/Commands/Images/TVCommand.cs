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

            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                img = DoTV(img, args);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    DoTV((MagickImage)frame, args, true);
                    frame.Resize(400, 300);
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

        MagickImage DoTV(MagickImage img, ImageArgs args, bool isGif = false) 
        {
            // Composite args
            float rotation = 0.15f;
            int srcX = 260;
            int srcY = 145;
            int compX = 166;
            int compY = 45;
            string imageFile = "tv.png";

            // Setup
            if(string.IsNullOrWhiteSpace(args.textArg))
                args.textArg = images[new Random().Next(0, images.Length)];

            if(args.textArg.ToLower() == "celebrate") {
                compX = 196;
                compY = 64;
                srcX = 149;
                srcY = 84;
                rotation = 0;
                imageFile = "tv2.png";
            }
            else if(args.textArg.ToLower() == "remote") {
                compX = 95;
                compY = 35;
                srcX = 459;
                srcY = 276;
                rotation = 0;
                imageFile = "tv3.png";
            }
            else if(args.textArg.ToLower() == "angry") {
                compX = 75;
                compY = 145;
                srcX = 280;
                srcY = 165;
                rotation = 0;
                imageFile = "tv4.png";
            }
            MagickImage tv = new MagickImage(ResourceManager.GetResourcePath(imageFile, ResourceType.Resource));
            MagickImage tvClean = new MagickImage(ResourceManager.GetResourcePath(imageFile, ResourceType.Resource));

            // Composite
            img.Resize(new MagickGeometry($"{srcX}x{srcY}!"));
            img.BackgroundColor = MagickColors.Transparent;
            img.Rotate(rotation);
            tv.Alpha(AlphaOption.Remove);
            tv.Composite(img, compX, compY, CompositeOperator.SrcIn);
            if(args.textArg.ToLower() == "remote" || args.textArg.ToLower() == "angry")
                tv.Composite(tvClean, 0, 0, CompositeOperator.SrcOver, "-background none");
            if(isGif) {
                img.Resize(new MagickGeometry($"{tv.Width}x{tv.Height}!"));
                img.Rotate(rotation*-1);
                img.CopyPixels(tv);
                return null;
            }
            else
                return tv;
        }

        static string[] images = { "celebrate", "remote", "normal", "angry" };
    }
}