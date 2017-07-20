using System.Collections.Generic;
using NConfiguration.Ini.Parsing;

namespace NConfiguration.Ini
{
	public sealed class Section
	{
		public string Name { get; private set; }

		public List<KeyValuePair<string, string>> Pairs { get; private set; }

		public Section(string name)
		{
			Name = name;
			Pairs = new List<KeyValuePair<string, string>>();
		}

		public static List<Section> Parse(string text)
		{
			var context = new ParseContext();
			context.ParseSource(text);
			return new List<Section>(context.Sections);
		}
	}
}
