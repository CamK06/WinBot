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
    public class ChaosCommand : BaseCommandModule
    {
        [Command("chaos")]
        [Description("Absolute chaos... adds 5 random effects to an image")]
        [Usage("[image] [-scale=(1-5) -layers=(1-3)]")]
        [Category(Category.Images)]
        public async Task Chaos(CommandContext Context, [RemainingText]string input)
        {
            // Handle arguments
            ImageArgs args = ImageCommandParser.ParseArgs(Context, input);
            int seed = new System.Random().Next(1000, 99999);
            if(args.layers > 3)
                args.layers = 3;
            else if(args.scale > 5)
                args.scale = 5;

            // Download the image
            string tempImgFile = TempManager.GetTempFile(seed+"-chaosDL."+args.extension, true);
            new WebClient().DownloadFile(args.url, tempImgFile);

            var msg = await Context.ReplyAsync("Processing...\nThis WILL take a while, it's chaos.");

            // Create C H A O S
            MagickImage img = null;
            MagickImageCollection gif = null;
            string effects = "";
            if(args.extension.ToLower() != "gif") {
                img = new MagickImage(tempImgFile);
                for(int i = 0; i < 5; i++) { 
                    Random r = new Random();
                    ImageEffect effect = (ImageEffect)r.Next(0, 12);
                    effects += $"{effect.ToString()} ";
                    ApplyEffect(img, args, effect);
                }
            }
            else {
                gif = new MagickImageCollection(tempImgFile);
                for(int i = 0; i < 5; i++) {
                    Random r = new Random();
                    ImageEffect effect = (ImageEffect)r.Next(0, 12);
                    effects += $"{effect.ToString()} ";
                    if(effect == ImageEffect.Jpeg) {
                        JpegCommand.DoGifJpegification(gif, args);
                        continue;
                    }
                    foreach(var frame in gif)
                        ApplyEffect((MagickImage)frame, args, effect);
                }
            }
            TempManager.RemoveTempFile(seed+"-chaosDL."+args.extension);

            // Save the image
            MemoryStream imgStream = new MemoryStream();
            if(args.extension.ToLower() != "gif")
                img.Write(imgStream);
            else
                gif.Write(imgStream);
            imgStream.Position = 0;

            // Send the image
            await msg.ModifyAsync("Uploading...\nThis WILL take a while, it's chaos.");
            await Context.Channel.SendFileAsync($"Applied effects: ``{effects}``", imgStream, "chaos."+args.extension);
            await msg.DeleteAsync();
        }

        void ApplyEffect(MagickImage img, ImageArgs args, ImageEffect effect)
        {
            // YANDEV CODE G O
            if(effect == ImageEffect.Explode)
                ExplodeCommand.DoExplode(img, args);
            else if(effect == ImageEffect.Deepfry)
                DeepfryCommand.DoEnhancedFrying(img, args);
            else if(effect == ImageEffect.Haah)
                HaahCommand.DoHaah(img);
            else if(effect == ImageEffect.Hooh)
                HoohCommand.DoHooh(img);
            else if(effect == ImageEffect.Waaw)
                WaawCommand.DoWaaw(img);
            else if(effect == ImageEffect.Woow)
                WoowCommand.DoWoow(img);
            else if(effect == ImageEffect.Wall)
                WallCommand.DoWall(img, args);
            else if(effect == ImageEffect.Magik)
                MagikCommand.DoMagik(img, args);
            else if(effect == ImageEffect.Invert)
                InvertCommand.DoInvert(img);
            else if(effect == ImageEffect.Implode)
                ImplodeCommand.DoImplode(img, args);
            else if(effect == ImageEffect.Flip)
                FlipCommand.DoFlip(img);
            else if(effect == ImageEffect.Flop)
                FlopCommand.DoFlop(img);
            else if(effect == ImageEffect.Jpeg)
                JpegCommand.DoJpegification(img, args);
        }
    }

    enum ImageEffect
    {
        Explode, Deepfry, Haah, Hooh, Waaw, Flop, Woow, Wall, Magik, Invert, Implode, Flip, Jpeg
    }
}