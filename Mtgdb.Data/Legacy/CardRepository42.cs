using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mtgdb.Data
{
	public class CardRepository42
	{
		public void LoadFile()
		{
			_lines = File.ReadAllLines(AppDir.Data.AddPath("mtgjson-42-id.csv"));
		}

		public void Load()
		{
			_cardsById = _lines
				.Skip(1)
				.Select(line => line.Split('\t'))
				.ToDictionary(parts => parts[0], parts => (ScryfallId: parts[1], Name: parts[2]));

			_lines = null;
		}

		public (string ScryfallId, string Name) GetById(string id) =>
			_cardsById.TryGet(id);

		public IEnumerable<string> Ids =>
			_cardsById.Keys;

		private Dictionary<string, (string ScryfallId, string Name)> _cardsById;
		private string[] _lines;
	}
}