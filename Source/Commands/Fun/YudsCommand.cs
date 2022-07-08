using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class YudsCommand : BaseCommandModule
    {
        [Command("yuds")]
        [Description("Yuds")]
        [Category(Category.Fun)]
        [Hidden]
        public async Task Yuds(CommandContext Context, [RemainingText]string query)
        {
            await Context.ReplyAsync(michaelRosenGifs[new Random().Next(0, michaelRosenGifs.Length)]);
        }

        public static string[] michaelRosenGifs = new string[]
        {
            "https://c.tenor.com/N66me6-wOwMAAAAd/nice-good.gif",
            "https://c.tenor.com/9wZMhJpXUlIAAAAM/michael-rosen-rosen.gif",
            "https://c.tenor.com/pEtOxcxh_xUAAAAC/noice-michael-rosen.gif",
            "https://c.tenor.com/9d_38ep4Z7oAAAAM/michael-rosen-rosen.gif",
            "https://c.tenor.com/DLFnnOHY_2UAAAAC/michael-rosen-horrible.gif",
            "https://tenor.com/view/michael-rosen-rosen-angry-fridge-i-wonder-who-put-that-there-gif-21326805",
            "https://tenor.com/view/i-dont-think-its-funny-michael-rosen-rosen-not-funny-gif-19357247",
            "https://c.tenor.com/1GdMoBplrvcAAAAM/funny-michael-rosen.gif",
            "https://c.tenor.com/NddAwitDVBUAAAAM/noice.gif",
            "https://c.tenor.com/yaM9VJk2t1YAAAAM/michael-rosen-shocked.gif",
            "https://c.tenor.com/Rz3DamDZLPgAAAAM/michael-rosen-confused.gif",
            "https://c.tenor.com/Bgt8_GZcv1sAAAAM/michael-rosen-rosen.gif",
            "https://c.tenor.com/5K3_Yr8GCDwAAAAC/michael-rosen-rosen.gif",
            "https://c.tenor.com/mUbmxAS1JqoAAAAM/bendy-wobbely.gif",
            "https://c.tenor.com/KTg0qdyXaKoAAAAM/spaghetti.gif",
            "https://c.tenor.com/GvtC6UdYS7IAAAAM/michael-rosen-yum.gif",
            "https://c.tenor.com/LW2-bFmkagwAAAAM/michael-rosen-stare.gif",
            "https://c.tenor.com/3DOlOgUPqfUAAAAM/michael-rosen-exited.gif",
            "https://c.tenor.com/UQdp3km2HqoAAAAd/danke.gif",
            "https://c.tenor.com/ZYYGAFSt3PcAAAAC/michael-rosen-eat.gif",
            "https://c.tenor.com/oJ-8tkmanKIAAAAM/michael-rosen-fantastic.gif",
            "https://c.tenor.com/VRm3_aNDn_0AAAAM/micheal-rosen.gif",
            "https://c.tenor.com/pN4Gorwq9h8AAAAM/what-pout.gif"
        };
    }
}