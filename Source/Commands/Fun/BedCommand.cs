using System.Drawing;
using System.Drawing.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class BedCommand : BaseCommandModule
    {
		[Command("bed")]
        [Description("Tell someone to go to bed.")]
        [Usage("[User] [Image: parz, agp, agp2, mehdi]")]
        [Category(Category.Fun)]
        public async Task bed(CommandContext Context, string screenname = "", string image = "parz")
        {
			// Select the image with some YanDev code
            string imageFile = "parz.png";
            float genX = 365f;
            float bedY = 506f;
            float userY = 615f;
            int fontSize = 70;
            if(image.ToLower() == "agp") {
                imageFile = "agp.png";
                genX = 125;
                bedY = 35;
                fontSize = 50;
                userY = 360;
            }
            else if(image.ToLower() == "agp2") {
                imageFile = "agp2.png";
                genX = 177.5f;
                bedY = 60;
                fontSize = 50;
                userY = 450;
            }
            else if(image.ToLower() == "mehdi") {
                imageFile = "mehdi.png";
                bedY = 100;
                genX = 307.5f; 
                userY = 680f;
            }

            // Load the font
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddFontFile("impact.ttf");
			
			// Create the image
            Bitmap img = new Bitmap(Bitmap.FromFile(imageFile));
			Graphics bmp = Graphics.FromImage(img);
    
			 // Set up the fonts and drawing stuff
            Font IMPACTfont = new Font(
                fonts.Families[0].Name,
                fontSize,
                FontStyle.Regular,
                GraphicsUnit.Pixel
            );

            SolidBrush brush;
			brush = new SolidBrush(System.Drawing.Color.White);
			StringFormat snFormat = new StringFormat();
			snFormat.Alignment = StringAlignment.Center;
			snFormat.LineAlignment = StringAlignment.Center;
			
			// Draw the text onto the image
            bmp.DrawString("GO TO BED", IMPACTfont, brush, genX, bedY, snFormat);
			bmp.DrawString(screenname.ToUpper(), IMPACTfont, brush, genX, userY, snFormat);
			
			// Save the image to a temporary file
            bmp.Save();
            img.Save("bed.png");

            await Context.Channel.SendFileAsync("bed.png");
		}
		
	}
}