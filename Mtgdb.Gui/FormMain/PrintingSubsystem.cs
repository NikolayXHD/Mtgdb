using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class PrintingSubsystem
	{
		private const int CardsPerRow = 4;
		private const int CardsPerColumn = 2;
		private const int PxPerInch = 300;
		private const float MmPerInch = 25.4f;
		private const float BorderMm = 0.25f;
		private static readonly SizeF SizeMm = new SizeF(63, 88);
		private static readonly int Border = (int)Math.Round(BorderMm * PxPerInch / MmPerInch);

		private static readonly Size CardSizePx = new Size(
				(int)Math.Round(SizeMm.Width * PxPerInch / MmPerInch),
				(int)Math.Round(SizeMm.Height * PxPerInch / MmPerInch));

		private static readonly int Height = CardSizePx.Height * CardsPerColumn + Border * (CardsPerColumn - 1);
		private static readonly int Width = CardSizePx.Width * CardsPerRow + Border * (CardsPerRow - 1);

		private readonly ImageRepository _imageRepository;
		private readonly CardRepository _cardRepository;

		public PrintingSubsystem(ImageRepository imageRepository, CardRepository cardRepository)
		{
			_imageRepository = imageRepository;
			_cardRepository = cardRepository;
		}

		public void ShowPrintingDialog(DeckModel deckModel)
		{
			var dlg = new SaveFileDialog
			{
				Filter = @"|*.png",
				AddExtension = true,
				DefaultExt = @".png"
			};

			if (dlg.ShowDialog() == DialogResult.OK)
				ThreadPool.QueueUserWorkItem(_ => { print(deckModel, dlg.FileName); });
		}

		private void print(DeckModel deckModel, string fileName)
		{
			var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
			var dir = Path.GetDirectoryName(fileName);
			if (dir == null)
				return;

			var ext = Path.GetExtension(fileName);

			Bitmap page;
			Graphics gr;

			createPage(out page, out gr);

			int x = 0, y = 0;
			int pageNum = 0;
			bool pageSaved = true;

			var decks = new[] { deckModel.MainDeck, deckModel.SideDeck };

			foreach (var deck in decks)
				foreach (string cardsId in deck.CardsIds)
				{
					var card = _cardRepository.CardsById[cardsId];

					if (card.ImageModel == null)
						continue;

					var model = _imageRepository.GetImagePrint(card, _cardRepository.GetReleaseDateSimilarity);
					var image = Image.FromFile(model.FullPath);

					int count = deck.GetCount(card);
					for (int c = 0; c < count; c++)
					{
						gr.DrawImage(
							image,
							x*(CardSizePx.Width + Border),
							y*(CardSizePx.Height + Border),
							CardSizePx.Width,
							CardSizePx.Height);

						pageSaved = false;

						x++;

						if (x == CardsPerRow)
						{
							x = 0;
							y++;
						}

						if (y == CardsPerColumn)
						{
							x = 0;
							y = 0;
							pageNum++;
							page.Save(Path.Combine(dir, $"{fileNameWithoutExt}.{pageNum}{ext}"), ImageFormat.Png);
							createPage(out page, out gr);
							pageSaved = true;
						}
					}
				}

			if (!pageSaved)
			{
				pageNum++;
				page.Save(Path.Combine(dir, $"{fileNameWithoutExt}.{pageNum}{ext}"), ImageFormat.Png);
			}
		}

		private static void createPage(out Bitmap page, out Graphics gr)
		{
			page = new Bitmap(Width, Height);
			page.SetResolution(PxPerInch, PxPerInch);
			gr = Graphics.FromImage(page);
			gr.FillRectangle(new SolidBrush(Color.White), new Rectangle(new Point(0, 0), page.Size));
		}
	}
}