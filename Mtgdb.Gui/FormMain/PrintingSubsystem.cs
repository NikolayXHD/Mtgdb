using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class PrintingSubsystem
	{
		private const int CardsPerRow = 4;
		private const int CardsPerColumn = 2;
		private const int PxPerInch = 300;
		private const float MmPerInch = 25.4f;
		private const float BorderMm = 0.25f;
		private static readonly SizeF _sizeMm = new SizeF(63, 88);
		private static readonly int _border = (int)Math.Round(BorderMm * PxPerInch / MmPerInch);
		private static readonly Size _cardSizePx = _sizeMm.MultiplyBy(PxPerInch / MmPerInch).Round();

		private static readonly int _height = _cardSizePx.Height * CardsPerColumn + _border * (CardsPerColumn - 1);
		private static readonly int _width = _cardSizePx.Width * CardsPerRow + _border * (CardsPerRow - 1);

		private readonly ImageRepository _imageRepository;
		private readonly CardRepository _cardRepository;

		public PrintingSubsystem(ImageRepository imageRepository, CardRepository cardRepository)
		{
			_imageRepository = imageRepository;
			_cardRepository = cardRepository;
		}

		public string SelectOutputFileDialog(string fileName)
		{
			var dlg = new SaveFileDialog
			{
				Filter = @"|*.png",
				AddExtension = true,
				DefaultExt = @".png",
				FileName = fileName.NullIfEmpty() ?? "deck"
			};

			if (dlg.ShowDialog() == DialogResult.OK)
				return dlg.FileName;

			return null;
		}

		public void Print(DeckEditorModel deckEditorModel, string fileName)
		{
			var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
			var dir = Path.GetDirectoryName(fileName);
			if (dir == null)
				return;

			var ext = Path.GetExtension(fileName);

			createPage(out var page, out var gr);

			int x = 0, y = 0;
			int pageNum = 0;
			bool pageSaved = true;

			var decks = new[] { deckEditorModel.MainDeck, deckEditorModel.SideDeck };

			foreach (var deck in decks)
				foreach (string cardsId in deck.CardsIds)
				{
					var card = _cardRepository.CardsById[cardsId];

					if (!card.HasImage(Ui))
						continue;

					var model = _imageRepository.GetImagePrint(card, _cardRepository.GetReleaseDateSimilarity);
					var image = Image.FromFile(model.ImageFile.FullPath.Value);

					int count = deck.GetCount(card.Id);
					for (int c = 0; c < count; c++)
					{
						gr.DrawImage(
							image,
							new Rectangle(
								x * (_cardSizePx.Width + _border),
								y * (_cardSizePx.Height + _border),
								_cardSizePx.Width,
								_cardSizePx.Height));

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
			page = new Bitmap(_width, _height);
			page.SetResolution(PxPerInch, PxPerInch);
			gr = Graphics.FromImage(page);
			gr.FillRectangle(Brushes.White, new Rectangle(new Point(0, 0), page.Size));
		}

		public UiModel Ui { get; set; }
	}
}
