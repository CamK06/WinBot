using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using Discord;
using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class RateCommand : ModuleBase<SocketCommandContext>
    {
        [Command("rate")]
        [Summary("Rate something|[Thing to rate]")]
        [Priority(Category.Fun)]
        private async Task Rate([Remainder] string option)
        {
            Random r = new Random();
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);

            if(option.ToLower().Contains("windows")) {
                if(option.ToLower().Contains("10") || option.ToLower().Contains("ten")) {
                    eb.WithTitle($"🤔 I give **{option}** a solid **SHIT** rating");
                    await ReplyAsync("", false, eb.Build());
                    return;
                }
            }
            else if(option.ToLower().Contains("star")) {
                if(option.ToLower().Contains("man")) {
                    eb.WithTitle($"🤔 I give **{option}** a solid **GOD** rating");
                    await ReplyAsync("", false, eb.Build());
                    return;
                }
            }

            if(!ratings.ContainsKey(option.ToLower()))
                eb.WithTitle($"🤔 I give **{option}** a solid {r.Next(1, 10)}/10");
            else
                eb.WithTitle($"🤔 I give **{option}** a solid {ratings[option.ToLower()]}/10");
            await ReplyAsync("", false, eb.Build());
        }

        private Dictionary<string, int> ratings = new Dictionary<string, int>()
        {
            { "microsoft", 0 }, { "linux", 11 }, { "arch", 11 },
            { "debian", 11 }, { "ubuntu", 6 }, { "windows 10", -99 },
            { "windows", 4 }, { "windows 8", 7 }, { "windows 8.1", 9 },
            { "windows 7", 11 }, { "windows 2000", 11 }, { "duff", 486 },
            { "windows10", -99 }, { "windows 98", 98 }, { "windows98", 98 },
            { "microsoft windows 10", -99}, { "microsoft windows10", -99 },
            { "microsoftwindows10", -99 }, { "adobe", 0 }, { "discord", -1 },
            { "discord app", -1 }, { "discordapp", -1 }, { "irc", 100 },
            { "mirc", 11 }, {"windows 95", 95}, {"windows95", 95}
        };
    }
}