
using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using DiscARC.Commands.Attributes;


namespace DiscARC.Commands.Main
{
    public class EightBallCommand : BaseCommandModule
    {
        [Command("8ball")]
        [Description("The magic 8 ball reaches into the future, to find the answers to your questions. It knows what will be, and is willing to share this with you.")]
        [Usage("[Query]")]
        [Category(Category.Fun)]
        public async Task EightBall(CommandContext Context, string query = null)
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            Random r = new Random();  
            string[] predictions = {"It is certain", "It is decidedly so", "Without a doubt", "Yes – definitely", "You may rely on it", "As I see it, yes", "Most likely", "Outlook good", "Yes", "Signs point to yes", "Reply hazy, try again", "Ask again later", "Better not tell you now", "Cannot predict now", "Concentrate and ask again", "Don't count on it", "My reply is no", "My sources say no", "Outlook not so good", "Very doubtful"};
            
            
            int choiceIndex = r.Next(predictions.Length); 
            string choice = predictions[choiceIndex];

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
            if (query == null) { throw new Exception("No query provided!"); }
            else { eb.AddField($"The answer to your question, \"{query}\", is...", choice);}
            eb.WithTitle("🎱 I reach into the future...");
            
            await Context.ReplyAsync("", eb.Build());
        }
    }
}
