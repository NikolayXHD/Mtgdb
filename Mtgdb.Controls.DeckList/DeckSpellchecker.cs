using System.Collections.Generic;
using JetBrains.Annotations;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Mtgdb.Dal.Index;
using Mtgdb.Index;

namespace Mtgdb.Controls
{
	public class DeckSpellchecker : LuceneSpellchecker<int, DeckModel>
	{
		private const string IndexVerision = "0";

		[UsedImplicitly]
		public DeckSpellchecker(DeckDocumentAdapter adapter)
			: base(adapter)
		{
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("deck").AddPath("suggest");
		}

		protected override IEnumerable<DeckModel> GetObjectsToIndex() =>
			Models;

		protected override Directory CreateIndex(DirectoryReader reader)
		{
			Directory index;

			if (!_indexCreated && _version.IsUpToDate)
			{
				lock (_syncDirectory)
					using (var fsDirectory = FSDirectory.Open(_version.Directory))
						index = new RAMDirectory(fsDirectory, IOContext.READ_ONCE);

				var spellchecker = CreateSpellchecker();
				spellchecker.Load(index);

				var state = CreateState(reader, spellchecker, loaded: true);
				Update(state);

				_indexCreated = true;
				return index;
			}

			index = base.CreateIndex(reader);

			if (index == null)
				return null;

			lock (_syncDirectory)
				_version.CreateDirectory();

			index.SaveTo(_version.Directory);
			_version.SetIsUpToDate();

			_indexCreated = true;
			return index;
		}

		public IReadOnlyList<DeckModel> Models { get; set; }

		public string IndexDirectoryParent
		{
			get => _version.Directory.Parent();
			set => _version = new IndexVersion(value, IndexVerision);
		}

		private IndexVersion _version;
		private bool _indexCreated;

		private readonly object _syncDirectory = new object();
	}
}