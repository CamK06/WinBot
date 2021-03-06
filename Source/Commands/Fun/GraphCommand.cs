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

			// Change the image URL to an attachment if one is present
			if (image == null && Context.Message.Attachments.Count > 0)
				image = Context.Message.Attachments.FirstOrDefault().Url;

			// Check filesize
			client.OpenRead(image);
			Int64 fileSize = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
			if(!client.ResponseHeaders["Content-Type"].Contains("image"))
			{
				await ReplyAsync("Your file is not an image!");
				return;
			}
			if(fileSize > 16777216) // 16MB limit
			{
				await ReplyAsync("Your file must be below 16MB in size!");
				return;
			}
			string extension = client.ResponseHeaders["Content-Type"].Split("image/").Last();

			// Download the image
			client.DownloadFile(image, $"graph.{extension}");

			// Create the image
			Bitmap chad = new Bitmap(Bitmap.FromFile("chadClean.png"));
			Bitmap dImage = new Bitmap(Bitmap.FromFile($"graph.{extension}"));
			using (Graphics canvas = Graphics.FromImage(chad))
			{
				canvas.DrawImage(dImage, graphPos);
				canvas.Save();
			}

			// Send the image
			chad.Save($"graph.{extension}");
			await Context.Channel.SendFileAsync($"graph.{extension}");
		}

		static Point[] graphPos = new Point[] {
			new Point(900, 500),  // Top left
			new Point(1440, 385), // Top right
			new Point(985, 890),  // Bottom left
		};
	}
}