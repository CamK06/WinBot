namespace WinBot.Misc
{
    public class BotConfig
    {
        public string token { get; set; } = "TOKEN";
        public string[] prefixes { get; set; } = { "PREFIX" };
        public string status { get; set; } = " ";
        public ActivityType activityType { get; set; } = ActivityType.Playing;
        public IDConfig ids { get; set; } = new IDConfig();
        public APIKeys apiKeys { get; set; } = new APIKeys();
    }

    public class IDConfig
    {
        public ulong hostGuild { get; set; } = 0;
        public ulong logChannel { get; set; } = 0;
        public ulong botOwner { get; set; } = 0;
    }

    public class APIKeys
    {
        public string weather { get; set; } = "WEATHERAPIKEY";
    }

    public static class Global
    {
        public static DiscordChannel logChannel { get; set; }
    }
}