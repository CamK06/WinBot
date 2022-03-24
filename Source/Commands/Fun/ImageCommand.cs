// Stupid random picture command that'll probably end up having to be removed because of abuse lmfao
// Just a joke command anyways
// Thanks Jan.
using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;
using static WinBot.Util.ResourceManager;

using Newtonsoft.Json;
using DSharpPlus;

namespace WinBot.Commands.Main
{
    public class ImageCommand : BaseCommandModule
    {
        [Command("img")]
        [Description("Gets a random user-submitted image")]
        [Usage("[add (or leave blank)] [image (or leave blank)]")]
        [Category(Category.Fun)]
        public async Task Image(CommandContext Context, string command = null, [RemainingText]string image = null)
        {
            string jsonFile = GetResourcePath("randomImages", Util.ResourceType.JsonData);

            // Verify the image json file and/or load it
            if(imageUrls == null) {
                if(File.Exists(jsonFile))
                    imageUrls = JsonConvert.DeserializeObject<List<UserImage>>(File.ReadAllText(jsonFile));
                else {
                    imageUrls = new List<UserImage>();
                    File.WriteAllText(jsonFile, JsonConvert.SerializeObject(imageUrls, Formatting.Indented));
                }
            }

            // If we're viewing an existing image
            if(imageUrls.FirstOrDefault(x => x.id == command) != null) {
                UserImage nImage = imageUrls.FirstOrDefault(x => x.id == command);

                // Create the embed
                DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
                eb.WithTitle("Image");
                eb.WithColor(DiscordColor.Gold);
                eb.WithImageUrl(nImage.url);
                eb.WithFooter($"ID: {nImage.id}\nSubmitted by: {nImage.author}\nSubmit your own with the \"img add\" command");
                await Context.ReplyAsync("", eb.Build());

                return;
            }

            // If we're viewing a random image
            if(command == null || command.ToLower() != "add" && command.ToLower() != "del" && command.ToLower() != "count") {
                if(imageUrls.Count == 0)
                    throw new System.Exception("There are no images! Use the image add command to add some.");
                UserImage randImage = imageUrls[new System.Random().Next(0, imageUrls.Count)];

                // Create the embed
                DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
                eb.WithTitle("Random Image");
                eb.WithColor(DiscordColor.Gold);
                eb.WithImageUrl(randImage.url);
                eb.WithFooter($"ID: {randImage.id}\nSubmitted by: {randImage.author}\nSubmit your own with the \"img add\" command");
                await Context.ReplyAsync("", eb.Build());
            }
            // If we're adding a new image
            else if(command.ToLower() == "add") {

                ImageArgs args = ImageCommandParser.ParseArgs(Context, image);
                string newImg = args.url;

                // Verify that the image is indeed a valid image
                WebClient client = new WebClient();
                client.OpenRead(newImg);
                if (!client.ResponseHeaders["Content-Type"].Contains("image") || client.ResponseHeaders["Content-Type"].Contains("svg"))
                    throw new System.Exception("Your file is not a valid image!");
                if(imageUrls.FirstOrDefault(x => x.url == newImg) != null)
                    throw new Exception("That image already exists!");

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
                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(imageUrls, Formatting.Indented));
                await Context.ReplyAsync($"Successfully added your image! ID:`{id}`");
            }
            // If we're removing an image
            else if(command.ToLower() == "del") {
                if(!PermissionMethods.HasPermission(Context.Member.PermissionsIn(Context.Channel), Permissions.ManageMessages) && Context.User.Id != Bot.client.CurrentApplication.Owners.FirstOrDefault().Id)
                    throw new System.Exception("You lack the sufficient permissions to run this command");

                if(image == null)
                    throw new System.Exception("You must provide an image to remove");

                UserImage imageToRemove = imageUrls.FirstOrDefault(x => x.id == image);
                if(imageToRemove == null)
                    throw new System.Exception("You must provide a valid image ID");

                imageUrls.Remove(imageToRemove);
                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(imageUrls, Formatting.Indented));
                await Context.ReplyAsync($"Successfully removed `{image}`!");
            }
            else if(command.ToLower() == "count")
                await Context.ReplyAsync($"There are {imageUrls.Count} images.");
        }

        static List<UserImage> imageUrls = null;
    }

    class UserImage
    {
        public string author, url, id;
        public System.DateTime date;
    }
}