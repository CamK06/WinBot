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
    public class BillboardCommand : BaseCommandModule
    {
        [Command("billboard")]
        [Description("Do something")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task Billboard(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-billboardDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                img = DoBillboard(img, args);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    DoBillboard((MagickImage)frame, args, true);
                    frame.Resize(768, 768);
                }
            }

            TempManager.RemoveTempFile(seed+"-billboardDL."+args.extension);

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            if(args.extension.ToLower() != "gif")
                img.Write(imgStream);
            else
                gif.Write(imgStream);
            imgStream.Position = 0;

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            await Context.Channel.SendFileAsync(imgStream, "billboard."+args.extension);
            await msg.DeleteAsync();
        }

        MagickImage DoBillboard(MagickImage img, ImageArgs args, bool isGif = false) 
        {
            // Composite args
            float rotation = 0.15f;
            int srcX = 925/2;
            int srcY = 225/2;
            int compX = 850/2;
            int compY = 425/2;
            string imageFile = "billboard1.png";

            // Setup
            if(string.IsNullOrWhiteSpace(args.textArg))
                args.textArg = images[new Random().Next(0, images.Length)];

            if(args.textArg.ToLower() == "1") {
                compX = 925/2;
                compY = 225/2;
                srcX = 850/2;
                srcY = 425/2;
                rotation = -5.5f;
                imageFile = "billboard1.png";
            }
            else if(args.textArg.ToLower() == "2") {
                compX = 120*2;
                compY = 32*2;
                srcX = 360*2;
                srcY = 105*2;
                rotation = 0;
                imageFile = "billboard2.png";
            }
            else if(args.textArg.ToLower() == "3") {
                compX = 398;
                compY = 124;
                srcX = 290;
                srcY = 492;
                rotation = 0;
                imageFile = "billboard3.png";
            }
            MagickImage tv = new MagickImage(ResourceManager.GetResourcePath(imageFile, ResourceType.Resource));
            MagickImage tvClean = new MagickImage(ResourceManager.GetResourcePath(imageFile, ResourceType.Resource));

            // Composite
            img.Resize(new MagickGeometry($"{srcX}x{srcY}!"));
            img.BackgroundColor = MagickColors.Transparent;
            img.Rotate(rotation);
            tv.Alpha(AlphaOption.Remove);
            tv.Composite(img, compX, compY, CompositeOperator.SrcIn);
            if(args.textArg.ToLower() == "1"){
                tv.Composite(tvClean, 0, 0, CompositeOperator.SrcOver, "-background none");
            }
            if(isGif) {
                img.Resize(new MagickGeometry($"{tv.Width}x{tv.Height}!"));
                img.Rotate(rotation*-1);
                img.CopyPixels(tv);
                return null;
            }
            else
                return tv;
        }

        static string[] images = { "1", "2", "3" };
    }
}