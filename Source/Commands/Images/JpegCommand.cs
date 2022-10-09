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
    public class JpegCommand : BaseCommandModule
    {
        [Command("jpeg")]
        [Description("J P E G I F Y an image")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task Jpeg(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-jpegDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // D E E P F R Y  I T 
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                DoJpegification(img, args);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                DoGifJpegification(gif, args);
            }
            TempManager.RemoveTempFile(seed+"-jpegDL."+args.extension);
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
            await Context.Channel.SendFileAsync(imgStream, "jpeg."+args.extension);
            await msg.DeleteAsync();
        }

        public static void DoJpegification(MagickImage image, ImageArgs args)
        {
            if(args.scale > 3)
                throw new System.Exception("Scale must not be greater than 3");
            
            MagickFormat originalFormat = image.Format;
            image.Format = MagickFormat.Jpeg;
            image.Quality = args.scale;
            
            // Temporarily save the image to memory
            MemoryStream stream = new MemoryStream();
            image.Write(stream);
            stream.Position = 0;

            // Load the image back in
            image = new MagickImage(stream);
            image.Format = originalFormat;
        }

        public static void DoGifJpegification(MagickImageCollection gif, ImageArgs args)
        {
            if(args.scale > 3)
                throw new System.Exception("Scale must not be greater than 3");
            
            foreach(var frame in gif) {
                IMagickImage<ushort> newFrame = frame.Clone();
                newFrame.Format = MagickFormat.Jpeg;
                newFrame.Quality = args.scale;
                
                // Temporarily save the image to memory
                MemoryStream stream = new MemoryStream();
                newFrame.Write(stream);
                stream.Position = 0;

                // Load the image back in
                newFrame = new MagickImage(stream);
                newFrame.Format = frame.Format;

                frame.CopyPixels(newFrame);
            }
        }
    }
}