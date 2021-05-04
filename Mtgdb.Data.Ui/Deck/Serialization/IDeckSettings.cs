using System.Collections.Generic;

namespace Mtgdb.Ui
{
	public interface IDeckSettings
	{
		Dictionary<string, int> MainDeckCount { get; set; }

		public List<string> MainDeckOrder { get; set; }

		public Dictionary<string, int> SideDeckCount { get; set; }

		public List<string> SideDeckOrder { get; set; }

		public Dictionary<string, int> MaybeDeckCount { get; set; }

		public List<string> MaybeDeckOrder { get; set; }

		public string DeckName { get; set; }

		public FsPath? DeckFile { get; set; }
	}
}
