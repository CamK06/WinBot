using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using Newtonsoft.Json;
using DSharpPlus;

namespace WinBot.Commands.Staff
{
    public class BlacklistCommand : BaseCommandModule
    {
        [Command("blacklist")]
        [Description("Toggle blacklist on a user to prevent them from using the bot")]
        [Usage("[user]")]
        [Category(Category.Staff)]
        public async Task Blacklist(CommandContext Context, DiscordMember user = null)
        {
            if(!PermissionMethods.HasPermission(Context.Member.PermissionsIn(Context.Channel), Permissions.KickMembers) && Context.Member.Id != Bot.client.CurrentApplication.Owners.FirstOrDefault().Id)
                return;

            // This is a clunky method of listing things but it works
            if(user == null) {
                string list = "";
                foreach(ulong blacklistuser in Bot.blacklistedUsers) {
                    string username = $"{blacklistuser}";
                    if(Context.Guild.GetMemberAsync(blacklistuser).Result != null) {
                        try { 
                        username = Context.Guild.GetMemberAsync(blacklistuser).Result.Username;
                        } catch{}
                    }
                    list += username + "\n";
                }
                if(Bot.blacklistedUsers.Count <= 0)
                    list = "There are no blacklisted users.";

                DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
                eb.WithTitle("Blacklist");
                eb.WithColor(DiscordColor.Gold);
                eb.WithDescription($"```\n{list}```");
                await Context.RespondAsync("", eb.Build());
                return;
            }

            // Blacklist/unblacklist
            if(Bot.blacklistedUsers.Contains(user.Id)) {
                Bot.blacklistedUsers.Remove(user.Id);
                File.WriteAllText("blacklist.json", JsonConvert.SerializeObject(Bot.blacklistedUsers, Formatting.Indented));
                await Context.RespondAsync($"Unblacklisted {user.Username}#{user.Discriminator}!");
            }
            else {
                Bot.blacklistedUsers.Add(user.Id);
                File.WriteAllText("blacklist.json", JsonConvert.SerializeObject(Bot.blacklistedUsers, Formatting.Indented));
                await Context.RespondAsync($"Blacklisted {user.Username}#{user.Discriminator}!");
            }
        }
    }
}