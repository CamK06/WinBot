// NOTE: The code in this file is hilariously shit, I'm just lazy and want this to work lol. Don't care about the quality.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using static WinBot.Util.ResourceManager;
using WinBot.Misc;
using WinBot.Util;
using WinBot.Commands.Attributes;

using MarkovSharp.TokenisationStrategies;

using Newtonsoft.Json;

namespace WinBot.Commands.Fun
{
    public class MarkovCommand : BaseCommandModule
    {
        [Command("markov")]
        [Aliases(new string[] { "mk" })]
        [Description("Markov chains and things")]
        [Usage("[user] [length]")]
        [Category(Category.Fun)]
        public async Task Markov(CommandContext Context, DiscordMember user = null, [RemainingText]int length = 5)
        {
            List<string> data = null;
            string sourceName = "Source: User-Submitted Messages";

            if(user == null) {
                data = new List<string>();
                string json = File.ReadAllText(GetResourcePath("randomMessages", Util.ResourceType.JsonData));
                List<UserMessage> msgs = JsonConvert.DeserializeObject<List<UserMessage>>(json);
                foreach(UserMessage msg in msgs)
                    data.Add(msg.content);
            }
            else { 
                User bUser = UserData.GetOrCreateUser(user);
                data = bUser.messages;
                sourceName = "Source: " + bUser.username;
            }

            if(length > 25)
                length = 25;
            else if(length < 0)
                length = 1;

            // Generate the markov text
            StringMarkov model = new StringMarkov(1);
            model.Learn(data);
            model.EnsureUniqueWalk = true;

            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithAuthor(sourceName);
            eb.WithColor(DiscordColor.Gold);
            eb.WithFooter(data.Count + " messages in data. Better results will be achieved with more messages.");
            eb.WithDescription(string.Join(' ', model.Walk(length)).Replace("@", "").Truncate(4096));
            await Context.ReplyAsync(eb);
        }
    }
}