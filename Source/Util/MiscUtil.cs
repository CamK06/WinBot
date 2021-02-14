using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace WinBot.Util
{
	public class MiscUtil
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
	}
}