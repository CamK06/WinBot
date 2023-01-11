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
        // This is just a hacky way to avoid having to change ImageArgs scale to a float
        // while still allowing the -scaleup option to exist with decimal increments
        static float scale = 1.0f;

        [Command("magik")]
        [Description("Really mess up an image")]
        [Usage("[image] [-scale=(1-5) -layers=(1-3) -gif -size=25]")]
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
            scale = args.scale;

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-magikDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // MAGIKIFY
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);

                if(string.IsNullOrWhiteSpace(args.textArg))
                    DoMagik(img, args);
                else if(args.textArg.ToLower() == "-gif") {  // We're turning the image into a gif
                    gif = new MagickImageCollection();

                    // Default to 50 frames
                    if(args.size == 1)
                        args.size = 25;
                    else if(args.size > 64)
                        throw new System.Exception("New gif size must not exceed 64 frames!");

                    // Create args.size frames with slightly different magik applied to each
                    for(int i = 0; i < args.size; i++) {

                        // Resize the frame to a percentage based on args.size, this is to provide
                        // a slightly different magik effect for each frame
                        MagickImage frame = new MagickImage(img);
                        frame.Resize(new Percentage(System.Math.Abs((100+args.size/2)-i)));
                        DoMagik(frame, args);

                        // Resize the frame back to its original size and add it to the gif
                        frame.Resize(img.Width, img.Height);
                        gif.Add(frame);
                    }
                }
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                bool scaleup = !string.IsNullOrWhiteSpace(args.textArg) && args.textArg.ToLower() == "-scaleup";
                foreach(var frame in gif) {
                    DoMagik((MagickImage)frame, args);
                    frame.Resize(gif[0].Width, gif[0].Height);
                    if(scaleup)
                        scale+=0.05f;
                }
            }
            TempManager.RemoveTempFile(seed+"-magikDL."+args.extension);

            // Change the extension to gif if we turned an image into a gif
            if(!string.IsNullOrWhiteSpace(args.textArg) && args.textArg.ToLower() == "-gif")
                args.extension = "gif";

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            if(args.extension.ToLower() != "gif")
                img.Write(imgStream);
            else
                gif.Write(imgStream, MagickFormat.Gif);
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
                img.LiquidRescale((int)(img.Width * 0.5), (int)(img.Height * 0.5), scale > 1 ? 0.5*scale : 1, 0);
                img.LiquidRescale((int)(img.Width * 1.5), (int)(img.Height * 1.5), scale > 1 ? scale : 2, 0);
            }
            img.Scale(img.Width*2, img.Height*2);
        }
    }
}