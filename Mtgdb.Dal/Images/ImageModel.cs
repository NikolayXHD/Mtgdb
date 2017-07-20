using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	public class ImageModel
	{
		private static readonly Regex SetCodeRegex = new Regex(@"^([\w\d]+)\b", RegexOptions.Compiled);
		
		public ImageModel(string fileName, string rootPath, string setCode = null, string artist = null, bool isArt = false)
		{
			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			if (fileNameWithoutExtension == null)
				throw new ArgumentException(@"fileName is null", nameof(fileName));

			string directoryName = Path.GetDirectoryName(fileName);
			if (directoryName == null)
				throw new ArgumentException(@"directoryName is null", nameof(fileName));

			FullPath = fileName;

			string[] parts = fileNameWithoutExtension.Split(new[] { '.' }, StringSplitOptions.None);
			var lastNamePart = Enumerable.Range(0, parts.Length).Last(i =>
				i == 0 ||
				// Richard Garfield, Ph.D..xlhq.jpg
				// S.N.O.T..xlhq.jpg
				// Our Market Research....xlhq.jpg
				parts[i].Length <= 1 ||
				// Sarpadian Empires, Vol. VII.xlhq.jpg
				// Но Thoughtseize.[Size 16x20].jpg
				!(parts[i].StartsWith("[") && parts[i].EndsWith("]")) && parts[i].Contains(' '));

			Type = string.Join(".", parts.Skip(1 + lastNamePart));
			string name = string.Join(".", parts.Take(1 + lastNamePart));

			if (Str.Equals(name, "The Ultimate Nightmare of Wizards of the Coast Customer Service"))
				name = "the ultimate nightmare of wizards of the coastr customer service";
			else if (Str.Equals(name, "Richard Garfield, Ph.D"))
				name = "Richard Garfield, Ph.D.";
			else if (name.StartsWith("Our Market Research", Str.Comparison))
				name = "our market research shows that players like really long card names so we made";
			else
				name = name.Replace(" - ", string.Empty);
			
			ImageName = string.Intern(name);
			var nameParts = ImageName.SplitTalingNumber();
			Name = string.Intern(nameParts.Item1);
			VariantNumber = nameParts.Item2;
			
			HasTransparentCorner = directoryName.Contains("MagicDuels");
			IsPreprocessed = directoryName.Contains("Mtgdb.Pictures");

			IsCrop = FullPath.Contains("Crops") || (Type != null && (Type.Contains("crop") || Type.Contains("xrop")));

			rootPath = AppDir.GetRootPath(rootPath);

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
				var setCodeMatch = SetCodeRegex.Match(directoryName.Substring(rootPath.Length));
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
		

		public string ImageName { get; }
		public bool IsPreprocessed { get; }

		private int getQuality()
		{
			if (string.IsNullOrEmpty(Type))
				return 1;

			if (Type.Equals("full", Str.Comparison))
				return 2;

			if (Type.Equals("xlhq", Str.Comparison))
				return 3;

			return 0;
		}

		public string SetCode { get; }
		public bool SetCodeIsFromAttribute { get; }

		public string Name { get; }
		public int VariantNumber { get; }
		private string Type { get; }
		public string FullPath { get; }
		public bool HasTransparentCorner { get; }
		public bool IsCrop { get; }
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