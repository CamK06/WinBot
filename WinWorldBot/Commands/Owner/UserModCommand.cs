using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Utils;
using WinWorldBot.Data;

namespace WinWorldBot.Commands
{
    public class UserModCommand : ModuleBase<SocketCommandContext>
    {
        [Command("usermod"), Alias("um")]
        [Summary("Modify a user's stats.|")]
        [Priority(Category.Owner)]
        private async Task Exec(SocketUser User, UserStat Stat, int val)
        {
            if(Context.Message.Author.Id != 363850072309497876) return;

            User u = UserData.GetUser(User);

            switch(Stat)
            {
                case UserStat.IncorrectTrivia:
                    u.IncorrectTrivia = val;
                    break;
                case UserStat.CorrectTrivia:
                    u.CorrectTrivia = val;
                    break;

                default: break;
            }

            UserData.SaveData();
        }
    }

    enum UserStat
    {
        IncorrectTrivia, CorrectTrivia
    }
}
