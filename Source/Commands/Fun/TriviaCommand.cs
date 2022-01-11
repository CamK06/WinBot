using System;
using System.Web;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;

using WinBot.Misc;
using WinBot.Commands.Attributes;

using Newtonsoft.Json;

namespace WinBot.Commands.Main
{
    public class TriviaCommand : BaseCommandModule
    {
        [Command("trivia")]
        [Aliases("t")]
        [Description("Get a random trivia question")]
        [Usage("[category]")]
        [Category(Category.Main)]
        public async Task Trivia(CommandContext Context, string input = null)
        {
            // If the user is requesting their stats
            if(input.ToLower() == "stats") {
                User u = UserData.GetOrCreateUser(Context.User);
                await Context.ReplyAsync($"You've answered {u.totalTrivia} questions. Of those, {u.correctTrivia} ({Math.Round((float)u.correctTrivia/(float)u.totalTrivia*100.0f)}%) were correct");
                return;
            }

            // Generate an API request
            string URL = "https://opentdb.com/api.php?amount=1";
            if(!string.IsNullOrWhiteSpace(input) && Categories.ContainsKey(input.ToLower()))
                URL += $"&category={Categories[input.ToLower()]}";

            // Make the API call
            string json = new WebClient().DownloadString(URL);
            dynamic output = JsonConvert.DeserializeObject(json);

            // Create a list of the answers
            List<string> answerStrings = new List<string>();
            answerStrings.Add((string)output.results[0].correct_answer);
            for(int i = 0; i < output.results[0].incorrect_answers.Count; i++)
                answerStrings.Add((string)output.results[0].incorrect_answers[i]);
            
            // Shuffle answers
            int answer = 0;
            string answerText = "";
            for(int i = answerStrings.Count-1; i > 0; i--) {
                
                // Swap random answers
                int k = new Random().Next(i+1);
                string value = answerStrings[k];
                answerStrings[k] = answerStrings[i];
                answerStrings[i] = value;

                // Set the answer value for later
                if(value == (string)output.results[0].correct_answer)
                    answer = i;
                else if(answerStrings[k] == (string)output.results[0].correct_answer)
                    answer = k;
                answerText = HttpUtility.HtmlDecode(answerStrings[answer]);
            }

            // Cleanly format the answers
            for(int i = 0; i < answerStrings.Count; i++) 
                answerStrings[i] = $"[{i + 1}] " + HttpUtility.HtmlDecode(answerStrings[i]);
                
            // Create and send trivia embed
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithAuthor(Context.User.Username, null, Context.User.GetAvatarUrl(DSharpPlus.ImageFormat.Png));
            eb.WithFooter("Type your choice (number)");
            eb.AddField("**Trivia**", $"A(n) {output.results[0].difficulty} question from the {output.results[0].category} category");
            eb.AddField("**Question**", $"{HttpUtility.HtmlDecode((string)output.results[0].question)}");
            eb.AddField("**Answers**", $"```cs\n{string.Join("\n", answerStrings)}\n```");
            DiscordMessage msg = await Context.ReplyAsync(eb);

            // Wait for a response
            DateTime gameStart = DateTime.Now;
            var result = await Context.Channel.GetNextMessageAsync(m => {
                string txt = m.Content;
                return m.Content == "1" || m.Content == "2" || m.Content == "3" || m.Content == "4"; 
            });

            // Timeout message
            if(result.TimedOut) {
                await msg.RespondAsync("Timed out after 1 minute.");
                return;
            }

            // Parse the answer number
            int.TryParse(result.Result.Content, out int index);
            index -= 1;
            
            // Verify answer & send response embed
            DiscordEmbedBuilder eb2 = new DiscordEmbedBuilder();
            User user = UserData.GetOrCreateUser(result.Result.Author);
            if(index == answer) {
                user.correctTrivia++;
                eb2.WithTitle($"That is correct, {result.Result.Author.Username}! It was `{answerText}`");
                eb2.WithColor(new DiscordColor("#3BA55D"));
            }
            else {
                eb2.WithTitle($"That is incorrect, {result.Result.Author.Username}! The answer is `{answerText}`");
                eb2.WithColor(new DiscordColor("#ED4245"));
            }
            user.totalTrivia++;
            eb2.WithFooter($"Answer time: {Math.Round(DateTime.Now.Subtract(gameStart).TotalSeconds)}s");
            await Context.ReplyAsync(eb2);
        }

        internal static Dictionary<string, int> Categories = new Dictionary<string, int>(){
            {"general", 9},
            {"books", 10}, {"book", 10},
            {"films", 11}, {"film", 11}, {"movies", 11}, {"movie", 11},
            {"music", 12},
            {"musicals", 13}, {"musical", 13}, {"theatre", 13}, {"theater", 13},
            {"tv", 14}, {"television", 14},
            {"games", 15}, {"game", 15}, {"gaming", 15},
            {"boardgames", 16}, {"boardgame", 16}, {"boards", 16}, {"board", 16},
            {"science", 17}, {"nature", 17},
            {"computers", 18}, {"computer", 18}, {"it", 18}, {"technology", 18}, {"tech", 18},
            {"math", 19}, {"maths", 19}, {"mathematics", 19},
            {"mythology", 20}, {"myths", 20}, {"myth", 20},
            {"sports", 21}, {"sport", 21},
            {"geography", 22}, {"geo", 22},
            {"history", 23},
            {"politics", 24},
            {"art", 25},
            {"celebrities", 26}, {"celebrity", 26}, {"celebs", 26}, {"celeb", 26},
            {"animals", 27}, {"animal", 27},
            {"vehicles", 28}, {"vehicle", 28},
            {"comics", 29}, {"comic", 29},
            {"gadgets", 30}, {"gadget", 30},
            {"japan", 31}, {"japanese", 31},  {"anime", 31}, {"mange", 31},  {"animu", 31}, {"mango", 31},
            {"cartoons", 32}, {"cartoon", 32},
        };
    }
}