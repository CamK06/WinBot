// Stupid random picture command that'll probably end up having to be removed because of abuse lmfao
// Just a joke command anyways
// Thanks Jan.
#if TOFU
using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using Newtonsoft.Json;

namespace WinBot.Commands.Main
{
    public class ImageCommand : BaseCommandModule
    {
        [Command("img")]
        [Description("Gets a random user-submitted image")]
        [Usage("[add (or leave blank)] [image (or leave blank)]")]
        [Category(Category.Main)]
        public async Task Image(CommandContext Context, string command = null, string image = null)
        {
            // Verify the image json file and/or load it
            if(imageUrls == null) {
                if(File.Exists("randomImages.json"))
                    imageUrls = JsonConvert.DeserializeObject<List<UserImage>>(File.ReadAllText("randomImages.json"));
                else {
                    imageUrls = new List<UserImage>();
                    File.WriteAllText("randomImages.json", JsonConvert.SerializeObject(imageUrls, Formatting.Indented));
                }
            }

            // If we're viewing a random image
            if(command == null || command.ToLower() != "add" && command.ToLower() != "del") {
                if(imageUrls.Count == 0)
                    throw new System.Exception("There are no images! Use the image add command to add some.");
                UserImage randImage = imageUrls[new System.Random().Next(0, imageUrls.Count)];

                // Create the embed
                DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
                eb.WithTitle("Random Picture");
                eb.WithColor(DiscordColor.Gold);
                eb.WithImageUrl(randImage.url);
                eb.WithFooter($"ID: {randImage.id}\nSubmitted by: {randImage.author}\nSubmit your own with the \"img add\" command");
                await Context.RespondAsync("", eb.Build());
            }
            // If we're adding a new image
            else if(command.ToLower() == "add") {
                string newImg = "";

                // Check for an image
                if(image == null && Context.Message.Attachments == null)
                    throw new System.Exception("You must provide an image to add!");
                else if(Context.Message.Attachments.Count > 0)
                    newImg = Context.Message.Attachments[0].Url;
                else if(image != null)
                    newImg = image;

                // Verify that the image is indeed a valid image
                WebClient client = new WebClient();
                client.OpenRead(newImg);
                if (!client.ResponseHeaders["Content-Type"].Contains("image") || client.ResponseHeaders["Content-Type"].Contains("svg")) {
                    throw new System.Exception("Your file is not a valid image!");
                }

                // Add the image
idRecalc:
                int idInt = new Random().Next(0, 0xFFFF);
                string id = Convert.ToBase64String(BitConverter.GetBytes(idInt));
                if(imageUrls.FirstOrDefault(x => x.id == id) != null)
                    goto idRecalc;
                imageUrls.Add(new UserImage() {
                    author = $"{Context.User.Username}#{Context.User.Discriminator}",
                    date = System.DateTime.Now,
                    url = newImg,
                    id = id
                });
                File.WriteAllText("randomImages.json", JsonConvert.SerializeObject(imageUrls, Formatting.Indented));
                await Context.RespondAsync($"Successfully added your image");
            }
            // If we're removing an image
            else if(command.ToLower() == "del" && Context.User.Id == Bot.client.CurrentApplication.Owners.FirstOrDefault().Id) {
                if(image == null)
                    throw new System.Exception("You must provide an image to remove");

                imageUrls.Remove(imageUrls.FirstOrDefault(x => x.id == image));
                File.WriteAllText("randomImages.json", JsonConvert.SerializeObject(imageUrls, Formatting.Indented));
                await Context.RespondAsync($"Successfully removed `{image}`");
            }
        }

        static List<UserImage> imageUrls = null;
    }

    class UserImage
    {
        public string author, url, id;
        public System.DateTime date;
    }
}
#endif