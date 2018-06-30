using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Mtgdb.Controls;
using Mtgdb.Gui.Resx;

namespace Mtgdb.Gui
{
	public static class IconRecognizerFactory
	{
		public static IconRecognizer Create()
		{
			var entries = ResourcesCost.ResourceManager
				.GetResourceSet(CultureInfo.CurrentCulture, true, true)
				.Cast<DictionaryEntry>()
				.ToArray();

			var mappings = entries.Select(_ => new
			{
				symbols = getSymbol((string) _.Key), 
				image = (Bitmap) _.Value
			});

			var imageByText = new Dictionary<string, Bitmap>(Str.Comparer);

			foreach (var mapping in mappings)
			{
				if (mapping.symbols == null || mapping.symbols.Length == 0)
					continue;

				foreach (string symbol in mapping.symbols)
					imageByText[symbol] = mapping.image;
			}

			var nonShadowedIcons = new HashSet<string>(Str.Comparer)
			{
				"E",
				"Q"
			};

			var iconRecognizer = new IconRecognizer(imageByText, nonShadowedIcons);
			return iconRecognizer;
		}

		private static string[] getSymbol(string key)
		{
			string name = key.TrimStart('_');

			if (name == "05")
				return new[] { "0.5", ".5", "1/2", "½" };

			if (name == "i")
				return new[] { "∞", "oo" };

			if (name.Length == 1 || name.Length == 2 && name.All(char.IsDigit) || name == "100" || name == "1000000" || name == "chaos" || name == "any")
				return new[] { name };

			if (name.Length == 2)
				return new[] { $"{name[0]}/{name[1]}", $"{name[1]}/{name[0]}", $"{name[0]}{name[1]}", $"{name[1]}{name[0]}" };

			return null;
		}
	}
}