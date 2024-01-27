using System.Drawing;
using System.Drawing.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Util;
using static WinBot.Util.ResourceManager;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class BedCommand : BaseCommandModule
    {
		[Command("bed")]
        [Description("Tell someone to go to bed.")]
        [Usage("[User] [Image: parz, agp, agp2, mehdi]")]
        [Attributes.Category(Category.Fun)]
        public async Task bed(CommandContext Context, string screenname = "", string image = "parz")
        {
            // Randomize the image if no input was given
            if(image == "parz" || string.IsNullOrWhiteSpace(image))
                image = images[new System.Random().Next(0, images.Length)];

			// Select the image with some YanDev code
            string imageFile = GetResourcePath("parz.png", Util.ResourceType.Resource);
            float genX = 365f;
            float bedY = 506f;
            float userY = 615f;
            int fontSize = 70;
            if(image.ToLower() == "agp") {
                imageFile = GetResourcePath("agp.png", Util.ResourceType.Resource);
                genX = 125;
                bedY = 35;
                fontSize = 50;
                userY = 360;
            }
            else if(image.ToLower() == "agp2") {
                imageFile = GetResourcePath("agp2.png", Util.ResourceType.Resource);
                genX = 177.5f;
                bedY = 60;
                fontSize = 50;
                userY = 450;
            }
            else if(image.ToLower() == "mehdi") {
                imageFile = GetResourcePath("mehdi.png", Util.ResourceType.Resource);
                bedY = 100;
                genX = 307.5f; 
                userY = 680f;
            }
            else if(image.ToLower() == "mehdi2") {
                imageFile = GetResourcePath("mehdi2.png", Util.ResourceType.Resource);
                bedY = 100;
                genX = 307.5f; 
                userY = 175;
            }

            // Load the font
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddFontFile(GetResourcePath("impact.ttf", Util.ResourceType.Resource));
			
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
            string imagePath = TempManager.GetTempFile($"bed-{image}-{screenname}-{Context.User.Id}.png", true);
            img.Save(imagePath);
            await Context.Channel.SendFileAsync(imagePath);
            TempManager.RemoveTempFile($"bed-{image}-{screenname}-{Context.User.Id}.png");
		}
		
        static string[] images = { "NONE", "mehdi", "agp", "agp2" };
	}
}