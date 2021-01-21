using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Utils;
using WinWorldBot.Data;

using System.Runtime.Serialization.Formatters.Binary;

namespace WinWorldBot.Commands
{
    public class JsonToBin : ModuleBase<SocketCommandContext>
    {
        [Command("json2bin")]
        [Summary("Does stuff|")]
        [Priority(Category.Owner)]
        private async Task JsonToBinary()
        {
            if(Context.Message.Author.Id != Globals.StarID)
                return;

            await ReplyAsync("Converting data from JSON to bin");
            FileStream file = new FileStream("out.bin", FileMode.OpenOrCreate);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file, UserData.Users);
            file.Close();
            await ReplyAsync("Done!");
        }
    }
}