using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;
using WinBot.Misc;
using WinBot.Util;

using System.Drawing;
using System.Drawing.Text;

namespace WinBot.Commands.Main
{
    public class LeaderboardCommand : BaseCommandModule
    {
        [Command("lb")]
        [Description("Show the server leaderboard")]
        [Category(Category.Main)]
        public async Task Leaderboard(CommandContext Context)
        {
            // Get the leaderboard and #1 user
            List<User> leaderboard = Leveling.GetOrderedLeaderboard();
            DiscordUser topUser = Bot.client.GetUserAsync(leaderboard[0].id).Result;

            // Generate text
            string description = "";
            int userCounter = 0;
            string longestLine = "";
            bool hasDisplayedCurrentUser = false;
            foreach(User lbUser in leaderboard) {

                string toAdd = $"{userCounter+1}. {lbUser.username} - {lbUser.level} ({MiscUtil.FormatNumber((int)lbUser.totalxp)} Total XP)";

                if(userCounter < 10) {
                    description += $"{toAdd}\n\n";
                    if(lbUser.id == Context.User.Id)
                        hasDisplayedCurrentUser = true;
                    if(toAdd.Length > longestLine.Length)
                        longestLine = toAdd;
                }
                else if(lbUser.id == Context.User.Id) {
                    description += $"{toAdd}\n\n";
                    if(userCounter != 10)
                        description += "...";
                    if(toAdd.Length > longestLine.Length)
                        longestLine = toAdd;
                }
                else if(userCounter == 10 && !hasDisplayedCurrentUser) {
                    description += "...\n";
                }
                userCounter++;
            }

            // Set up text drawing
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddFontFile("Roboto-Regular.ttf");
            SolidBrush brush = new SolidBrush(Color.White);
            StringFormat drawForm = new StringFormat();
            Font roboto = new Font(fonts.Families[0].Name, 50, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF longestLineLen = MiscUtil.MeasureString(longestLine, roboto);

            // Image creation:

            // Setup
            Bitmap bmp = new Bitmap((int)longestLineLen.Width+52, (100*userCounter)-14);
            Graphics img = Graphics.FromImage(bmp);
            img.Clear(Color.FromArgb(35, 39, 42));

            brush.Color = Color.White;
            img.DrawString(description, roboto, brush, new PointF(26, 26));

            // Save and send the image
            string imagePath = "leaderboard.png";
            bmp.Save(imagePath);
            await Context.Channel.SendFileAsync(imagePath);
        }
    }
}
