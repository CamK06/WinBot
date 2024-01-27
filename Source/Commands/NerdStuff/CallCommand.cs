using System;
using System.Xml;
using System.Net.Http;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;

using Newtonsoft.Json;

using HtmlAgilityPack;

namespace WinBot.Commands.NerdStuff
{
    public class CallCommand : BaseCommandModule
    {
        [Command("call")]
        [Description("Look up a ham radio callsign")]
        [Attributes.Category(Category.NerdStuff)]
        public async Task Call(CommandContext Context, string callsign)
        {
            string returnedXml = new HttpClient().GetStringAsync($"https://xmldata.qrz.com/xml/current/?username={Bot.config.apiKeys.qrzUsername}&password={Bot.config.apiKeys.qrzPassword}&callsign={callsign}").Result;
            string returnedHtml = new HttpClient().GetStringAsync($"https://www.qrz.com/db/{callsign}").Result;
            
            // Parse the HTML to fetch the image
            // TODO: Add more than just the image
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(returnedHtml);
            string imageUrl = doc.DocumentNode.SelectSingleNode("//img[@id='mypic']").Attributes["src"].Value;

            // Parse the XML and convert to JSON to make handling it easy
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(returnedXml);
            string json = JsonConvert.SerializeXmlNode(xmlDoc);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            if(jsonObj.QRZDatabase.Session.Error != null)
                throw new Exception("That callsign does not exist!");
            
            // Basic callsign data
            string call = jsonObj.QRZDatabase.Callsign.call;
            string fname = jsonObj.QRZDatabase.Callsign.fname;
            string name = jsonObj.QRZDatabase.Callsign.name;
            string addr2 = jsonObj.QRZDatabase.Callsign.addr2;
            string state = jsonObj.QRZDatabase.Callsign.state;
            string country = jsonObj.QRZDatabase.Callsign.country;
            
            // Create and send embed
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithTitle($"QRZ Data for {call}");
            eb.WithUrl($"https://www.qrz.com/db/{call}");
            eb.WithColor(DiscordColor.Gold);
            eb.AddField("Name", fname + " " + name, true);
            eb.AddField("Address", addr2 + ", " + state, true);
            eb.AddField("Country", country, true);
            if(imageUrl != null)
                eb.WithThumbnail(imageUrl);
            eb.WithTimestamp(DateTime.Now);
            await Context.Channel.SendMessageAsync(eb.Build());
        }
    }
}
