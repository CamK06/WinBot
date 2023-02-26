#if !TOFU
using System;
using System.IO;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.Entities;

using CodeHollow.FeedReader;

using Newtonsoft.Json;

using Serilog;

using WinBot.Util;
using CodeHollow.FeedReader.Feeds;

namespace WinBot.Misc
{
    public class WWRSS
    {
        public static List<string> sentItems = new List<string>();

        public static async Task Init()
        {
            try {
            // Load cached items
            if (ResourceManager.ResourceExists("rss.cache"))
                sentItems = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(ResourceManager.GetResourcePath("rss.cache")));

            // Set up the timer
            Timer t = new Timer(43200000);
            t.AutoReset = true;
            t.Elapsed += async (object sender, ElapsedEventArgs e) => {
                await FetchItems();
            };
            t.Start();

            // Do an initial fetch of items
            await FetchItems();

            Log.Write(Serilog.Events.LogEventLevel.Information, "RSS service started");
            }catch(System.Exception ex) {
                Log.Write(Serilog.Events.LogEventLevel.Information, ex.Message);
            }
        }

        public static async Task FetchItems()
        {
            try {
            // Setup
#if !BLOAT
            var feed = await FeedReader.ReadAsync("https://winworldpc.com/downloads/latest.rss");
#else
            var feed = await FeedReader.ReadAsync("https://tech.hindustantimes.com/rss/tech/news");
#endif
            DiscordChannel additions = await Bot.client.GetChannelAsync(Bot.config.ids.rssChannel);

            foreach (FeedItem item in feed.Items)
            {
                // Don't send an item twice
                if(sentItems.Contains(item.Id))
                    continue;

#if BLOAT
                if(!(item.Title.ToLower().Contains("asteroid") || item.Title.ToLower().Contains("earth")
                || item.Title.ToLower().Contains("nasa") || item.Title.ToLower().Contains("space")
		        || item.Title.ToLower().Contains("alien") || item.Title.ToLower().Contains("ufo")
                || item.Title.ToLower().Contains("mars") || item.Title.ToLower().Contains("life")))
                    continue;
#endif

#if !BLOAT
                // Create and send the embed
                DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
                eb.WithTitle(item.Title);
                eb.WithUrl(item.Link);
                eb.WithImageUrl(item.Content);
                eb.WithColor(DiscordColor.Red);
                await additions.SendMessageAsync("", eb.Build());
#else
                var klauses = additions.Guild.GetEmojisAsync().Result.Where(x => x.Name.ToLower().Contains("klaus")).ToArray();
                var klaus = klauses[new Random().Next(0, klauses.Length)];
                await additions.SendMessageAsync($"{klaus} {item.Title} {klaus}\n{item.Link}");
#endif

                // Cache the item so it isn't sent in the next fetch
                sentItems.Add(item.Id);
                
                await Task.Delay(1024);
                File.WriteAllText(ResourceManager.GetResourcePath("rss.cache"), JsonConvert.SerializeObject(sentItems, Formatting.Indented));
                await Task.Delay(1024);
            }
            } catch(System.Exception ex) {
                Log.Write(Serilog.Events.LogEventLevel.Information, ex.Message);
            }
        }
    }
}
#endif
