using System;
using System.Net;
using System.Linq;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;

using ImageMagick;

namespace WinBot.Util
{
    class ImageCommandParser
    {
        public static ImageArgs ParseArgs(CommandContext Context, string input)
        {
            ImageArgs args = new ImageArgs();

            // No arguments, No URL
            if(Context.Message.Attachments.Count > 0) {     
                args.url = Context.Message.Attachments[0].Url;
            }
            // Reply
            else if(Context.Message.ReferencedMessage != null) {
                
                DiscordMessage referencedMessage = Context.Message.ReferencedMessage;
                if(referencedMessage.Attachments.Count > 0)
                    args.url = referencedMessage.Attachments[0].Url;
                else if(Uri.IsWellFormedUriString(referencedMessage.Content, UriKind.Absolute))
                    args.url = referencedMessage.Content;
                else if(referencedMessage.Embeds.Count > 0) {
                    if(referencedMessage.Embeds.FirstOrDefault(x => x.Image != null) != null) 
                        args.url = referencedMessage.Embeds.FirstOrDefault(x => x.Image != null).Image.Url.ToString();
                    else if (referencedMessage.Embeds.FirstOrDefault(x => x.Thumbnail != null) != null)
                        args.url = referencedMessage.Embeds.FirstOrDefault(x => x.Thumbnail != null).Thumbnail.Url.ToString();
                }
            }
            // No arguments, Image URL
            else if(Uri.IsWellFormedUriString(input, UriKind.Absolute)) {
                args.url = input;
            }
            // Arguments, Image URL
            else if(!string.IsNullOrWhiteSpace(input) && input.Split(' ').Count() > 1 && !input.Split(' ')[0].StartsWith("-")) {

                string[] splitArgs = input.Split(' ');
                for(int i = 0; i < splitArgs.Length; i++) {
                    
                    // If the current word is a URL
                    if(Uri.IsWellFormedUriString(splitArgs[i], UriKind.Absolute))
                        args.url = splitArgs[i];
                }
            }
            // Recent message
            if(args.url == null) {
                var messages = Context.Channel.GetMessagesAsync(30).Result;
                foreach(DiscordMessage msg in messages) {
                    
                    if(msg.Attachments.Count > 0) {
                        args.url = msg.Attachments[0].Url;
                        break;
                    }
                    else if(Uri.IsWellFormedUriString(msg.Content, UriKind.Absolute)) {
                        args.url = msg.Content;
                        break;
                    }
                    else if(msg.Embeds.Count > 0) {
                        if(msg.Embeds.FirstOrDefault(x => x.Image != null) != null) 
                            args.url = msg.Embeds.FirstOrDefault(x => x.Image != null).Image.Url.ToString();
                        else if (msg.Embeds.FirstOrDefault(x => x.Thumbnail != null) != null)
                            args.url = msg.Embeds.FirstOrDefault(x => x.Thumbnail != null).Thumbnail.Url.ToString();
                        break;
                    }
                }
                if(args.url == null)
                    throw new Exception("Invalid or no image! The image has to have been sent in the past 30 messages!");
            }
            args.url = args.url.Split('?')[0];

            // Tenor handling
            WebClient client = new WebClient();
            if(args.url.Contains("tenor.com/") && !args.url.Contains("c.tenor")) {

                // Download the tenor webpage
                string html = client.DownloadString(args.url);
                string htmlFrag = "<meta itemprop=\"contentUrl\" content=\"";
                if(!html.Contains(htmlFrag))
                    throw new Exception("Invalid or no image!");
                args.url = "";
                
                // Extract the URL from the HTML
                int first = html.IndexOf(htmlFrag) + htmlFrag.Length;
                for(int i = first; i < html.Length; i++) {
                    if(html[i] == '"')
                        break;
                    args.url += html[i];
                }
            }

            if(!string.IsNullOrWhiteSpace(input) && !Uri.IsWellFormedUriString(input, UriKind.Absolute)) {

                // Handle arguments
                string[] argsStr = input.Split(' ');
                for(int i = 0; i < argsStr.Length; i++) {
                    if(argsStr[i].StartsWith("-")) {

                        // Parse the argument name and value
                        string argName = argsStr[i].Replace("-", "").ToLower();
                        int.TryParse(argName.Split("=").LastOrDefault(), out int argVal);
                        argName = argName.Split("=")[0];

                        // Set the appropriate value
                        if(argName == "layers")
                            args.layers = argVal;
                        else if(argName == "size")
                            args.size = argVal;
                        else if(argName == "scale")
                            args.scale = argVal;
                        else
                            args.textArg = '-' + argName;
                    }
                    else if(!Uri.IsWellFormedUriString(argsStr[i], UriKind.Absolute))
                        args.textArg = argsStr[i];
                }
            }

            // Verify the image
            client.OpenRead(args.url);
            Int64 fileSize = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
            if (!client.ResponseHeaders["Content-Type"].Contains("image") || client.ResponseHeaders["Content-Type"].Contains("svg"))
                throw new Exception("Invalid or no image!");
            if(fileSize > 104900000)
                throw new Exception("Your image must be below 100MB in size!");
            args.extension = client.ResponseHeaders["Content-Type"].Split("image/").Last();

            // Verify the args
            if(args.scale > 10)
                throw new Exception("Scale must not be greater than 10!");
            if(args.size > 3000)
                throw new Exception("Size must not be greater than 3000!");

            return args;
        }
    }

    public class ImageArgs
    {
        public string url { get; set; } = null;
        public string extension { get; set; }
        public string textArg { get; set; }
        public int layers { get; set; } = 1;
        public int size { get; set; } = 1;
        public int scale { get; set; } = 1;
    }
}
