#if TOFU
using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

using WinBot.Misc;

namespace WinBot.Commands.Owner
{
    public class ChatCommands : BaseCommandModule
    {
        [Command("addprompt")]
        [Description("Adds a prompt to the chatbot")]
        [Usage("[id] [response]")]
        [Category(Category.Owner)]
        public async Task AddPrompt(CommandContext Context, string id, [RemainingText]string response)
        {
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
				throw new Exception("You must be the bot owner to run this command!");
			
            // Create the prompt
			Prompt prompt = new Prompt()
            {
                prompt = id,
                responses = new string[] { response },
                weights = new System.Collections.Generic.Dictionary<string, float>()
            };
            ChatSystem.prompts.Add(prompt);
            ChatSystem.SavePrompts();

            // Responses
            await Context.ReplyAsync($"Successfully added prompt with ID: `{id}`");
            await Context.ReplyAsync("Remember to add weights to this prompt");
        }

        [Command("addweight")]
        [Description("Adds weights to a chatbot prompt")]
        [Usage("[promptId, [weight(word,val)]]")]
        [Category(Category.Owner)]
        public async Task AddWeight(CommandContext Context, string promptId, [RemainingText]string weight)
        {
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
				throw new Exception("You must be the bot owner to run this command!");
			
            // Check if the weight is valid
            if(weight.Split(',').Length <= 1)
                throw new Exception("Invalid weight. It must be formatted as follows: `[word],[val]`");

            // Split the weight
            string word = "";
            word = weight.Split(',')[0];
            int.TryParse(weight.Split(',')[1], out int value);
            if(value <= 0)
                throw new Exception("Invalid weight value");
            if(string.IsNullOrWhiteSpace(word))
                throw new Exception("Failed to get word");

            // Add the weight
			Prompt prompt = ChatSystem.prompts.FirstOrDefault(x => x.prompt == promptId);
            prompt.weights.Add(word.ToLower(), value);
            ChatSystem.SavePrompts();

            await Context.ReplyAsync($"Successfully added weight `{weight}` to prompt with ID: `{promptId}`");
        }

        [Command("modweight")]
        [Description("Modify a weight of a chatbot prompt")]
        [Usage("[promptId] [word] [value]")]
        [Category(Category.Owner)]
        public async Task ModWeight(CommandContext Context, string promptId, string word, float value)
        {
            if(Context.User.Id != Bot.config.ownerId && !Bot.whitelistedUsers.Contains(Context.User.Id))
				throw new Exception("You must be the bot owner to run this command!");

            // Modify the weight
			Prompt prompt = ChatSystem.prompts.FirstOrDefault(x => x.prompt == promptId);
            if(prompt == null)
                throw new Exception("Invalid or no prompt");
            var weight = prompt.weights.FirstOrDefault(x => x.Key == word);
            if(weight.Value <= 0)
                throw new Exception("Invalid or no weight");
            prompt.weights.Remove(weight.Key);
            
            prompt.weights.Add(word, value);
            ChatSystem.SavePrompts();

            await Context.ReplyAsync($"Successfully modified weight `{word},{value}` to prompt with ID: `{promptId}`");
        }
    }
}
#endif