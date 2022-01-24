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
    public class InvertCommand : BaseCommandModule
    {
        [Command("invert")]
        [Description("Invert an image")]
        [Usage("[image]")]
        [Category(Category.Images)]
        public async Task Invert(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-invertDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

            // I n v e r t
            MagickImage img = null;
            MagickImageCollection gif = null;
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                img.Negate(Channels.RGB);
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                foreach(var frame in gif) {
                    frame.Negate(Channels.RGB);
                }
            }
            TempManager.RemoveTempFile(seed+"-invertDL."+args.extension);
            if(args.extension.ToLower() != "gif")
                args.extension = img.Format.ToString().ToLower();

            // Save the image
            await msg.ModifyAsync("Saving...\nThis may take a while depending on the image size");
            string finalimgFile = TempManager.GetTempFile(seed+"-invert." + args.extension, true);
            MemoryStream imgStream = new MemoryStream();
            if(args.extension.ToLower() != "gif")
                img.Write(imgStream);
            else
                gif.Write(imgStream);

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
            DiscordMessageBuilder mb = new DiscordMessageBuilder();
            mb.WithFile("invert." + args.extension, imgStream);
            await mb.SendAsync(Context.Channel);
            //await Context.Channel.SendFileAsync(finalimgFile);
            await msg.DeleteAsync();
            TempManager.RemoveTempFile(seed+"-invert."+args.extension);
        }
    }
}