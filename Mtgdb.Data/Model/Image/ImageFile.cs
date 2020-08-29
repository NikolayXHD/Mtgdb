using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Mtgdb.Data
{
	public class ImageFile
	{
		private static readonly Regex _setCodeRegex = new Regex(@"^([\w\d]+)\b", RegexOptions.IgnoreCase);

		public ImageFile(
			[NotNull] FsPath fileName, FsPath rootPath, string setCode = null, string artist = null,
			bool isArt = false, int? customPriority = null)
		{
			var fileNameWithoutExtension = fileName.Basename(extension: false);
			FsPath directoryName = fileName.Parent();

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
					parts[i].Contains(' ') && !(parts[i].StartsWith("[") && parts[i].EndsWith("]")));

			Type = string.Join(".", parts.Skip(1 + lastNamePart));

			string imageNameRaw = string.Join(".", parts.Take(1 + lastNamePart));
			var imageName = _patternToRemove.Replace(imageNameRaw, string.Empty);
			var replacedName = _nameReplacements.TryGet(imageName) ?? imageName;
			ImageName = string.Intern(replacedName);

			var nameParts = ImageName.SplitTailingNumber();
			Name = string.Intern(nameParts.Item1);
			VariantNumber = nameParts.Item2;

			rootPath = rootPath.ToAppRootedPath();

			if (setCode != null)
			{
				SetCode = string.Intern(setCode);
				SetCodeIsFromAttribute = true;
			}
			else
			{
				var setCodeMatch = _setCodeRegex.Match(directoryName.RelativeTo(rootPath).Value);
				if (setCodeMatch.Success)
					SetCode = string.Intern(setCodeMatch.Value.ToUpperInvariant());
				else
					SetCode = string.Empty;
			}

			Priority = customPriority ?? getPriority();

			if (artist != null)
				Artist = string.Intern(artist);

			IsArt = isArt;
			IsToken = directoryName.Value.IndexOf("Token", Str.Comparison) >= 0;
		}

		public ImageModel ApplyRotation(Card card, bool zoom)
		{
			bool isAftermath =
				Str.Equals(card.Layout, CardLayouts.Aftermath) && Str.Equals(card.Side, CardSides.B);

			if (isAftermath)
				return rotateLeft();

			if (zoom && Str.Equals(card.Layout, CardLayouts.Split))
				return rotateRight();

			if (card.IsFlipped())
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

		private int getPriority()
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

		[NotNull]
		public FsPath FullPath { get; }

		public int Priority { get; }
		public string Artist { get; }
		public bool IsArt { get; }
		public bool IsToken { get; }

		public override string ToString()
		{
			return $"{SetCode} {Name} #{VariantNumber} q{Priority}";
		}

		// -S.xlhq.jpg marks image with artist signature
		private static readonly Regex _patternToRemove = new Regex(" - |-S$| - S$", RegexOptions.IgnoreCase);

		private static readonly Dictionary<string, string> _nameReplacements = new Dictionary<string, string>(Str.Comparer)
		{
			["Will O' The Wisp"] = "Will-O'-The-Wisp",
			["Two Headed Giant of Foriys"] = "Two-Headed Giant of Foriys",
			["Richard Garfield, Ph.D"] = "Richard Garfield, Ph.D.",
			["Tough of the Horned God"] = "Touch of the Horned God",

			// fix split cards from XLHQ named like aftermath
			["Assure»Assemble"] = "AssureAssemble",
			["Connive»Concoct"] = "ConniveConcoct",
			["Discovery»Dispersal"] = "DiscoveryDispersal",
			["Expansion»Explosion"] = "ExpansionExplosion",
			["Find»Finality"] = "FindFinality",
			["Flower»Flourish"] = "FlowerFlourish",
			["Integrity»Intervention"] = "IntegrityIntervention",
			["Invert»Invent"] = "InvertInvent",
			["Response»Resurgence"] = "ResponseResurgence",
			["Status»Statue"] = "StatusStatue",
			["Turn»Burn"] = "TurnBurn",

			// RNA split cards
			["Bedeck»Bedazzle"] = "BedeckBedazzle",
			["Carnival»Carnage"] = "CarnivalCarnage",
			["Collision»Colossus"] = "CollisionColossus",
			["Consecrate»Consume"] = "ConsecrateConsume",
			["Depose»Deploy"] = "DeposeDeploy",
			["Incubation»Incongruity"] = "IncubationIncongruity",
			["Repudiate»Replicate"] = "RepudiateReplicate",
			["Revival»Revenge"] = "RevivalRevenge",
			["Thrash»Threat"] = "ThrashThreat",
			["Warrant»Warden"] = "WarrantWarden",

			// UMA split cards
			["Fire»Ice"] = "FireIce",

			// C19 split
			["DuskDawn"] = "Dusk»Dawn",
			["FarmMarket"] = "Farm»Market",
			["RefuseCooperate"] = "Refuse»Cooperate",
		};
	}
}
