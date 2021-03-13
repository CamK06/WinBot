using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using AnimatedGif;

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
			if (Context.Message.Attachments.Count > 0)
				image = Context.Message.Attachments.FirstOrDefault().Url;
			else if (Emote.TryParse(image, out Emote emote))
				image = emote.Url;

			// Check filesize
			client.OpenRead(image);
			Int64 fileSize = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
			if (!client.ResponseHeaders["Content-Type"].Contains("image") || client.ResponseHeaders["Content-Type"].Contains("svg"))
			{
				await ReplyAsync("Your file is not a valid image!");
				return;
			}
			if (fileSize > 16777216) // 16MB limit
			{
				await ReplyAsync("Your file must be below 16MB in size!");
				return;
			}
			string extension = client.ResponseHeaders["Content-Type"].Split("image/").Last();

			// Download the image
			client.DownloadFile(image, $"graph.{extension}");
			if (extension == "gif")
			{
				await CreateGif();
				return;
			}

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

		async Task CreateGif()
		{
			// Setup
			Bitmap chad = new Bitmap(Bitmap.FromFile("chadClean.png"));
			Bitmap[] chads;

			// Read the original gif
			System.Drawing.Image originalGif = System.Drawing.Image.FromFile("graph.gif");
			FrameDimension dimension = new FrameDimension(originalGif.FrameDimensionsList[0]);
			int frameCount = originalGif.GetFrameCount(dimension);
			if (frameCount > 128) frameCount = 128;
			chads = new Bitmap[frameCount];

			// Process the original gif
			if (!Directory.Exists("gifTemp"))
				Directory.CreateDirectory("gifTemp");
			for (int i = 0; i < frameCount; i++)
			{
				// Create graph for current frame
				originalGif.SelectActiveFrame(dimension, i);
				using (Graphics canvas = Graphics.FromImage(chad))
				{
					canvas.DrawImage(originalGif, graphPos);
					canvas.Save();
				}
				chads[i] = new Bitmap(chad);
			}

			// Create the gif
			using (var gif = AnimatedGif.AnimatedGif.Create("nickelback.gif", 66))
				for (int i = 0; i < frameCount; i++)
					await gif.AddFrameAsync(System.Drawing.Image.FromFile($"gifTemp/{i}.png"));

			// Send the gif
			await Context.Channel.SendFileAsync("nickelback.gif");
		}
	}
}