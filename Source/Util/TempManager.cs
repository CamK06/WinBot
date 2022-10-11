using System;
using System.IO;
using System.Timers;
using System.Collections.Generic;

using Serilog;

using Newtonsoft.Json;

using static WinBot.Util.ResourceManager;

namespace WinBot.Util
{
    public class TempManager
    {
        public static Dictionary<string, string> tempFiles = new Dictionary<string, string>();

        public static void Init()
        {
            Timer t = new Timer(43200000);
            t.AutoReset = true;
            t.Elapsed += (object sender, ElapsedEventArgs e) => {
                Flush();
            };

            // Load temp files
            if(File.Exists(GetResourcePath("tempFiles", ResourceType.JsonData))) {
                string json = File.ReadAllText(GetResourcePath("tempFiles", ResourceType.JsonData));
                tempFiles = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }

        public static string GetTempFile(string name, bool replaceExisting = false)
        {
            // Sanitize the name
            name = name.Replace("/", "");

            // Check for and return existing temp file
            // Delete existing temp file if we are replacing it
            if(!replaceExisting && tempFiles.ContainsKey(name))
                return tempFiles[name];
            else if(tempFiles.ContainsKey(name) && replaceExisting)
                tempFiles.Remove(name);

            // Generate a new file path
            string filePath = $"Temp/{new Random().Next(9999, 99999)}-{name}";
            tempFiles.Add(name, filePath);
            Log.Information($"Created temp file with name: {name} and path: {filePath}");

            // Save temp files
            File.WriteAllText(GetResourcePath("tempFiles", ResourceType.JsonData),
                              JsonConvert.SerializeObject(tempFiles, Formatting.Indented));

            return filePath;
        }

        public static bool TempFileExists(string name)
        {
            return tempFiles.ContainsKey(name);
        }

        public static void RemoveTempFile(string name)
        {
            // Sanitize the name
            name = name.Replace("/", "");

            // Do nothing if the file doesn't exist
            if(!tempFiles.ContainsKey(name))
                return;

            // Remove the file
            File.Delete(tempFiles[name]);
            tempFiles.Remove(name);

            // Save temp files
            File.WriteAllText(GetResourcePath("tempFiles", ResourceType.JsonData), 
                              JsonConvert.SerializeObject(tempFiles, Formatting.Indented));
        }

        public static void Flush()
        {
            // This should never *actually* be ran
            if(!Directory.Exists("Temp"))
                return;

            Directory.Delete("Temp", true);
            Directory.CreateDirectory("Temp");
            tempFiles.Clear();
            File.WriteAllText(GetResourcePath("tempFiles", ResourceType.JsonData), 
                              JsonConvert.SerializeObject(tempFiles, Formatting.Indented));
            Log.Information("Temp files have been cleared");
        }
    }
}