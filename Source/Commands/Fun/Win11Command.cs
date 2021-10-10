using System.Drawing;
using System.Drawing.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class Win11Command : BaseCommandModule
    {
		[Command("win11")]
        [Description("This bot can't run Windows 11. Here's why:")]
        [Usage("[Reasons ('|' separated)]")]
        [Category(Category.Fun)]
        public async Task Win11(CommandContext Context, [RemainingText]string reasonStr)
        {
            // Extract each reason into an array
            if(reasonStr == null)
                return;
            string[] reasons = reasonStr.Split('|');

			// Create the image
            Bitmap img = new Bitmap(Bitmap.FromFile("win11.png"));
			Graphics bmp = Graphics.FromImage(img);

            // Drawing setup
            Font font = new Font("Noto Sans", 13, FontStyle.Regular);
            SolidBrush brush;
			brush = new SolidBrush(System.Drawing.Color.Black);
			
			// Draw the text onto the 
            int cy = 162;
            for(int i = 0; i < reasons.Length; i++) {
                bmp.DrawString(reasons[i], font, brush, 41 + (i > 0 ? -5 : 0), cy);
                cy += 32;
            }
			
			// Save the image to a temporary file
            bmp.Save();
            img.Save("11error.png");

            await Context.Channel.SendFileAsync("11error.png");
		}
		
	}
}