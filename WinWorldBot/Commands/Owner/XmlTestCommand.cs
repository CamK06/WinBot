using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Collections.Generic;
using System.Net;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class XmlTestCommand : ModuleBase<SocketCommandContext>
    {
        [Command("xmltest")]
        private Task XML()
        {
            List<Channel> ChannelList = new List<Channel>();
            List<Role> Roles = new List<Role>();

            foreach (SocketTextChannel channel in Context.Guild.TextChannels)
            {
                Channel data = new Channel()
                {
                    Name = channel.Name,
                    Topic = channel.Topic,
                    Id = channel.Id,
                    Category = channel.Category.Name,
                    nsfw = channel.IsNsfw,
                    CreatedAt = channel.CreatedAt.DateTime
                };
                ChannelList.Add(data);
            }
            foreach (SocketVoiceChannel channel in Context.Guild.VoiceChannels)
            {
                Channel data = new Channel()
                {
                    Name = channel.Name,
                    Category = channel.Category.Name,
                    Id = channel.Id,
                    CreatedAt = channel.CreatedAt.DateTime,
                    IsVoice = true
                };
                ChannelList.Add(data);
            }

            foreach (SocketRole role in Context.Guild.Roles)
            {
                Role data = new Role()
                {
                    Name = role.Name,
                    Colour = new RoleColour() { r = role.Color.R, g = role.Color.G, b = role.Color.B },
                    DisplaySeparately = role.IsHoisted,
                    AllowMentions = role.IsMentionable
                };
                Roles.Add(data);
            }

            Directory.CreateDirectory(Context.Guild.Name);
            using (WebClient client = new WebClient())
            {
                foreach (Emote emote in Context.Guild.Emotes)
                {
                    client.DownloadFile(emote.Url, Context.Guild.Name + "/" + emote.Name + ".png");
                }
                client.DownloadFile(Context.Guild.IconUrl, Context.Guild.Name + "/" + "image.png");
            }


            FileStream userStream = new FileStream("user.xml", FileMode.OpenOrCreate);
            XmlSerializer s = new XmlSerializer(typeof(Guild));
            s.Serialize(userStream, new Guild()
            {
                Name = Context.Guild.Name,
                Id = Context.Guild.Id,
                Channels = ChannelList.ToArray(),
                Roles = Roles.ToArray()
            });
            userStream.Close();

            return Task.CompletedTask;
        }
    }

    public class RoleColour
    {
        public int r, g, b;
    }

    public class Guild
    {
        public string Name;
        public ulong Id;

        public Channel[] Channels;
        public Role[] Roles;
    }

    public class Role
    {
        public string Name;
        public RoleColour Colour;
        public bool DisplaySeparately;
        public bool AllowMentions;
    }

    public class Channel
    {
        public string Name;
        public string Topic;
        public string Category;
        public ulong Id;
        public bool nsfw;
        public bool IsVoice;
        public DateTime CreatedAt;
    }
}
