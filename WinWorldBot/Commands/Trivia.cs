using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class Trivia
    {
        public static List<TriviaGame> games = new List<TriviaGame>();

        public static async Task Init()
        {
            //Bot.client.ReactionAdded += ReactionAdded;
            Bot.client.MessageReceived += MessageReceived;
        }

        private static async Task MessageReceived(SocketMessage arg)
        {
            try
            {
                if (arg.Author.IsBot) return;

                // If a game exists on the message
                if (games.FirstOrDefault(x => x.channelID == arg.Channel.Id) != null)
                {
                    TriviaGame game = games.FirstOrDefault(x => x.channelID == arg.Channel.Id);

                    if (HasValidNumber(arg.Content) && game.isActive)
                    {
                        int Index = GetNumber(arg.Content);

                        EmbedBuilder Embed = new EmbedBuilder();

                        if (Index == game.answer) // The answer given is correct
                        {
                            Embed.WithTitle($"That is correct, {arg.Author.Username}! It was `{game.answerText}`");
                            Embed.WithColor(Color.Green);
                            Embed.WithFooter($"Answer time: {Math.Round(DateTime.Now.Subtract(game.startedAt).TotalSeconds)}s");
                            await arg.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else // The answer given is incorrect
                        {
                            Embed.WithTitle($"That is incorrect, {arg.Author.Username}. The answer is `{game.answerText}`");
                            Embed.WithColor(Color.Red);
                            Embed.WithFooter($"Answer time: {Math.Round(DateTime.Now.Subtract(game.startedAt).TotalSeconds)}s");
                            await arg.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        games.Remove(game);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
                Log.Write(ex.StackTrace);
            }
        }
        
        private static int GetNumber(string message)
        {
            string textNum = message.FirstOrDefault(x => x == '1' || x == '2' || x == '3' || x == '4').ToString();
            int.TryParse(textNum, out int val);
            Log.Write("Input text was " + message + " and outputted number was " + val);
            return val-1;
        }

        private static bool HasValidNumber(string message)
        {
            return message == "1" || message == "2" || message == "3" || message == "4";
        }

        private static async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {
                if (reaction.User.Value.IsBot) return;

                // If a game exists on the message
                if (games.FirstOrDefault(x => x.messageID == reaction.MessageId) != null)
                {
                    TriviaGame game = games.FirstOrDefault(x => x.messageID == reaction.MessageId);

                    if (TriviaCommand.Reactions.Contains(reaction.Emote) && game.isActive)
                    {
                        int Index = TriviaCommand.Reactions.FindIndex(x => x.Name.ToLower() == reaction.Emote.Name.ToLower());

                        EmbedBuilder Embed = new EmbedBuilder();

                        if (Index == game.answer) // The answer given is correct
                        {
                            Embed.WithTitle($"That is correct, {reaction.User.Value.Username}! It was `{game.answerText}`");
                            Embed.WithColor(Color.Green);
                            Embed.WithFooter($"Answer time: {Math.Round(DateTime.Now.Subtract(game.startedAt).TotalSeconds)}s");
                            await channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else // The answer given is incorrect
                        {
                            Embed.WithTitle($"That is incorrect, {reaction.User.Value.Username}. The answer is `{game.answerText}`");
                            Embed.WithColor(Color.Red);
                            Embed.WithFooter($"Answer time: {Math.Round(DateTime.Now.Subtract(game.startedAt).TotalSeconds)}s");
                            await channel.SendMessageAsync("", false, Embed.Build());
                        }
                        games.Remove(game);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
                Log.Write(ex.StackTrace);
            }
        }
    }

    public class TriviaGame
    {
        public bool isActive { get; set; }
        public int answer { get; set; }
        public ulong userID { get; set; }
        public ulong messageID { get; set; }
        public ulong channelID { get; set; }
        public string answerText { get; set; }
        public DateTime startedAt { get; set; }
    }
}