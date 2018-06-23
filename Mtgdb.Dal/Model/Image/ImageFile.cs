using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	public class ImageFile
	{
		private static readonly Regex _setCodeRegex = new Regex(@"^([\w\d]+)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public ImageFile(string fileName, string rootPath, string setCode = null, string artist = null, bool isArt = false)
		{
			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			if (fileNameWithoutExtension == null)
				throw new ArgumentException(@"fileName is null", nameof(fileName));

			string directoryName = Path.GetDirectoryName(fileName);
			if (directoryName == null)
				throw new ArgumentException(@"directoryName is null", nameof(fileName));

			FullPath = fileName;

			string[] parts = fileNameWithoutExtension.Split(Array.From('.'), StringSplitOptions.None);
			var lastNamePart = Enumerable.Range(0, parts.Length)
				.Last(i =>
					i == 0 ||
					// Richard Garfield, Ph.D..xlhq.jpg
					// S.N.O.T..xlhq.jpg
					// Our Market Research....xlhq.jpg
					parts[i].Length <= 1 ||
					// Sarpadian Empires, Vol. VII.xlhq.jpg
					// Но Thoughtseize.[Size 16x20].jpg
					!(parts[i].StartsWith("[") && parts[i].EndsWith("]")) && parts[i].Contains(' '));

			Type = string.Join(".", parts.Skip(1 + lastNamePart));

			var imageName = string.Join(".", parts.Take(1 + lastNamePart))
				.Replace(" - ", string.Empty);

			ImageName = string.Intern(ImageNamePatcher.PatchFileName(imageName));

			var nameParts = ImageName.SplitTalingNumber();
			Name = string.Intern(nameParts.Item1);
			VariantNumber = nameParts.Item2;

			rootPath = rootPath.ToAppRootedPath();

			if (!rootPath.EndsWith("\\"))
				rootPath = rootPath + "\\";

			if (!directoryName.EndsWith("\\"))
				directoryName = directoryName + "\\";

			if (setCode != null)
			{
				SetCode = string.Intern(setCode);
				SetCodeIsFromAttribute = true;
			}
			else
			{
				var setCodeMatch = _setCodeRegex.Match(directoryName.Substring(rootPath.Length));
				if (setCodeMatch.Success)
					SetCode = string.Intern(setCodeMatch.Value.ToUpperInvariant());
				else
					SetCode = string.Empty;
			}

			Quality = getQuality();

			if (artist != null)
				Artist = string.Intern(artist);

			IsArt = isArt;
			IsToken = directoryName.IndexOf("Tokens", Str.Comparison) >= 0;
		}

		public ImageModel ApplyRotation(Card card, bool zoom)
		{
			bool isAftermath =
				Str.Equals(card.Layout, "aftermath") &&
				card.Names?.Count == 2 &&
				Str.Equals(card.NameEn, card.Names[1]);

			if (isAftermath)
				return rotateLeft();

			if (zoom && Str.Equals(card.Layout, "split"))
				return rotateRight();

			bool isFlip = Str.Equals(card.Layout, "flip")
				&& card.Names?.Count == 2 &&
				Str.Equals(card.NameEn, card.Names[1]);

			if (isFlip)
				return rotate180();

			return NonRotated();
		}

		public ImageModel NonRotated()
		{
			return new ImageModel(this);
		}

		private ImageModel rotateLeft()
		{
			return new ImageModel(this, RotateFlipType.Rotate270FlipNone);
		}

		private ImageModel rotateRight()
		{
			return new ImageModel(this, RotateFlipType.Rotate90FlipNone);
		}

		private ImageModel rotate180()
		{
			return new ImageModel(this, RotateFlipType.Rotate180FlipNone);
		}

		private int getQuality()
		{
			if (string.IsNullOrEmpty(Type))
				return 1;

			if (Str.Equals(Type, "full"))
				return 2;

			if (Str.Equals(Type, "xlhq"))
				return 3;

			return 0;
		}

		public string ImageName { get; }

		public string SetCode { get; }
		public bool SetCodeIsFromAttribute { get; }

		public string Name { get; }
		public int VariantNumber { get; }
		private string Type { get; }
		public string FullPath { get; }
		public int Quality { get; }
		public string Artist { get; }
		public bool IsArt { get; }
		public bool IsToken { get; }

		public override string ToString()
		{
			return $"{SetCode} {Name} #{VariantNumber} q{Quality}";
		}
	}
}