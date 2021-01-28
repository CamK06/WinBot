using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace WinWorldBot.Utils
{
    internal class MiscUtil {
        public static string FormatDate(DateTimeOffset dt)
        {
            return dt.ToString("dddd, dd MMMM yyyy");
        }

        public static void SaveBlacklist()
        {
            string json = JsonConvert.SerializeObject(Bot.blacklistedUsers, Formatting.Indented);
            File.WriteAllText("blacklist.json", json);
        }

        public static List<ulong> LoadBlacklist()
        {
            if(!File.Exists("blacklist.json")) File.WriteAllText("blacklist.json", "[]");
            return JsonConvert.DeserializeObject<List<ulong>>(File.ReadAllText("blacklist.json"));
        }
    }
}