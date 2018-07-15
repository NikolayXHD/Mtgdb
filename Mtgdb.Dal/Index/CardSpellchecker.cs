using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Mtgdb.Index;

namespace Mtgdb.Dal.Index
{
	public class CardSpellchecker : LuceneSpellchecker<int, Card>
	{
		// analyze legality fields
		private const string IndexVerision = "0.40";

		public CardSpellchecker(CardRepository repo, CardDocumentAdapter adapter)
			: base(adapter)
		{
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("suggest");
			_repo = repo;
		}

		protected override IEnumerable<Card> GetObjectsToIndex()
		{
			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException();

			return _repo.SetsByCode.Values
				.Where(FilterSet)
				.SelectMany(s => s.Cards);
		}

		protected override Directory CreateIndex(DirectoryReader reader)
		{
			Directory index;

			if (_version.IsUpToDate)
			{
				using (var fsDirectory = FSDirectory.Open(_version.Directory))
					index = new RAMDirectory(fsDirectory, IOContext.READ_ONCE);

				var spellchecker = CreateSpellchecker();
				spellchecker.Load(index);

				var state = CreateState(reader, spellchecker, loaded: true);
				Update(state);

				return index;
			}

			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException($"{nameof(CardRepository)} must load localizations first");

			_version.CreateDirectory();
			index = base.CreateIndex(reader);

			if (index == null)
				return null;

			index.SaveTo(_version.Directory);
			_version.SetIsUpToDate();

			return index;
		}

		public void InvalidateIndex() =>
			_version.Invalidate();



		public string IndexDirectoryParent
		{
			get => _version.Directory.Parent();
			set => _version = new IndexVersion(value, IndexVerision);
		}

		public string IndexDirectory => _version.Directory;
		public bool IsUpToDate => _version.IsUpToDate;
		public Func<Set, bool> FilterSet { get; set; } = set => true;

		private IndexVersion _version;
		private readonly CardRepository _repo;
	}
}