using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Drawing2D;
using DSharpPlus.Entities;
using Serilog;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace WinBot.Util
{
    public static class MiscUtil
    {
        public static List<ulong> LoadBlacklist()
        {
            if (!File.Exists("blacklist.json"))
            {
                File.WriteAllText("blacklist.json", JsonConvert.SerializeObject(new List<ulong>()));
                return new List<ulong>();
            }
            return JsonConvert.DeserializeObject<List<ulong>>(File.ReadAllText("blacklist.json"));
        }

        public static void SaveBlacklist()
        {
            File.WriteAllText("blacklist.json", JsonConvert.SerializeObject(Bot.blacklistedUsers, Formatting.Indented));
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
        }

        public static string FormatDate(DateTimeOffset dt)
        {
            return dt.ToString("dddd, dd MMMM yyyy");
        }

        // For use in about command
        public static string GetHost()
        {
            try
            {
                if (Environment.OSVersion.ToString().ToLower().Contains("unix")) // Run uname if we're on a UNIX system
                    return "uname -sr".Bash();
                else if (Environment.OSVersion.ToString().ToLower().Contains("windows")) // Run systeminfo if we're on a Windows system
                    return "systeminfo".Bash();
                else // Otherwise we're on some really fucked up shit
                    return null;
            }
            catch { Environment.Exit(0); return null; }
        }


        // Taken from stackoverflow because of course it is: https://stackoverflow.com/a/33853557
        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static SizeF MeasureString(string s, Font font)
        {
            SizeF result;
            using (var image = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(image))
                {
                    result = g.MeasureString(s, font);
                }
            }

            return result;
        }

        public static Color GetAverageColor(Bitmap bmp)
        {

            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color clr = bmp.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            //Calculate average
            r /= total;
            g /= total;
            b /= total;

            return Color.FromArgb(r, g, b);
        }

        public static string FormatNumber(int num)
        {
            if (num >= 100000)
                return FormatNumber(num / 1000) + "K";
            if (num >= 10000)
            {
                return (num / 1000D).ToString("0.#") + "K";
            }
            if(num >= 1000) {
                return (num / 1000D).ToString("#.0") + "K";
            }
            return num.ToString("#,0");
        }
    }
}

namespace DSharpPlus.CommandsNext
{
    public static class DSharpImprovements
    {
        public static async Task<DiscordMessage> SendFileAsync(this DiscordChannel channel, string fileName)
        {
            if(!File.Exists(fileName)) {
                Log.Warning($"File does not exist! (SendFileAsync @ {channel.Name})");
                return null;
            }

            FileStream fStream = new FileStream(fileName, FileMode.Open);
            DiscordMessage msg = await new DiscordMessageBuilder().WithFile(fileName, fStream).SendAsync(channel);
            fStream.Close();

            return msg;
        }

        public static async Task<DiscordMessage> ReplyAsync(this CommandContext Context, string Content)
        {
            return await Context.Channel.SendMessageAsync(Content);
        }

        public static async Task<DiscordMessage> ReplyAsync(this CommandContext Context, string Content, DiscordEmbed Embed)
        {
            return await Context.Channel.SendMessageAsync(Content, Embed);
        }

        public static async Task<DiscordMessage> ReplyAsync(this CommandContext Context, DiscordEmbed Embed)
        {
            return await Context.Channel.SendMessageAsync("", Embed);
        }
    }
}