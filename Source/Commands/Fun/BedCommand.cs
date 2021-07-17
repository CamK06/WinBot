#if !TOFU

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;

using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace WinBot.Commands.Fun
{
    public class BedCommand : BaseCommandModule
    {
		
		[Command("bed")]
        [Description("Tell someone to go to bed.")]
        [Usage("[Name of person you want to go to bed]")]
        [Category(Category.Fun)]
        public async Task bed(CommandContext Context, string screenname = null)
        {
			
			 // Shit I shouldn't have to do
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddFontFile("impact.ttf");
			
			// Create the image
            Bitmap parz = new Bitmap(Bitmap.FromFile("parz.png"));
			Graphics bmp = Graphics.FromImage(parz);
			
			
			 // Set up the fonts and drawing stuff
            Font IMPACTfont = new Font(
                fonts.Families[0].Name,
                70,
                FontStyle.Regular,
                GraphicsUnit.Pixel
            );
            SolidBrush brush;
			brush = new SolidBrush(System.Drawing.Color.White);
			StringFormat snFormat = new StringFormat();
			snFormat.Alignment = StringAlignment.Center;
			snFormat.LineAlignment = StringAlignment.Center;
			
			
			// Draw the text onto the image
            bmp.DrawString("GO TO BED", IMPACTfont, brush, 227.0F, 506.0f);
			bmp.DrawString(screenname.ToUpper(), IMPACTfont, brush, 365.0F, 615.0f, snFormat);
			
			// Save the image to a temporary file, this has to be two separate Save functions because apparently that's what Microsoft thinks is good
            bmp.Save();
            parz.Save("bed.png");

            await new DiscordMessageBuilder()
                    .WithFile("bed.png")
                    .SendAsync(Context.Channel);
			
		}
		
		

		
	}
}
#endif
