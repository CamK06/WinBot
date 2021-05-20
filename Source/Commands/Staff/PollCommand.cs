using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class PollCommand : BaseCommandModule
    {
        [Command("poll")]
        [Description("Create a poll")]
        [Usage("[Title] [Options]")]
        [Category(Category.Staff)]
        [RequireUserPermissions(DSharpPlus.Permissions.KickMembers)]
        public async Task Poll(CommandContext Context, string title, [RemainingText]string optionString) 
        {
            // Parse/verify options
            string[] options = optionString.Split(" ");
            if(options.Length < 2) {
                await Context.RespondAsync("You must provide *at least* two options.");
                return;
            }
            else if(options.Length > 10) {
                await Context.RespondAsync("You cannot have more than 10 options!");
                return;
            }
            for(int i = 0; i < options.Length; i++) {
                options[i] = $"{optionEmotes[i]} {options[i]}";
            }

            // Create an embed
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithTitle(title);
            eb.WithColor(DiscordColor.Gold);
            eb.WithTimestamp(DateTime.Now);
            eb.WithFooter($"Poll started by {Context.User.Username}#{Context.User.Discriminator}");
            eb.AddField("Options", string.Join('\n', options));
            var msg = await Context.RespondAsync("", eb.Build());

            // Add reactions
            for(int i = 0; i < options.Length; i++) {
                await msg.CreateReactionAsync(optionEmotes[i]);
                await Task.Delay(512);
            }
        }

        static DiscordEmoji[] optionEmotes = new DiscordEmoji[] {
            DiscordEmoji.FromUnicode("🇦"), DiscordEmoji.FromUnicode("🇧"),
            DiscordEmoji.FromUnicode("🇨"), DiscordEmoji.FromUnicode("🇩"),
            DiscordEmoji.FromUnicode("🇪"), DiscordEmoji.FromUnicode("🇫"),
            DiscordEmoji.FromUnicode("🇬"), DiscordEmoji.FromUnicode("🇭"),
            DiscordEmoji.FromUnicode("🇮"), DiscordEmoji.FromUnicode("🇯"),
        };
    }
}