using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;

using Discord;
using Discord.Commands;

using Newtonsoft.Json;

namespace WinWorldBot.Commands
{
    public class TriviaCommand : ModuleBase<SocketCommandContext>
    {
        internal static List<Emoji> Reactions = new List<Emoji>(){
            new Emoji("1️⃣"), new Emoji("2️⃣"), new Emoji("3️⃣"), new Emoji("4️⃣")
        };

        [Command("trivia"), Alias("t")]
        [Summary("Get asked a trivia question|[Category]")]
        [Priority(Category.Fun)]
        private async Task TriviaCmd(string input = null)
        {
            string json = "";
            string URL = "https://opentdb.com/api.php?amount=1";

            if (input != null && Categories.ContainsKey(input.ToLower()))
            {
                URL += $"&category={Categories[input.ToLower()]}";
            }

            // Download the json string from the API
            using (WebClient client = new WebClient())
                json = client.DownloadString(URL);
            dynamic output = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object

            // Answers
            List<string> AnswerStrings = new List<string>();
            AnswerStrings.Add((string)output.results[0].correct_answer);
            for (int i = 0; i < output.results[0].incorrect_answers.Count; i++)
                AnswerStrings.Add((string)output.results[0].incorrect_answers[i]);

            int Answer = 0;
            string AnswerText = "";

            // Shuffle the answers
            Random Rand = new Random();
            int n = AnswerStrings.Count;
            while (n > 1)
            {
                n--;
                int k = Rand.Next(n + 1);
                string value = AnswerStrings[k];
                AnswerStrings[k] = AnswerStrings[n];
                AnswerStrings[n] = value;

                if (value == (string)output.results[0].correct_answer)
                    Answer = n;
                else if (AnswerStrings[k] == (string)output.results[0].correct_answer)
                    Answer = k;
                AnswerText = HttpUtility.HtmlDecode(AnswerStrings[Answer]);
            }

            // Add the numbers to the answers
            for (int i = 0; i < AnswerStrings.Count; i++)
                AnswerStrings[i] = $"[{i + 1}] " + HttpUtility.HtmlDecode(AnswerStrings[i]);

            // Create and send the embed
            var eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);
            eb.WithAuthor(Context.Message.Author.Username, Context.Message.Author.GetAvatarUrl());
            eb.WithFooter("Type your choice (number)");
            eb.AddField("**Trivia**", $"A(n) {output.results[0].difficulty} question from the {output.results[0].category} category");
            eb.AddField("**Question**", $"{HttpUtility.HtmlDecode((string)output.results[0].question)}");
            eb.AddField("**Answers**", $"```cs\n{string.Join("\n", AnswerStrings)}\n```");

            // Send out the message with reactions
            var Msg = await ReplyAsync("", false, eb.Build());
            //await ReplyAsync("I believe the answer is " + Answer);

            TriviaGame Game = new TriviaGame()
            {
                userID = Context.User.Id,
                isActive = true,
                messageID = Msg.Id,
                answer = Answer,
                answerText = AnswerText,
                startedAt = DateTime.Now,
                channelID = Context.Channel.Id
            };
            Trivia.games.Add(Game);
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