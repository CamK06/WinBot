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
        [Usage("[user]")]
        [Category(Category.Fun)]
        public async Task Markov(CommandContext Context, [RemainingText]DiscordMember user = null)
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

            // Generate the markov text
            StringMarkov model = new StringMarkov(1);
            model.Learn(data);

            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithAuthor(sourceName);
            eb.WithColor(DiscordColor.Gold);
            eb.WithFooter(data.Count + " messages in data. Better results will be achieved with more messages.");
            eb.WithDescription(model.Walk().First().Replace("@", ""));
            await Context.ReplyAsync(eb);
        }
    }
}