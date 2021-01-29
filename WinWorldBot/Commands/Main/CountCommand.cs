using System;
using System.Threading.Tasks;

using Discord.WebSocket;
using Discord.Commands;

using WinWorldBot.Data;

/* this is all kinda messy but eh, it works right? */

namespace WinWorldBot.Commands
{
    public class CountCommand : ModuleBase<SocketCommandContext>
    {
        [Command("count")]
        [Summary("Determine how many times something has been said|[Word or Phrase] [User]")]
        [Priority(Category.Main)]
        private async Task Count(string text, SocketGuildUser user = null)
        {
            await Context.Channel.TriggerTypingAsync();
            
            int count = 0;
            if(user != null)
            {
                foreach(UserMessage msg in UserData.GetUser(user).Messages)
                    if(msg.Content != null && msg.Content.ToLower().Contains(text.ToLower())) count++;
                await ReplyAsync($"{user} has said \"{text}\" {count} times since {UserData.GetUser(user).StartedLogging.ToShortDateString()}");
                return;
            }
            else
            {
                foreach(User member in UserData.Users)
                    foreach(UserMessage msg in member.Messages)
                        if(msg.Content != null && msg.Content.ToLower().Contains(text.ToLower())) count++;
                await ReplyAsync($"\"{text}\" has been said {count} times since {new DateTime(2021, 1, 10).ToShortDateString()}");
                return;
            }
        }
    }
}