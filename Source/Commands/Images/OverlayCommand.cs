using System.IO;
using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Util;
using WinBot.Commands.Attributes;
using static WinBot.Util.ResourceManager;

using ImageMagick;

namespace WinBot.Commands.Images
{
    public class OverlayCommand : BaseCommandModule
    {
        [Command("overlay")]
        [Description("Add an overlay to an image")]
        [Usage("[image] [overlay (mehdi, northkorea, usa, ussr, lgbt)]")]
        [Category(Category.Images)]
        public async Task Overlay(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);
            args.scale+=2;

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-overlayDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // Add overlay
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                DoOverlay(img, args);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    DoOverlay((MagickImage)frame, args);
                }
            }
            TempManager.RemoveTempFile(seed+"-overlayDL."+args.extension);
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
            await Context.Channel.SendFileAsync(imgStream, "overlay."+args.extension);
            await msg.DeleteAsync();
        }

        void DoOverlay(MagickImage image, ImageArgs args)
        {
            // Validate the image argument
            if(string.IsNullOrWhiteSpace(args.textArg))
                throw new System.Exception("No overlay provided!");
            args.textArg = args.textArg.Replace("/", "").Replace("\\", "").Replace(".", "");
            if(!ResourceExists(args.textArg + ".png", ResourceType.Resource))
                throw new System.Exception($"Image '{args.textArg}' does not exist!");

            // Load the image
            MagickImage overlayImage = new MagickImage(GetResourcePath(args.textArg + ".png", ResourceType.Resource));
            overlayImage.Resize(new MagickGeometry($"{image.Width}x{image.Height}!"));
            overlayImage.Alpha(AlphaOption.Set);
            overlayImage.BackgroundColor = MagickColors.None;
            overlayImage.Evaluate(Channels.Alpha, EvaluateOperator.Multiply, 0.25f);
            image.Composite(overlayImage, CompositeOperator.SrcAtop);
        }
    }
}