using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace WinBot.Commands.Fun
{
	public class GraphCommand : ModuleBase<SocketCommandContext>
	{
		[Command("graph")]
		[Summary("It's that one Nickelback meme.|[Image]")]
		[Priority(Category.Fun)]
		public async Task Graph(string image = null)
		{
			WebClient client = new WebClient();
			string fileName = "graph.png";

			// Change the image URL to an attachment if one is present
			if (image == null && Context.Message.Attachments.Count > 0)
				image = Context.Message.Attachments.FirstOrDefault().Url;

			// Download the image
			if (image.ToLower().EndsWith(".png"))
				client.DownloadFile(image, "graph.png");
			else if (image.ToLower().EndsWith(".jpg"))
			{
				client.DownloadFile(image, "graph.jpg");
				fileName = "graph.jpg";
			}
			else
			{
				await ReplyAsync("Invalid image! You must supply an image either as an attachment or url in jpg or png format");
				return;
			}

			// Create the image
			Bitmap chad = new Bitmap(Bitmap.FromFile("chadClean.png"));
			Bitmap dImage = new Bitmap(Bitmap.FromFile(fileName));
			using (Graphics canvas = Graphics.FromImage(chad))
			{
				canvas.DrawImage(dImage, graphPos);
				canvas.Save();
			}

			// Send the image
			chad.Save(fileName);
			await Context.Channel.SendFileAsync(fileName);
		}

		static Point[] graphPos = new Point[] {
			new Point(900, 500),  // Top left
			new Point(1440, 385), // Top right
			new Point(985, 890),  // Bottom left
		};
	}
}