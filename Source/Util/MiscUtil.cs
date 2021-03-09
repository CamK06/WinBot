using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace WinBot.Util
{
	public static class MiscUtil
	{
		public static List<ulong> LoadBlacklist()
		{
			if(!File.Exists("blacklist.json")) {
				File.WriteAllText("blacklist.json", JsonConvert.SerializeObject(new List<ulong>()));
				return new List<ulong>();
			}
			return JsonConvert.DeserializeObject<List<ulong>>(File.ReadAllText("blacklist.json"));
		}

		public static void SaveBlacklist()
		{
			File.WriteAllText("blacklist.json", JsonConvert.SerializeObject(Bot.blacklistedUsers, Formatting.Indented));
		}

		public static string Truncate(this string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value)) return value;
			return value.Length <= maxLength ? value : value.Substring(0, maxLength-3)+"...";
		}
	}
}