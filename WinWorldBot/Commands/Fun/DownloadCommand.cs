using System;
using System.Drawing;
using System.Drawing.Text;
using System.Threading.Tasks;

using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class DownloadCommand : ModuleBase<SocketCommandContext>
    {
        [Command("download"), Alias("dl")]
        [Summary("You wouldn't download a WinBot!|[verb] [noun] (arguments: -noa -red -would -will)")]
        [Priority(Category.Fun)]
        private async Task Download(string verb, [Remainder] string noun)
        {
            if(verb.ToLower() == "rick" && noun.ToLower().Contains("roll") || verb.ToLower().Contains("rick") && verb.ToLower().Contains("roll"))
            {
                await Context.Channel.SendFileAsync("rick.gif");
                return;
            }

            // Convoluted options implemented as poorly as possible
            bool noA = false;
            bool red = false;
            bool would = false;
            bool will = false;
            if(noun.Contains("-noa")) noA = true;
            if(noun.Contains("-red")) red = true;
            if(noun.Contains("-would")) would = true;
            if(noun.Contains("-will")) will = true;
            noun = noun.Replace("-noa", "").Replace("-red", "").Replace("-would", "").Replace("-will", "");

            // Shit I shouldn't have to do
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddFontFile("xband.ttf");

            // Create the image
            Bitmap img = new Bitmap(1280, 720);
            Graphics bmp = Graphics.FromImage(img);
            bmp.Clear(System.Drawing.Color.Black);

            // Set up the fonts and drawing stuff
            Font YOUWOULDNTDOWNLOADACARfont = new Font(
                fonts.Families[0].Name,
                175,
                FontStyle.Regular,
                GraphicsUnit.Pixel
            );
            SolidBrush brush;
            if(!red) brush = new SolidBrush(System.Drawing.Color.White);
            else brush = new SolidBrush(System.Drawing.Color.FromArgb(102, 0, 0));
            StringFormat drawForm = new StringFormat();

            // My best friend, RNG
            float youWouldnt = new Random().Next(-100, 100);
            float verbX = 135 + new Random().Next(25, 220);
            float nounX = 242.32f + new Random().Next(25, 120);

            // Draw the text onto the image
            bmp.DrawString("you", YOUWOULDNTDOWNLOADACARfont, brush, 165.05f + youWouldnt, 75.0f);
            if(!would && !will) bmp.DrawString("wouldn't", YOUWOULDNTDOWNLOADACARfont, brush, 465.0f + youWouldnt, 125.0f);
            else if(!will) bmp.DrawString("would", YOUWOULDNTDOWNLOADACARfont, brush, 465.0f + youWouldnt, 125.0f);
            else bmp.DrawString("will", YOUWOULDNTDOWNLOADACARfont, brush, 465.0f + youWouldnt, 125.0f);
            bmp.DrawString(verb, YOUWOULDNTDOWNLOADACARfont, brush, verbX, 325.5f);
            if(!noA) bmp.DrawString("a", YOUWOULDNTDOWNLOADACARfont, brush, verbX + (verb.Length * 95), 300.5f);
            bmp.DrawString(noun, YOUWOULDNTDOWNLOADACARfont, brush, nounX, 500.3f);

            // Save the image to a temporary file, this has to be two separate Save functions because apparently that's what Microsoft thinks is good
            bmp.Save();
            img.Save("download.png");

            await Context.Channel.SendFileAsync("download.png");
        }
    }
}