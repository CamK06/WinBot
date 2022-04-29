using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;

using MarkovSharp.TokenisationStrategies;

namespace WinBot.Commands.Fun
{
    public class HackermanCommand : BaseCommandModule
    {
        [Command("hackerman")]
        [Description("Hack into the mainframes... just kidding; Hack into Toxidation's network.")]
        [Usage("[lines]")]
        [Category(Category.Fun)]
        public async Task Hackerman(CommandContext Context, int lines = 1)
        {
            if(lines > 10)
                throw new System.Exception("You cannot generate more than 10 lines!");
            else if(lines <= 0)
                throw new System.Exception("You cannot generate less than 0 or 0 lines");

            Random r = new Random();

            // Generate pure nonsense
            List<string> data = new List<string>();
            for(int i = 0; i < 1000; i++) {
                string jargon = actions[r.Next(0, actions.Length)] + things[r.Next(0, things.Length)] + " with the " + hs[r.Next(0, hs.Length)];
                data.Add(jargon);
            }

            // Markov the shit out of it
            StringMarkov model = new StringMarkov(1);
            model.Learn(data);
            string text = model.Walk().First().Replace("@", "");
            for(int i = 0; i < lines-1; i++)
                text += "\n\n" + model.Walk().First().Replace("@", "");
            
            // Send an embed
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithDescription($"```cpp\n{text.Truncate(2000)}```");
            await Context.ReplyAsync("", eb.Build());
        }

        static string[] things = new string[] {
            "non-rotatable disk", "side fumbling CPU", "processor", "with multidimension network security access vulnerabilities",
            "oc6 level optical line", "microprocessor architecture", "server", "minecraft server",
            "webserver running Linux 0.01", "Linux system", "shell access terminals", "vulnerable networking firewall",
            "multiphase process memorizer", "x86 IBM level architecture", "network firewall daemon", "network routing device",
            "insecure Windows server", "Windows server 1985", "transdimensional phasing device"
        };

        static string[] hs = new string[] {
            "non malluable flemmy trammy hacker keyboard", "optical network", "vmware 2003 server", "kali linux elite hackerman edition", "elite hacker skills",
            "kali linux", "linux terminal", "hackerman elite tools utility package", "norton 3000", "norton softworks", "my peter norton", "my elite hacker keyboard",
            "my friends at anonymous", "help of my skills learned from working with the anonymous elite super ultra mega super hackerman hacktivist hacker group"
        };

        static string[] actions = new string[] {
            "I've hacked into your ", "I'm breaking into the ", "I've hacked the ", "I've gained effective root access to your ",
            "I'm gaining root access to ", "I've hacked the ", "I broke into the "
        };
    }
}