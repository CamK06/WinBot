using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Discord.WebSocket;

namespace WinWorldBot.Utils
{
    class YudsCounter
    {
        public static List<Weight> weights = new List<Weight>();

        public static bool IsQuestion(string text, SocketTextChannel debugChannel = null)
        {
            // Check for question marks, we do this first so as not to waste processing power calculating weights only to find a question mark
            if(text.ToLower().Contains("?") || text.ToLower().Contains("ʔ") || text.ToLower().Contains("¿"))
                return true;

            // Calculate the total weight
            float totalWeight = 0.0f;
            foreach(Weight weight in weights)
                if(text.ToLower().Contains(weight.word))
                    totalWeight += weight.value;

            Log.Write("DEBUG: Yuds counter weight is " + totalWeight);

            if(debugChannel != null)
            {
                string message = $"The total weight is: **{totalWeight}**\nMatched words:";
                foreach(Weight weight in weights)
                    if(text.ToLower().Contains(weight.word))
                        message += $"\n``{weight.word}``:``{weight.value}``";
                debugChannel.SendMessageAsync(message);
            }

            // Determine if the weight is high enough for it to be a question
            if(totalWeight >= 70.5f)
                return true;
            else
                return false;
        }

        public static void LoadWeights()
        {
            foreach(string line in File.ReadAllLines("weights.txt"))
            {
                // Line exclusions/comments
                if(line.Contains("#")) continue; // Comments
                if(String.IsNullOrWhiteSpace(line)) continue; // Empty lines

                // Check if a line has enough values before proceeding
                if(line.Contains(",") && line.Split(',').Count() > 1)
                {
                    string lline = line.ToLower().Replace(" ", "");
                    var split = lline.Split(',');

                    // Parse the weight value
                    float.TryParse(split.LastOrDefault(), out float value);

                    // Add the weight
                    weights.Add(new Weight()
                    {
                        word = split.FirstOrDefault(),
                        value = value
                    });
                }
                else
                {
                    // Error handling
                    Console.ForegroundColor = ConsoleColor.Red;
                    Log.Write($"ERROR: Weight {line} failed to validate!");
                    Console.ResetColor();
                }
            }
        }
    }

    class Weight
    {
        public string word { get; set; }
        public float value { get; set; }
    }
}