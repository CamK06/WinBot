using System;
using System.Timers;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using DiscARC.Commands.Attributes;
using DiscARC.Util;


namespace DiscARC.Commands.Main
{
    public class EightBallCommand : BaseCommandModule
    {
        [Command("8ball")]
        [Description("The magic 8 ball reaches into the future, to find the answers to your questions. It knows what will be, and is willing to share this with you.")]
        [Usage("[Query]")]
        [Category(Category.Fun)]
        public async Task EightBall(CommandContext Context, [RemainingText] string query = null)
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            Random r = new Random();              
            
            int choiceIndex = r.Next(Bot.predictions.Length); 
            string choice = Bot.predictions[choiceIndex];

            // Choose an embed color based on the response selected
            switch (choiceIndex) {
                case int index when (index <= 9):
                    eb.WithColor(DiscordColor.SapGreen);
                    break;
                case int index when ((9 < index) && (index <= 14)):
                    eb.WithColor(DiscordColor.Orange);
                    break;
                case int index when (14 < index):
                    eb.WithColor(DiscordColor.IndianRed);
                    break;
            }

            // If no query is provided, throw an exception
            if (query == null)
                throw new Exception("No query provided!");
            else
                eb.AddField($"The answer to your question, \"{query}\", is..." , choice);
            eb.WithTitle("🎱 I reach into the future...");
            
            await Context.ReplyAsync("", eb.Build());
        }
        
    }
}
