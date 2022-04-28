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

using MarkovSharp.Components;
using MarkovSharp.Models;
using MarkovSharp.TokenisationStrategies;

using Newtonsoft.Json;

namespace WinBot.Commands.Fun
{
    public class MarkovCommand : BaseCommandModule
    {
        [Command("markov")]
        [Aliases(new string[] { "mk" })]
        [Description("Search the urban dictionary")]
        [Usage("[query]")]
        [Category(Category.Fun)]
        public async Task Markov(CommandContext Context, [RemainingText]string input = null)
        {
            DiscordMember user = null;
            List<string> data = null;
            string sourceName = "Source: User-Submitted Messages";

            // If there is input, parse it for a user. If there is no input, source messages from the msg command
            if(input != null) 
                user = Context.Guild.SearchMembersAsync(input, 1).Result.FirstOrDefault();
            else {
                data = new List<string>();
                string json = File.ReadAllText(GetResourcePath("randomMessages", Util.ResourceType.JsonData));
                List<UserMessage> msgs = JsonConvert.DeserializeObject<List<UserMessage>>(json);
                foreach(UserMessage msg in msgs)
                    data.Add(msg.content);
            }

            // If the user is invalid error, if the user is valid, set the data source to their messages
			if(user == null && input != null)
				throw new Exception("You must provide a valid non-bot user!");
            else if(input != null) { 
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