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
        [Summary("You wouldn't download a WinBot!|[verb] [noun]")]
        [Priority(Category.Fun)]
        private async Task Download(string verb, [Remainder] string noun)
        {
            bool noA = false;
            if(noun.Contains("-noa")) noA = true;
            noun.Replace("-noa", "");

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
            SolidBrush brush = new SolidBrush(System.Drawing.Color.White);
            StringFormat drawForm = new StringFormat();

            // My best friend, RNG
            float youWouldnt = new Random().Next(-100, 100);
            float verbX = 135 + new Random().Next(25, 220);
            float nounX = 242.32f + new Random().Next(25, 120);

            // Draw the text onto the image
            bmp.DrawString("you", YOUWOULDNTDOWNLOADACARfont, brush, 165.05f + youWouldnt, 75.0f);
            bmp.DrawString("wouldn't", YOUWOULDNTDOWNLOADACARfont, brush, 465.0f + youWouldnt, 125.0f);
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