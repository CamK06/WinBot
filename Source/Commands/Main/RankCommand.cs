using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using System.Drawing.Text;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;
using static WinBot.Util.ResourceManager;
using WinBot.Misc;
using WinBot.Util;
using System.Text.RegularExpressions;

namespace WinBot.Commands.Main
{
    public class RankCommand : BaseCommandModule
    {
        [Command("rank")]
        [Description("Get your current level")]
        [Usage("[user]")]
        [Category(Category.Main)]
        public async Task Rank(CommandContext Context, [RemainingText]DiscordUser dUser = null)
        {
            if(dUser == null)
                dUser = Context.User;

            // Get info
            User user = UserData.GetOrCreateUser(dUser);
            float neededXP = ((user.level+1)*5)*40;

            // Fetch the user avater
            string url = dUser.GetAvatarUrl(DSharpPlus.ImageFormat.Jpeg, 256);
            Stream avatarStream = await new System.Net.Http.HttpClient().GetStreamAsync(url);
            Bitmap avatar = new Bitmap(Image.FromStream(avatarStream), new Size(230, 230));
            avatarStream.Close();

            // Set up text drawing
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddFontFile(GetResourcePath("Roboto-Regular.ttf", ResourceType.Resource));
            SolidBrush brush = new SolidBrush(Color.White);
            StringFormat drawForm = new StringFormat();
            Font roboto = new Font(fonts.Families[0].Name, 50, FontStyle.Regular, GraphicsUnit.Pixel);
            Font robotoSmall = new Font(fonts.Families[0].Name, 35, FontStyle.Regular, GraphicsUnit.Pixel);

            // Calculate offsets
            SizeF usernameSize = MiscUtil.MeasureString(Regex.Replace(dUser.Username, @"\p{Cs}", ""), roboto);
            SizeF levelSize = MiscUtil.MeasureString("LEVEL", robotoSmall);
            SizeF levelNumSize = MiscUtil.MeasureString($"{user.level}", roboto);
            SizeF xpSize = MiscUtil.MeasureString($"{MiscUtil.FormatNumber((int)user.xp)}/{MiscUtil.FormatNumber((int)neededXP)}", robotoSmall);
            int progressBar = (int)((user.xp/neededXP)*616);

            // Image creation:

            // Setup
            Bitmap bmp = new Bitmap(934, 282);
            Graphics img = Graphics.FromImage(bmp);
            img.Clear(Color.FromArgb(35, 39, 42));
            
            // Graphics
            img.DrawImage(avatar, new Point(26, 26)); // 26 in on x and y with a 230x230 avatar allows for all 3 sides to be evenly spaced.
            brush.Color = Color.FromArgb(72, 75, 78);
            img.FillPath(brush, MiscUtil.RoundedRect(new Rectangle(292, 200, 616, 50), 24));
            brush.Color = MiscUtil.GetAverageColor(avatar);
            if(progressBar>=40)
                img.FillPath(brush, MiscUtil.RoundedRect(new Rectangle(292, 200, progressBar, 50), 24));

            // Strings
            brush.Color = Color.White;
            img.DrawString(Regex.Replace(dUser.Username, @"\p{Cs}", ""), roboto, brush, new PointF(292, 140));
            img.DrawString($"{MiscUtil.FormatNumber((int)user.xp)}/{MiscUtil.FormatNumber((int)neededXP)}", robotoSmall, brush, new Point(600-((int)xpSize.Width/2), 205));
            brush.Color = MiscUtil.GetAverageColor(avatar);
            img.DrawString($"LEVEL", robotoSmall, brush, new Point(908-(int)levelNumSize.Width-6-(int)levelSize.Width, 26));
            img.DrawString($"{user.level}", roboto, brush, new Point(908-(int)levelNumSize.Width, 16));
            brush.Color = Color.Gray;
            img.DrawString($"#{dUser.Discriminator}", robotoSmall, brush, new PointF(292+usernameSize.Width, 150));

            // Save and send the image
            string imagePath = TempManager.GetTempFile($"rankCard-{user.username}-{user.id}.png", true);
            bmp.Save(imagePath);
            await Context.Channel.SendFileAsync(imagePath);
            TempManager.RemoveTempFile($"rankCard-{user.username}-{user.id}.png");
        }
    }
}