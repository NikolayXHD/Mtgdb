using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Dal.EditDistance;
using Mtgdb.Test;
using NUnit.Framework;
using Tesseract;

namespace Mtgdb.Util
{
	[TestFixture]
	public class ArtistDetectionUtils : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			_engine = new TesseractEngine(
				TestContext.CurrentContext.TestDirectory.AddPath("tessdata"),
				"eng",
				EngineMode.CubeOnly);

			LoadModules();
			LoadCards();

			ImgRepo.LoadFiles();
			ImgRepo.LoadZoom();
			
			Repo.OnImagesLoaded();

			_distance = new LevenstineDistance();
		}

		[Test]
		public void DetectArtist()
		{
			string[] setCodes = 
			{
				"AKH","AER","BFZ","C14","C15","C16","CN2","CP1","CP2","CP3","DD3_DVD","DD3_EVG","DD3_GVL",
				"DD3_JVC","DDO","DDP","DDQ","DDQ","DDS","DTK","E01","EMA","EMN","HOU","KLD","MPS","MM2",
				"MM3","OGW","ORI","PCA","SOI","W17"
			};

			Rectangle[] rectangles =
			{
				new Rectangle(162, 988, 280, 23),
				//new Rectangle(114, 954, 260, 28)
			};

			Func<Bitmap, BmpProcessor>[][] preFilters =
			{
				new Func<Bitmap, BmpProcessor>[]{}
			};

			Func<Bitmap, BmpProcessor>[][] postFilters =
			{
				new Func<Bitmap, BmpProcessor>[]
				{
					scaled => new BwFilter(scaled, 0.55f),
					scaled => new BwFilter(scaled, 0.6f),
				}
			};

			var result = new StringBuilder();

			foreach (string setCode in setCodes)
			{
				var cards = Repo.SetsByCode[setCode].Cards
					.OrderBy(c => c.ImageName).ToList();

				_artists = cards
					.Where(c => c.Artist != null)
					.Select(c => c.Artist)
					.Distinct(Str.Comparer)
					.ToArray();

				_artistDistance = new float[_artists.Length];

				string[] texts = new string[rectangles.Length];

				for (int m = 0; m < cards.Count; m++)
				{
					var card = cards[m];
					var model = card.ImageModel;
					var path = model.FullPath;

					Stopwatch sw = new Stopwatch();

					for (int r = 0; r < rectangles.Length; r++)
					{
						string text = ocr(path, rectangles[r], preFilters[r], postFilters[r]);

						sw.Start();

						text = text ?? string.Empty;

						text = new Regex(@"\s+").Replace(text, " ");
						texts[r] = text;
					}

					for (int a = 0; a < _artists.Length; a++)
					{
						_artistDistance[a] = texts.Min(text => _distance.GetDistances(_artists[a], text)).Distance;
					}

					int artistIndex = Enumerable.Range(0, _artists.Length)
						.AtMin(a => _artistDistance[a])
						.Find();

					string detectedArtist = _artists[artistIndex];

					sw.Stop();
					long elapsedMatching = sw.ElapsedMilliseconds;

					if (!Str.Equals(card.Artist, detectedArtist))
					{
						string message = setCode + "\t" + card.ImageName + "\t" + card.Artist + "\t" + detectedArtist + "\t" + model.FullPath;
						Log.Debug(message);
						result.AppendLine(message);
					}
				}
			}
		}

		private string ocr(
			string bitmapPath,
			Rectangle rect,
			IList<Func<Bitmap, BmpProcessor>> preScaleFilters,
			IList<Func<Bitmap, BmpProcessor>> postScaleFilters)
		{
			var bitmap = new Bitmap(bitmapPath);

			if (bitmap.Width != 745 || bitmap.Height != 1040)
				return null;

			var textArea = getPart(bitmap, rect);

			foreach (var preScaleFilter in preScaleFilters)
				preScaleFilter(textArea).Execute();

			//textArea.Save("D:\\temp\\img\\text.png");

			float[] factors =
			{
				//2.3f,
				//2.4f,
				//2.5f,
				2.6f,
				//2.7f
			};

			string[] texts = new string[factors.Length * postScaleFilters.Count];
			float[] textConfs = new float[texts.Length];

			var sw = new Stopwatch();

			for (int f = 0; f < factors.Length; f++)
			{
				long elapsedPreprocess = sw.ElapsedMilliseconds;

				sw.Start();

				var scaledSize = new Size(
					(int) (rect.Width * factors[f]),
					(int) (rect.Height * factors[f]));

				for (int p = 0; p < postScaleFilters.Count; p++)
				{
					int i = f * postScaleFilters.Count + p;

					var scaled = textArea.FitIn(scaledSize);
					postScaleFilters[p](scaled).Execute();
					
					//scaled.Save("D:\\temp\\img\\bw.png");

					using (var page = _engine.Process(scaled, PageSegMode.SingleLine))
					{
						texts[i] = page.GetText();
						textConfs[i] = page.GetMeanConfidence();
					}
				}

				sw.Stop();

				long elapsedOcr = sw.ElapsedMilliseconds;
			}

			int textIndex = Enumerable.Range(0, textConfs.Length)
				.AtMax(i=>texts[i].Trim().Count(char.IsLetter) > 2)
				.ThenAtMax(i => textConfs[i])
				.Find();

			return texts[textIndex];
		}

		private static Bitmap getPart(Bitmap bitmap, Rectangle rect)
		{
			var textArea = new Bitmap(rect.Width, rect.Height);
			var g = Graphics.FromImage(textArea);
			g.DrawImage(bitmap, new Rectangle(Point.Empty, rect.Size), rect, GraphicsUnit.Pixel);
			return textArea;
		}

		private TesseractEngine _engine;
		private string[] _artists;
		private float[] _artistDistance;
		private LevenstineDistance _distance;
	}
}
