#if TOFU
using System;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;
using static WinBot.Util.ResourceManager;

using Newtonsoft.Json;

using Serilog;

namespace WinBot.Commands.Staff
{
    public class MuteCommand : BaseCommandModule
    {
        [Command("mute")]
        [Description("Mute a dirtbag")]
        [Usage("[user]")]
        [Attributes.Category(Category.Staff)]
        [RequireUserPermissions(DSharpPlus.Permissions.KickMembers)]
        public async Task Mute(CommandContext Context, [RemainingText] DiscordMember user)
        {
            // Try to set the muted role
            if(Bot.config.ids.mutedRole == 0) {
                await Context.ReplyAsync("No muted role is set in the bot config!");
                Log.Error("Muted role is not set in the config!");
                return;
            }
            if(Global.mutedRole == null)
                Global.mutedRole = Global.targetGuild.GetRole(Bot.config.ids.mutedRole);
            if(Global.mutedRole == null)
                throw new Exception("Shitcord is failing to return a valid muted role");

            // Toggle the role on the user
            if (!Global.mutedUsers.Contains(user.Id)) {
                await user.GrantRoleAsync(Global.mutedRole);
                Global.mutedUsers.Add(user.Id);
                await Context.ReplyAsync($"Muted {user.Username}!");
            }
            else {
                await user.RevokeRoleAsync(Global.mutedRole);
                Global.mutedUsers.Remove(user.Id);
                await Context.ReplyAsync($"Unmuted {user.Username}!");
            }
            File.WriteAllText(GetResourcePath("mute", ResourceType.JsonData), JsonConvert.SerializeObject(Global.mutedUsers, Formatting.Indented));
        }
    }
}
#endif