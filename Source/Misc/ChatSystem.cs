using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.Text.RegularExpressions;

using DSharpPlus.Entities;

// TODO: clean up this file as it is a direct copy and paste from the prototype/POC

namespace WinBot.Misc
{
    public class ChatSystem
    {
        public static List<Prompt> prompts;
        private static float threshold = 25.0f;

        public static string Respond(string msg, DiscordUser user)
        {
            // Strip the message of punctuation to make things easy
            msg = Regex.Replace(msg, @"[^\w\s]", "");

            Prompt prompt = GetPrompt(msg);
            if(prompt == null)
                return null;

            return prompt.responses.Random().Replace("@u", user.Mention);
        }

        private static Prompt GetPrompt(string msg)
        {
            if(prompts == null)
                LoadPrompts();

            string[] words = msg.ToLower().Split(" ");

            // Test the message against all prompts
            Prompt bestPrompt = null;
            float bestWeight = 0.0f;
            foreach(Prompt prompt in prompts) {
                
                float totalWeight = 0.0f;
                for(int i = 0; i < words.Length; i++) {
                    if(prompt.weights.TryGetValue(words[i], out float weight)) {
                        totalWeight += weight;
                    } else {
                        totalWeight -= 1.0f; // If we don't know what tf is being said, remove one from weight
                    }
                }
                if(totalWeight > bestWeight) {
                    bestPrompt = prompt;
                    bestWeight = totalWeight;
                }
            }

            if(bestWeight >= threshold)
                return bestPrompt;
            else
                return null;
        }

        public static void LoadPrompts()
        {
            if(!File.Exists("chatPrompts.json"))
                throw new Exception("There is no prompts file!");

            prompts = JsonConvert.DeserializeObject<List<Prompt>>(File.ReadAllText("chatPrompts.json"));
        }

        public static void SavePrompts()
        {
            File.WriteAllText("chatPrompts.json", JsonConvert.SerializeObject(prompts, Formatting.Indented));
        }
    }

    public class Prompt
    {
        // Possible way to improve things; add a threshold for each individual prompt
        public string prompt { get; set; }
        public string[] responses { get; set; }
        public Dictionary<string, float> weights { get; set; }
    }

    public static class LinqExtensions
    {
        public static TSource Random<TSource>(this IEnumerable<TSource> source)
        {
            Random r = new Random();
            IList<TSource> list = source as IList<TSource>;

            // Return a random value
            if(list != null)
                if(list.Count > 0) {
                    return list[r.Next(0, list.Count)];
                }

            // Return default if nothing can be done
            return default(TSource);
        }
    }
}