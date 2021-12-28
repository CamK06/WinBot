using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;
using WinBot.Misc;
using WinBot.Util;

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

            // Generate an embed description
            string description = "";
            int userCounter = 0;
            bool hasDisplayedCurrentUser = false;
            foreach(User lbUser in leaderboard) {
                if(userCounter < 10) {
                    description += $"**{userCounter+1}.** {lbUser.username} - {lbUser.level} ({MiscUtil.FormatNumber((int)lbUser.totalxp)} Total XP)\n";
                    if(lbUser.id == Context.User.Id)
                        hasDisplayedCurrentUser = true;
                }
                else if(lbUser.id == Context.User.Id) {
                    description += $"**{userCounter+1}.** {lbUser.username} - {lbUser.level} ({MiscUtil.FormatNumber((int)lbUser.totalxp)} Total XP)\n";
                    if(userCounter != 10) 
                        description += "...";
                }
                else if(userCounter == 10 && !hasDisplayedCurrentUser) {
                    description += "...\n";
                }
                userCounter++;
            }

            // Create the embed
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithThumbnail(topUser.GetAvatarUrl(DSharpPlus.ImageFormat.Jpeg));
            eb.WithDescription(description);
            eb.WithFooter("Note: this is NOT a temporary leaderboard");
            await Context.ReplyAsync("", eb.Build());
        }
    }
}


/*
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;
using WinBot.Misc;
using WinBot.Util;
using static WinBot.Util.ResourceManager;

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
            int lineCounter = 0;
            string longestLine = "";
            foreach(User lbUser in leaderboard) {

                string toAdd = $"{userCounter+1}. {lbUser.username} - {lbUser.level} ({MiscUtil.FormatNumber((int)lbUser.totalxp)} Total XP)";

                if(userCounter < 10) {
                    description += $"{toAdd}\n\n";
                    if(toAdd.Length > longestLine.Length)
                        longestLine = toAdd;
                    lineCounter++;
                }
                else if(lbUser.id == Context.User.Id) {
                    description += $"{toAdd}\n\n";
                    if(toAdd.Length > longestLine.Length)
                        longestLine = toAdd;
                    lineCounter++;
                }
                userCounter++; 
            }

            // Set up text drawing
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddFontFile(GetResourcePath("Roboto-Regular.ttf", ResourceType.Resource));
            SolidBrush brush = new SolidBrush(Color.White);
            StringFormat drawForm = new StringFormat();
            Font roboto = new Font(fonts.Families[0].Name, 50, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF longestLineLen = MiscUtil.MeasureString(longestLine, roboto);

            // Image creation:

            // Setup
            Bitmap bmp = new Bitmap((int)longestLineLen.Width+52, (100*lineCounter)-44);
            Graphics img = Graphics.FromImage(bmp);
            img.Clear(Color.FromArgb(35, 39, 42));

            brush.Color = Color.White;
            img.DrawString(description, roboto, brush, new PointF(26, 26));

            // Save and send the image
            string imagePath = TempManager.GetTempFile($"leaderBoard.png", true);
            bmp.Save(imagePath);
            await Context.Channel.SendFileAsync(imagePath);
            TempManager.RemoveTempFile($"leaderBoard.png");
        }
    }
}*/