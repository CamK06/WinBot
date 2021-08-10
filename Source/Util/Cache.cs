using System;
using System.IO;
using System.Timers;

using Serilog;
using Serilog.Events;

namespace WinBot.Util
{
    public class Cache
    {
        public static void InitTimer()
        {
            // Set up the timer
            Timer t = new Timer(43200000);
            t.AutoReset = true;
            t.Elapsed += (object sender, ElapsedEventArgs e) => {
                Flush();
            };
            t.Start();
        }

        public static void Verify()
        {
            if(!Directory.Exists("Cache")) {
                Directory.CreateDirectory("Cache");
                Log.Write(LogEventLevel.Information, "Created new cache directory");
            }
        }

        public static void Add(string json, string name)
        {
            Cache.Verify();
            string path = $"Cache/{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}_{name}.json";
            if(File.Exists(path))
                return;

            File.WriteAllText(path, json);
        }

        public static string Fetch(string name)
        {
            Cache.Verify();
            string path = $"Cache/{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}_{name}.json";
            if(File.Exists(path))
                return File.ReadAllText(path);
            else {
                Log.Write(LogEventLevel.Warning, $"{name} does not exist in the cache, returning null.");
                return null;
            }
        }

        public static void Flush()
        {
            Directory.Delete("Cache", true);
            Directory.CreateDirectory("Cache");
            Log.Write(LogEventLevel.Information, "Cache has been flushed");
        }
    }
}