#if !TOFU

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
        [Usage("[Name of person you want to go to bed]")]
        [Category(Category.Fun)]
        public async Task bed(CommandContext Context, string screenname = "")
        {
			
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
			
			// Save the image to a temporary file
            bmp.Save();
            parz.Save("bed.png");

            await Context.Channel.SendFileAsync("bed.png");
		}
		
	}
}
#endif